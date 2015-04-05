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

        internal int ReadInt32()
        {
            var res = BitConverter.ToInt32(data, Pos);
            Pos += sizeof(int);
            return res;
        }

        internal ushort ReadUInt16()
        {
            var res = BitConverter.ToUInt16(data, Pos);
            Pos += sizeof(ushort);
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
        internal string ReadString()
        {
            int pos_start = Pos;
            while (data[Pos++] != '\0') ;
            var len = Pos - pos_start - 1; // don't include '\0'

            return System.Text.Encoding.UTF8.GetString(data, pos_start, len);
        }

        internal string ReadString(int position)
        {
            throw new NotImplementedException();
        }
    }
}
