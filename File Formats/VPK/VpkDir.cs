/*
 * Doto-Content-Unlocker, tool for unlocking custom dota2 content.
 * 
 *  Copyright (c) 2014 Teq, https://github.com/Teq2/Doto-Content-Unlocker
 * 
 *  Permission is hereby granted, free of charge, to any person obtaining a copy of 
 *  this software and associated documentation files (the "Software"), to deal in the 
 *  Software without restriction, including without limitation the rights to use, copy, 
 *  modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 *  and to permit persons to whom the Software is furnished to do so, subject to the 
 *  following conditions:
 * 
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 *  INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 *  PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 *  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 *  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 *  OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Doto_Unlocker.VPK
{
    /* Structures taken from:
     * https://developer.valvesoftware.com/wiki/VPK_File_Format */

    struct VPKHeaderEx
    {
        public uint Signature;
        public uint Version;
        public uint TreeLength; // The length of the directory tree

        // for VPK_v2 only
        public uint Unknown1;
        public uint FooterLength;
        public uint Unknown3;
        public uint Unknown4;
    };

    struct VPKDirectoryEntryInfo
    {
        public uint CRC;
        public ushort PreloadBytes;
	    // A zero based index of the archive this file's data is contained in.
	    // If 0x7fff, the data follows the directory.
        public ushort ArchiveIndex;
	    // If ArchiveIndex is 0x7fff, the offset of the file data relative to the end of the directory (see the header for more details).
	    // Otherwise, the offset of the data from the start of the specified archive.
        // uint changed to int
        public int EntryOffset;
	    // If zero, the entire file is stored in the preload data.
	    // Otherwise, the number of bytes stored starting at EntryOffset.
        // uint changed to int
        public int EntryLength;
	    // Removed
        //public ushort Terminator;
        public byte[] PreloadedData;
        // Added
        public int PreloadedDataOffset; 
    };

    class VpkEntryNameComparer : IEqualityComparer<VpkFile>
    {
        public bool Equals(VpkFile x, VpkFile y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(VpkFile obj)
        {
            return obj != null ? obj.Name.GetHashCode() : 0;
        }
    }

    public enum VPKFail { StructureFail, MagicFail, UnsupportedVer };

    public class InvalidVpkFileStructureException : Exception, ISerializable
    {
        public VPKFail FailType { get; set; }
        public uint Version { get; set; }
        public int SuccessfullyParsed { get; set; }

        public InvalidVpkFileStructureException() : base() { }
        public InvalidVpkFileStructureException(string message) : base() { }
        public InvalidVpkFileStructureException(string message, Exception inner) : base() { }
        protected InvalidVpkFileStructureException(SerializationInfo info, StreamingContext context) : base() { }
    }

    class VpkDir
    {
        const uint VpkMagic = 0x55AA1234;
        const ushort VpkTerminator = 0xFFFF;
        const ushort VpkDirOffset = 0x7FFF;
        byte[] rawData;
        VPKHeaderEx header;
        List<VpkFile> entries;

        public int Count { get { return entries.Count; } }

        public VpkFile this[int idx]
        {
            get
            {
                return entries[idx];
            }
        }

        public IList<VpkFile> Data
        {
            get
            {
                return entries;
            }
        }

        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidVpkFileStructureException"></exception>
        /// <exception cref="UnsupportedFileVersionException"></exception>
        public VpkDir(string path)
        {
            try
            {
                rawData = File.ReadAllBytes(path); // preloading is faster
                Parse();
            }
            catch (Exception e)
            {
                if (e is IOException || e is UnauthorizedAccessException)
                    throw new VpkFileIOException("Err accessing vpk file", e)
                    {
                        VpkFileName = path,
                    };
                else
                    throw;
            }
        }

        private void Parse()
        {
            FastBinaryReader reader = new FastBinaryReader(ref rawData);
            try
            {
                LoadHeader(reader);
                LoadEntries(reader);
            }
            catch(Exception ex)
            {
                if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is ArgumentException)
                {
                    throw new InvalidVpkFileStructureException("Unexpected end of file", ex) { FailType = VPKFail.StructureFail, Version = header.Version };
                }
                else
                    throw;
            }
        }

        private void LoadHeader(FastBinaryReader reader)
        {
            header.Signature = reader.ReadUInt32();
            if (header.Signature != VpkMagic) throw new InvalidVpkFileStructureException() { FailType = VPKFail.MagicFail };
            header.Version = reader.ReadUInt32();
            if (header.Version > 2) throw new InvalidVpkFileStructureException() { FailType = VPKFail.UnsupportedVer, Version = header.Version };
            header.TreeLength = reader.ReadUInt32();

            // additional header data
            if (header.Version == 2)
            {
                header.Unknown1 = reader.ReadUInt32();
                header.FooterLength = reader.ReadUInt32();
                header.Unknown3 = reader.ReadUInt32();
                header.Unknown4 = reader.ReadUInt32();
            }
        }

        private void LoadEntries(FastBinaryReader reader)
        {
            entries = new List<VpkFile>();
            VpkFile prevEntry, entry;

            // first entry (has simplified format)
            entry = new VpkFile();
            entry.Ext = reader.ReadString();
            entry.Path = reader.ReadString();
            entry.Name = reader.ReadString();
            LoadEntryInfo(reader, ref entry);
            entries.Add(entry);

            while (true) 
            {
                prevEntry = entry;
                entry = new VpkFile();    

                if (!LoadEntryStr(reader, ref entry)) break; // EoF, last entry is 0x00 0x00 0x00
                // skipping has been used ?
                if (entry.Ext == null) entry.Ext = prevEntry.Ext;
                if (entry.Path == null) entry.Path = prevEntry.Path;
                // some entries have syntax EXT 0x00 PATH 0x00 0x00, without ENTRYINFO struc, skip them
                if (entry.Name != null)
                {
                    LoadEntryInfo(reader, ref entry);
                    entries.Add(entry);
                }

                System.Diagnostics.Debug.Assert((entry.Ext != null) && (entry.Path != null), "Nulls in vpkentry");
            }
        }

        private bool LoadEntryStr(FastBinaryReader reader, ref VpkFile entry)
        {
            if (reader.PeekByte() == 0) // first control-char
            {
                reader.Pos++;
                if (reader.PeekByte() == 0) // second control-char
                {
                    reader.Pos++;
                    entry.Ext = reader.ReadString();
                    if (string.IsNullOrEmpty(entry.Ext)) return false; // end of file (0x00 0x00 0x00)
                }         
                entry.Path = reader.ReadString();
            }

            if (reader.PeekByte() != 0) // stream position could be changed
                entry.Name = reader.ReadString();
            return true;
        }

        private void LoadEntryInfo(FastBinaryReader reader, ref VpkFile entry)
        {
            entry.Info.CRC = reader.ReadUInt32();
            entry.Info.PreloadBytes = reader.ReadUInt16();
            entry.Info.ArchiveIndex = reader.ReadUInt16();
            entry.Info.EntryOffset = reader.ReadInt32();
            entry.Info.EntryLength = reader.ReadInt32();
            var terminator = reader.ReadUInt16();
            if (terminator != VpkTerminator) throw new InvalidVpkFileStructureException() { FailType = VPKFail.StructureFail, Version = header.Version, SuccessfullyParsed = entries.Count };

            if (entry.Info.PreloadBytes > 0)
            {
                entry.Info.PreloadedDataOffset = reader.Pos;
                entry.Info.PreloadedData = reader.ReadBytes(entry.Info.PreloadBytes);
            }
            //if (entry.Info.ArchiveIndex == VpkDirOffset && entry.Info.EntryOffset != 0) not used in dota 2
        }
    }
}
