using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker
{
    class FastBinaryReader
    {
        byte[] data;

        internal int Pos
        {
            get;
            set;
        }

        internal FastBinaryReader(ref byte[] rawData)
        {
            data = rawData;
        }

        internal uint ReadUInt32()
        {
            var res = BitConverter.ToUInt32(data, Pos);
            Pos += sizeof(uint);
            return res;
        }

        internal ushort ReadUInt16()
        {
            var res = BitConverter.ToUInt16(data, Pos);
            Pos += sizeof (ushort);
            return res;
        }

        internal byte[] ReadBytes(int len)
        {
            byte[] res = new byte[len];
            Buffer.BlockCopy(data, Pos, res, 0, len);
            Pos += len;
            return res;
        }

        internal byte ReadByte()
        {
            return data[Pos++];
        }

        internal byte PeekByte()
        {
            return data[Pos];
        }

        /// <summary>
        /// Read null-terminated string
        /// </summary>
        /// <returns>String without null-termitator</returns>
        internal string ReadSring()
        {
            int pos_start = Pos;
            while (data[Pos++] != '\0') ;
            var len = Pos - pos_start - 1; // don't include '\0'

            return System.Text.Encoding.UTF8.GetString(data, pos_start, len);
        }

        internal string ReadSring(int position)
        {
            return null;
        }
    }
}
