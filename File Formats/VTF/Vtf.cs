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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Doto_Unlocker.VTF
{
    static class Vtf
    {
        static object sync = new object();

        // stuctures aren't actually used, Marshalling is too slow
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BITMAPFILEHEADER
        {
            public ushort bfType;
            public uint bfSize;
            public ushort bfReserved1;
            public ushort bfReserved2;
            public uint bfOffBits;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BITMAPINFOHEADER
        {
            public uint biSize;
            public uint biWidth;
            public uint biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public uint biXPelsPerMeter;
            public uint biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        unsafe private static readonly int bmpHeadersSize = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);
        unsafe private static readonly uint bmpInfoHeadersSize = (uint) sizeof(BITMAPINFOHEADER);

        static Vtf()
        {
            VTFLib.vlInitialize();
        }

        public static unsafe Image VtfToImage(byte[] vtf_data)
        {
            Monitor.Enter(sync); // vtflib can work only in a single thread

            uint uiImage;
            VTFLib.vlCreateImage(&uiImage);
            try
            {
                VTFLib.vlBindImage(uiImage);

                fixed (byte* lpBuffer = vtf_data)
                {
                    if (!VTFLib.vlImageLoadLump(lpBuffer, (uint)vtf_data.Length, false))
                        throw new ArgumentException(Marshal.PtrToStringAnsi(VTFLib.vlGetLastError()));
                }

                var width = VTFLib.vlImageGetWidth();
                var height = VTFLib.vlImageGetHeight();
                var image_len = width * height * 4;
                var fmt = VTFLib.vlImageGetFormat();

                byte[] lpImageData = new byte[image_len];
                byte[] lpBmpData = new byte[bmpHeadersSize + image_len];

                fixed (byte* lpOutput = lpImageData)
                {
                    // BGRA vs RGBA - slower 30-40% using this lib
                    if (!VTFLib.vlImageConvert(VTFLib.vlImageGetData(1, 0, 0, 0), lpOutput, width, height, fmt, VTFLib.ImageFormat.ImageFormatRGBA8888))
                    {
                        throw new ArgumentException(Marshal.PtrToStringAnsi(VTFLib.vlGetLastError()));
                    }

                    fixed (byte* lpBmp = lpBmpData)
                    {
                        // BMP Header
                        *(ushort*)lpBmp = 0x4D42;       // bfType
                        *(uint*)(lpBmp + 14) = bmpInfoHeadersSize;      // biSize (header size)
                        *(uint*)(lpBmp + 18) = width;   // biWidth
                        *(uint*)(lpBmp + 22) = height;  // biHeight
                        *(ushort*)(lpBmp + 28) = 32;    // biBitCount

                        // RGBA -> BGRA and FLIP-VERTICAL
                        uint* pDest = (uint*)(lpBmp + bmpHeadersSize);
                        uint len = width * height;
                        uint* pSrc = (uint*)lpOutput;

                        // fastest way to convert bits from RGBA to BGRA
                        for (uint
                            i = 0,
                            j = 0,
                            line = height - 1,
                            lineOffsetDst = width * line;
                            i < len; i++, j++)
                        {
                            if (j == width) // next line
                            {
                                j = 0;
                                // would be much faster if width would be always a power of two (lineOffsetDst = line << log2(width))
                                lineOffsetDst -= width;
                            }
                            uint val = pSrc[i];
                            pDest[lineOffsetDst + j] = (val >> 16 | (val << 16)) & 0x00ff00ff | (val & 0xff00ff00) /* G and A channels are unchanged */;
                        }
                    }
                }
                return Image.FromStream(new MemoryStream(lpBmpData), false, false);
            }
            finally
            {
                VTFLib.vlDeleteImage(uiImage);
                Monitor.Exit(sync);
            }
        }
    }
}
