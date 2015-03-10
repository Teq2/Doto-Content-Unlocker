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
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

[Serializable]
class VpkFileOutdatedException : Exception, ISerializable
{
    public VpkFileOutdatedException() : base() { }
    public VpkFileOutdatedException(string message) : base() { }
    public VpkFileOutdatedException(string message, Exception inner) : base() { }
    protected VpkFileOutdatedException(SerializationInfo info, StreamingContext context) : base() { }

    public string VpkFileName { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime DirLoaded { get; set; }
}

[Serializable]
class VpkFileIOException : Exception, ISerializable
{
    public VpkFileIOException() : base() { }
    public VpkFileIOException(string message) : base() { }
    public VpkFileIOException(string message, Exception inner) : base() { }
    protected VpkFileIOException(SerializationInfo info, StreamingContext context) : base() { }

    public string VpkFileName { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime DirLoaded { get; set; }
}

// ADD CRC32 CHECK?

namespace Doto_Unlocker.VPK
{
    class VpkArchive
    {
        private string m_path;
        private string m_archive;
        private DateTime dir_loaded;
        private VpkFile dir;

        public VpkArchive(string dirPath, string archiveName)
        {
            if (string.IsNullOrEmpty(dirPath)) throw new ArgumentNullException("Path");
            if (string.IsNullOrEmpty(archiveName)) throw new ArgumentNullException("Archive");

            m_path = dirPath;
            m_archive = archiveName;

            dir = new VpkFile(m_path + "\\" + m_archive + "_dir.vpk");
            dir_loaded = DateTime.Now;
        }

        /// <exception cref="VpkFileIOException">Vpk file access error</exception>
        /// <exception cref="VpkFileOutdatedException">If pack-file updated after dir parsed</exception>
        public byte[] ReadFile(VpkEntry fileEntry)
        {
            VPKDirectoryEntryInfo fileStruc = fileEntry.Info;
            // is file preloaded?
            if (fileStruc.PreloadBytes > 0) return fileStruc.PreloadedData;

            var fname = String.Format(@"{0}\{1}_{2:D3}.vpk", m_path, m_archive, fileStruc.ArchiveIndex);
            CheckVersion(fname);

            // load
            FileStream file = null;
            try
            {
                file = new FileStream(fname, FileMode.Open, FileAccess.Read);
                file.Position = fileStruc.EntryOffset;
                byte[] data = new byte[fileStruc.EntryLength];
                file.Read(data, 0, (int)fileStruc.EntryLength);
                return data;
            }
            catch (IOException e)
            {
                throw new VpkFileIOException("Err accessing vpk file", e) { VpkFileName = fname };
            }
            catch (System.Security.SecurityException e)
            {
                throw new VpkFileIOException("Err accessing vpk file", e) { VpkFileName = fname };
            }
            finally
            {
                if (file != null) file.Close();
            }
        }

        /// <exception cref="VpkFileIOException">Vpk file access error</exception>
        /// <exception cref="VpkFileOutdatedException">If pack-file updated after dir parsed</exception>
        public void EditFile(VpkEntry fileEntry, ref byte[] data, bool restoreTimestamp)
        {
            VPKDirectoryEntryInfo fileStruc = fileEntry.Info;
            // preloaded files aren't supporting
            if (fileStruc.PreloadBytes > 0) return;

            var fname = String.Format(@"{0}\{1}_{2:D3}.vpk", m_path, m_archive, fileStruc.ArchiveIndex);
            CheckVersion(fname);

            // overwrite
            FileStream file = null;
            var fileTime = File.GetLastWriteTime(fname);
            try
            {
                file = new FileStream(fname, FileMode.Open, FileAccess.Write);
                file.Position = fileStruc.EntryOffset;
                file.Write(data, 0, data.Length);
            }
            catch (IOException e)
            {
                throw new VpkFileIOException("Err accessing vpk file", e) { VpkFileName = fname };
            }
            catch (System.Security.SecurityException e)
            {
                throw new VpkFileIOException("Err accessing vpk file", e) { VpkFileName = fname };
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    if (restoreTimestamp) File.SetLastWriteTime(fname, fileTime);
                }
            }
        }

        /// <exception cref="VpkFileIOException">Vpk file access error</exception>
        /// <exception cref="VpkFileOutdatedException">If pack-file updated after dir parsed</exception>
        public byte[] ReadFile(string fullPath)
        {
            var entry = FindFile(fullPath);

            if (entry != null)
                return ReadFile(entry);
            else
                return null;
        }

        /// <summary>root folder is 0x20</summary>
        /// <param name="fullPath">full relative path with filename, without starting slash</param>
        public VpkEntry FindFile(string fullPath)
        {
            var match = Regex.Match(fullPath, @"^[\\/]*(.+)[/\\]+(.+)\.(.+)$");
            if (match.Success)
            {
                string path = match.Groups[1].Value;
                string name = match.Groups[2].Value;
                string ext = match.Groups[3].Value;
                return dir.Data.SingleOrDefault((entry) => entry.Path.Equals(path) && entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && entry.Ext.Equals(ext));
            }
            return null;
        }

        public VpkEntry[] FindFiles(string partialPath)
        {
            return dir.Data.Where(e => e.Path.StartsWith(partialPath)).ToArray();
        }

        public VpkEntry[] FindFiles(string partialPath, string ext)
        {
            return dir.Data.Where(e => e.Path.StartsWith(partialPath) && e.Ext == ext).ToArray();
        }

        private void CheckVersion(string fname)
        {
            DateTime vpkTime = File.GetLastWriteTime(fname);
            if (DateTime.Compare(dir_loaded, vpkTime) <= 0)
                throw new VpkFileOutdatedException() { VpkFileName = fname, LastUpdated = vpkTime, DirLoaded = dir_loaded };
        }
    }
}
