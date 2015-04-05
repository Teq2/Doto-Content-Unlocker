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

namespace Doto_Unlocker.VPK
{
    class VpkArchive
    {
        private string dirPath;
        private string archiveName;
        private DateTime dirLoadedDate;
        private VpkDir dir;

        public VpkArchive(string dirPath, string archiveName)
        {
            if (string.IsNullOrEmpty(dirPath)) throw new ArgumentNullException("Path");
            if (string.IsNullOrEmpty(archiveName)) throw new ArgumentNullException("Archive");

            this.dirPath = dirPath;
            this.archiveName = archiveName;

            dir = new VpkDir(string.Format("{0}/{1}_dir.vpk", dirPath, archiveName));
            dirLoadedDate = DateTime.Now;
        }

        public IEnumerator<VpkFile> GetEnumerator()
        {
            return dir.Data.GetEnumerator();
        }

        /// <exception cref="VpkFileIOException">Vpk file access error</exception>
        /// <exception cref="VpkFileOutdatedException">If pack-file updated after dir parsed</exception>
        public byte[] ReadFile(VpkFile fileEntry)
        {
            VPKDirectoryEntryInfo fileStruc = fileEntry.Info;

            // was file fully preloaded?
            if (fileStruc.PreloadBytes > 0 && fileStruc.EntryLength == 0) 
                return fileStruc.PreloadedData;

            DateTime fileTime = default(DateTime);
            var vpkFileName = String.Format("{0}/{1}_{2:D3}.vpk", dirPath, archiveName, fileStruc.ArchiveIndex);
            try {
                fileTime = CheckFileWasUpdated(vpkFileName);

                using (var file = new FileStream(vpkFileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] fileData = new byte[fileStruc.PreloadBytes + fileStruc.EntryLength];
                    // ok, file opened, fill preloaded bytes in
                    if (fileStruc.PreloadBytes > 0)
                        Buffer.BlockCopy(fileStruc.PreloadedData, 0, fileData, 0, fileStruc.PreloadBytes);

                    file.Position = fileStruc.EntryOffset;
                    file.Read(fileData, fileStruc.PreloadBytes, (int)fileStruc.EntryLength);
                    return fileData;
                }
            }
            catch (Exception e)
            {
                if (e is IOException || e is UnauthorizedAccessException)
                    throw new VpkFileIOException("Err accessing vpk file", e)
                    {
                        VpkFileName = vpkFileName,
                        DirLoaded = dirLoadedDate,
                        LastUpdated = fileTime
                    };
                else
                    throw;
            }
        }

        /// <exception cref="VpkFileIOException">Vpk file access error</exception>
        /// <exception cref="VpkFileOutdatedException">If pack-file updated after being parsed</exception>
        public void OverwriteFile(VpkFile fileEntry, byte[] data, bool restoreTimestamp=false)
        {
            VPKDirectoryEntryInfo fileStruc = fileEntry.Info;

            string vpkDataFileName;
            if (fileStruc.PreloadBytes > 0)
            {
                // part of a file located in dir-file
                vpkDataFileName = String.Format("{0}/{1}_dir.vpk", dirPath, archiveName);
                WriteDataPortionToFile(vpkDataFileName, data, 0, fileStruc.PreloadedDataOffset, fileStruc.PreloadBytes, restoreTimestamp);
                // for further reading
                Buffer.BlockCopy(data, 0, fileStruc.PreloadedData, 0, fileStruc.PreloadBytes);
            }

            if (fileStruc.EntryLength != 0)
            {
                vpkDataFileName = String.Format(@"{0}/{1}_{2:D3}.vpk", dirPath, archiveName, fileStruc.ArchiveIndex);
                WriteDataPortionToFile(vpkDataFileName, data, fileStruc.PreloadBytes, fileStruc.EntryOffset, fileStruc.EntryLength, restoreTimestamp);
            }
        }

        /// <exception cref="VpkFileIOException">Vpk file access error</exception>
        /// <exception cref="VpkFileOutdatedException">If pack-file updated after being parsed</exception>
        public byte[] ReadFile(string fullPath)
        {
            var entry = FindFile(fullPath);
            return entry != null ? ReadFile(entry) : null;
        }

        /// <summary>root folder is 0x20</summary>
        /// <param name="fullPath">full relative path with filename</param>
        public VpkFile FindFile(string fullPath)
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

        public VpkFile[] FindFiles(string partialPath)
        {
            return dir.Data.Where(e => e.Path.StartsWith(partialPath)).ToArray();
        }

        public VpkFile[] FindFiles(string partialPath, string ext)
        {
            return dir.Data.Where(e => e.Path.StartsWith(partialPath) && e.Ext == ext).ToArray();
        }

        private void WriteDataPortionToFile(string fileName, byte[] data, int srcOffset, int dstOffset, int dataLength, bool restoreTimestamp = false)
        {
            DateTime fileTime = default(DateTime);
            try
            {
                fileTime = CheckFileWasUpdated(fileName);
                using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Write))
                {
                    file.Position = dstOffset;
                    file.Write(data, srcOffset, dataLength);
                }
                if (restoreTimestamp) File.SetLastWriteTime(fileName, fileTime);
            }
            catch (Exception e)
            {
                if (e is IOException || e is UnauthorizedAccessException)
                    throw new VpkFileIOException("Err accessing vpk file", e) 
                    { 
                        VpkFileName = fileName, DirLoaded = dirLoadedDate, LastUpdated = fileTime
                    };
                else
                    throw;
            }
        }

        private DateTime CheckFileWasUpdated(string fileName)
        {
            DateTime lastWrite = File.GetLastWriteTime(fileName);
            if (DateTime.Compare(dirLoadedDate, lastWrite) <= 0)
                throw new VpkFileOutdatedException() { VpkFileName = fileName, LastUpdated = lastWrite, DirLoaded = dirLoadedDate };
            return lastWrite;
        }
    }
}
