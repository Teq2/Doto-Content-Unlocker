using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker
{
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
}

namespace Doto_Unlocker.VPK
{
    class VpkFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }

        public VPKDirectoryEntryInfo Info;

        public string FullPath
        {
            get
            {
                return string.Format("{0}/{1}.{2}", Path, Name, Ext);
            }
        }
    }
}
