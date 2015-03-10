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
    struct VPKHeaderEx
    {
        public uint Signature;
        public uint Version;
        public uint TreeLength; // The length of the directory

        // for VPK_v2 only
        public uint Unknown1;
        public uint FooterLength;
        public uint Unknown3;
        public uint Unknown4;
    };

    class VpkEntry : Doto_Unlocker.VPK.IVpkEntry
    {
        public string Path {get;set;}
        public string Name {get;set;}
        public string Ext {get;set;}

        public VPKDirectoryEntryInfo Info;

        public string FullPath
        {
            get
            {
                return Path + '/' + Name + '.' + Ext;
            }
        }
    }

    class VpkEntryNameComparer : IEqualityComparer<VpkEntry>
    {
        public bool Equals(VpkEntry x, VpkEntry y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(VpkEntry obj)
        {
            return obj != null ? obj.Name.GetHashCode() : 0;
        }
    }

    // Info taken from volvo-wiki
    struct VPKDirectoryEntryInfo
    {
        public uint CRC; // A 32bit CRC of the file's data.
        public ushort PreloadBytes; // The number of bytes contained in the index file.
	    // A zero based index of the archive this file's data is contained in.
	    // If 0x7fff, the data follows the directory.
        public ushort ArchiveIndex;
	    // If ArchiveIndex is 0x7fff, the offset of the file data relative to the end of the directory (see the header for more details).
	    // Otherwise, the offset of the data from the start of the specified archive.
        public uint EntryOffset;
	    // If zero, the entire file is stored in the preload data.
	    // Otherwise, the number of bytes stored starting at EntryOffset.
        public uint EntryLength;
	    //public ushort Terminator;
        public byte[] PreloadedData;
    };

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

    class VpkFile
    {
        const uint VPK_magic = 0x55AA1234;
        const ushort VPK_terminator = 0xFFFF;
        const ushort VPK_dirOffset = 0x7FFF;
        byte[] rawData;
        VPKHeaderEx header;
        List<VpkEntry> entries;

        public VpkEntry this[int idx]
        {
            get
            {
                return entries[idx];
            }
        }

        public List<VpkEntry> Data
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
        public VpkFile(string path)
        {
            rawData = File.ReadAllBytes(path); // preloading is faster
            Parse();
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
                    throw new InvalidVpkFileStructureException("Unexpected end of file", ex) { FailType = VPKFail.StructureFail };
                }
                else
                    throw;
            }
        }

        private void LoadHeader(FastBinaryReader reader)
        {
            header.Signature = reader.ReadUInt32();
            if (header.Signature != VPK_magic) throw new InvalidVpkFileStructureException() { FailType = VPKFail.MagicFail };
            header.Version = reader.ReadUInt32();
            if (header.Version > 2) throw new InvalidVpkFileStructureException() { FailType = VPKFail.UnsupportedVer };
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
            entries = new List<VpkEntry>();
            VpkEntry prevEntry, entry;

            // first entry
            entry = new VpkEntry();
            entry.Ext = reader.ReadSring();
            entry.Path = reader.ReadSring();
            entry.Name = reader.ReadSring();
            LoadEntryInfo(reader, ref entry);
            entries.Add(entry);

            while (true) 
            {
                prevEntry = entry;
                entry = new VpkEntry();    

                if (!LoadEntryStr(reader, ref entry)) break; // EoF, last entry is 0x00 0x00 0x00
                // skipping has been used ?
                if (string.IsNullOrEmpty(entry.Ext)) entry.Ext = prevEntry.Ext;
                if (string.IsNullOrEmpty(entry.Path)) entry.Path = prevEntry.Path;
                // some entries has syntax EXT 0x00 PATH 0x00 0x00, without ENTRYINFO struc
                if (!string.IsNullOrEmpty(entry.Name))
                {
                    LoadEntryInfo(reader, ref entry);
                    entries.Add(entry);
                }
            }
        }

        private bool LoadEntryStr(FastBinaryReader reader, ref VpkEntry entry)
        {
            if (reader.PeekByte() == 0) // first control-char
            {
                reader.Pos++;
                if (reader.PeekByte() == 0) // second control-char
                {
                    reader.Pos++;
                    entry.Ext = reader.ReadSring();
                    if (string.IsNullOrEmpty(entry.Ext)) return false; // end of file (0x00 0x00 0x00)
                }         
                entry.Path = reader.ReadSring();
            }

            if (reader.PeekByte() != 0) // stream position could be changed
                entry.Name = reader.ReadSring();
            return true;
        }

        private void LoadEntryInfo(FastBinaryReader reader, ref VpkEntry entry)
        {
            entry.Info.CRC = reader.ReadUInt32();
            entry.Info.PreloadBytes = reader.ReadUInt16();
            entry.Info.ArchiveIndex = reader.ReadUInt16();
            entry.Info.EntryOffset = reader.ReadUInt32();
            entry.Info.EntryLength = reader.ReadUInt32();
            var terminator = reader.ReadUInt16();
            if (terminator != VPK_terminator) throw new InvalidVpkFileStructureException() { FailType = VPKFail.StructureFail, SuccessfullyParsed = entries.Count };

            if (entry.Info.PreloadBytes > 0)
            {
                entry.Info.PreloadedData = reader.ReadBytes(entry.Info.PreloadBytes);
            }
            //if (entry.Info.ArchiveIndex == VPK_dirOffset && entry.Info.EntryOffset != 0) not used in dota 2
        }
    }
}
