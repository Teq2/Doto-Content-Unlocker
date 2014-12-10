using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker.VSIF
{
    public enum VSIFFail { StructureFail, MagicFail, UnsupportedVer };

    public class InvalidVsifFileStructureException : Exception, ISerializable
    {
        public VSIFFail FailType { get; set; }
        public uint Version { get; set; }
        public int SuccessfullyParsed { get; set; }

        public InvalidVsifFileStructureException() : base() { }
        public InvalidVsifFileStructureException(string message) : base() { }
        public InvalidVsifFileStructureException(string message, Exception inner) : base() { }
        protected InvalidVsifFileStructureException(SerializationInfo info, StreamingContext context) : base() { }
    }

    struct VsifHeader
    {
        public uint Signature;   // 4-byte identified used for file identification. Always big-endian "VSIF".
        public uint Version;     // scenes.image file version. 2 is used.
        public uint ScenesCount; // Total number of scene files.
        public uint StringsCount;// Number of strings in the string pool (explained later).
        public uint SceneOffset; // Offset to the scene entries (explained later).
    };

    struct VsifEntry
    {
        public uint CRC32;  // CRC32 hash of scene file name, in scenes\???.vcd format. All file names are lower-case and have \ as path separator.
        public byte[] Data; 
        public VsifSummary Summary;
    }

    struct VsifSummary
    {
        public uint val1;
        public uint val2;
        public List<uint> StringsIndexes;
    }

    class VsifFile
    {
        const uint VsifMagic = 0x46495356; // 'VSIF'
        const uint VsifVersion = 3;
        byte[] rawData;
        List<string> StringsPool = new List<string>();
        List<VsifEntry> Entries = new List<VsifEntry>();

        public VsifFile(ref byte[] rawData)
        {
            this.rawData = rawData;
        }

        public void Parse()
        {
            FastBinaryReader reader = new FastBinaryReader(ref rawData);
            try
            {
                VsifHeader header = LoadHeader(reader);
                LoadStrings(reader, ref header);
                LoadEntries(reader, ref header);
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is ArgumentException)
                {
                    //throw new InvalidFileStructureException("Unexpected end of file", ex) { FailType = VPKFail.StructureFail };
                }
                else
                    throw;
            }
        }

        private VsifHeader LoadHeader(FastBinaryReader reader)
        {
            VsifHeader header = new VsifHeader();
            header.Signature = reader.ReadUInt32();
            if (header.Signature != VsifMagic) throw new InvalidVsifFileStructureException() { FailType = VSIFFail.MagicFail };
            header.Version = reader.ReadUInt32();
            if (header.Version != VsifVersion) throw new InvalidVsifFileStructureException() { FailType = VSIFFail.UnsupportedVer };
            header.ScenesCount = reader.ReadUInt32();
            header.StringsCount = reader.ReadUInt32();
            header.SceneOffset = reader.ReadUInt32();
            return header;
        }

        private void LoadStrings(FastBinaryReader reader, ref VsifHeader header)
        {
            uint FirstStringOffest = reader.ReadUInt32();
            reader.Pos = (int)FirstStringOffest;

            StringsPool = new List<string>();
            for (int i = 0; i < header.StringsCount; i++)
            {
                string str = reader.ReadSring();
                if (i == 0x12e4)
                {

                }
                StringsPool.Add(str);
            }
        }

        private void LoadEntries(FastBinaryReader reader, ref VsifHeader header)
        {
            reader.Pos = (int)header.SceneOffset;
            Entries = new List<VsifEntry>();
            for (int i = 0; i < header.ScenesCount; i++)
            {
                var pos_1 = reader.Pos;
                var entry = new VsifEntry();
                entry.CRC32 = reader.ReadUInt32();
                //entry.Data
                uint dataOffset = reader.ReadUInt32();
                uint dataLen = reader.ReadUInt32();
                uint summaryStructOffset = reader.ReadUInt32();
                // save pos
                int nextEntry = reader.Pos;
                // data
                reader.Pos = (int)dataOffset;
                entry.Data = reader.ReadBytes((int)dataLen);
                // summary
                reader.Pos = (int)summaryStructOffset;
                entry.Summary.val1 = reader.ReadUInt32();
                entry.Summary.val2 = reader.ReadUInt32();
                var indexes = reader.ReadUInt32();
                entry.Summary.StringsIndexes = new List<uint>();
                for (int j = 0; j < indexes; j++)
                {
                    var index = reader.ReadUInt32();
                    entry.Summary.StringsIndexes.Add(index);
                }
                // restore 
                reader.Pos = nextEntry;

                Entries.Add(entry);
            }
        }
    }
}
