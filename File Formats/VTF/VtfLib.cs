﻿/*
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker
{
    /// <summary>
    /// WARNING: x64 version of VTFLib is 30% slower than x86!
    /// </summary>
    public static class VTFLib
    {
        #region Enumerations
        public enum Option
        {
            OptionDXTQuality = 0,

            OptionLuminanceWeightR,
            OptionLuminanceWeightG,
            OptionLuminanceWeightB,

            OptionBlueScreenMaskR,
            OptionBlueScreenMaskG,
            OptionBlueScreenMaskB,

            OptionBlueScreenClearR,
            OptionBlueScreenClearG,
            OptionBlueScreenClearB,

            OptionFP16HDRKey,
            OptionFP16HDRShift,
            OptionFP16HDRGamma,

            OptionUnsharpenRadius,
            OptionUnsharpenAmount,
            OptionUnsharpenThreshold,

            OptionXSharpenStrength,
            OptionXSharpenThreshold,

            OptionVMTParseMode
        }

        public enum ImageFormat
        {
            ImageFormatRGBA8888 = 0,
            ImageFormatABGR8888,
            ImageFormatRGB888,
            ImageFormatBGR888,
            ImageFormatRGB565,
            ImageFormatI8,
            ImageFormatIA88,
            ImageFormatP8,
            ImageFormatA8,
            ImageFormatRGB888BlueScreen,
            ImageFormatBGR888BlueScreen,
            ImageFormatARGB8888,
            ImageFormatBGRA8888,
            ImageFormatDXT1,
            ImageFormatDXT3,
            ImageFormatDXT5,
            ImageFormatBGRX8888,
            ImageFormatBGR565,
            ImageFormatBGRX5551,
            ImageFormatBGRA4444,
            ImageFormatDXT1OneBitAlpha,
            ImageFormatBGRA5551,
            ImageFormatUV88,
            ImageFormatUVWQ8888,
            ImageFormatRGBA16161616F,
            ImageFormatRGBA16161616,
            ImageFormatUVLX8888,
            ImageFormatI32F,
            ImageFormatRGB323232F,
            ImageFormatRGBA32323232F,
            ImageFormatCount,
            ImageFormatNone = -1
        }

        public enum ImageFlag : uint
        {
            ImageFlagNone = 0x00000000,
            ImageFlagPointSample = 0x00000001,
            ImageFlagTrilinear = 0x00000002,
            ImageFlagClampS = 0x00000004,
            ImageFlagClampT = 0x00000008,
            ImageFlagAnisotropic = 0x00000010,
            ImageFlagHintDXT5 = 0x00000020,
            ImageFlagSRGB = 0x00000040,
            ImageFlagNormal = 0x00000080,
            ImageFlagNoMIP = 0x00000100,
            ImageFlagNoLOD = 0x00000200,
            ImageFlagMinMIP = 0x00000400,
            ImageFlagProcedural = 0x00000800,
            ImageFlagOneBitAlpha = 0x00001000,
            ImageFlagEightBitAlpha = 0x00002000,
            ImageFlagEnviromentMap = 0x00004000,
            ImageFlagRenderTarget = 0x00008000,
            ImageFlagDepthRenderTarget = 0x00010000,
            ImageFlagNoDebugOverride = 0x00020000,
            ImageFlagSingleCopy = 0x00040000,
            ImageFlagUnused0 = 0x00080000,
            ImageFlagUnused1 = 0x00100000,
            ImageFlagUnused2 = 0x00200000,
            ImageFlagUnused3 = 0x00400000,
            ImageFlagNoDepthBuffer = 0x00800000,
            ImageFlagUnused4 = 0x01000000,
            ImageFlagClampU = 0x02000000,
            ImageFlagVertexTexture = 0x04000000,
            ImageFlagSSBump = 0x08000000,
            ImageFlagUnused5 = 0x10000000,
            ImageFlagBorder = 0x20000000,
            ImageFlagCount = 30
        }

        public enum CubemapFace
        {
            CubemapFaceRight = 0,
            CubemapFaceLeft,
            CubemapFaceBack,
            CubemapFaceFront,
            CubemapFaceUp,
            CubemapFaceDown,
            CubemapFaceSphereMap,
            CubemapFaceCount
        }

        public enum MipmapFilter
        {
            MipmapFilterPoint = 0,
            MipmapFilterBox,
            MipmapFilterTriangle,
            MipmapFilterQuadratic,
            MipmapFilterCubic,
            MipmapFilterCatrom,
            MipmapFilterMitchell,
            MipmapFilterGaussian,
            MipmapFilterSinC,
            MipmapFilterBessel,
            MipmapFilterHanning,
            MipmapFilterHamming,
            MipmapFilterBlackman,
            MipmapFilterKaiser,
            MipmapFilterCount
        }

        public enum SharpenFilter
        {
            SharpenFilterNone = 0,
            SharpenFilterNegative,
            SharpenFilterLighter,
            SharpenFilterDarker,
            SharpenFilterContrastMore,
            SharpenFilterContrastLess,
            SharpenFilterSmoothen,
            SharpenFilterSharpenSoft,
            SharpenFilterSharpenMeium,
            SharpenFilterSharpenStrong,
            SharpenFilterFindEdges,
            SharpenFilterContour,
            SharpenFilterEdgeDetect,
            SharpenFilterEdgeDetectSoft,
            SharpenFilterEmboss,
            SharpenFilterMeanRemoval,
            SharpenFilterUnsharp,
            SharpenFilterXSharpen,
            SharpenFilterWarpSharp,
            SharpenFilterCount
        }

        public enum DXTQuality
        {
            DXTQualityLow = 0,
            DXTQualityMedium,
            DXTQualityHigh,
            DXTQualityHighest,
            DXTQualityCount
        }

        public enum KernelFilter
        {
            KernelFilter4x = 0,
            KernelFilter3x3,
            KernelFilter5x5,
            KernelFilter7x7,
            KernelFilter9x9,
            KernelFilterDuDv,
            KernelFilterCount
        }

        public enum HeightConversionMethod
        {
            HeightConversionMethodAlpha = 0,
            HeightConversionMethodAverageRGB,
            HeightConversionMethodBiasedRGB,
            HeightConversionMethodRed,
            HeightConversionMethodGreed,
            HeightConversionMethodBlue,
            HeightConversionMethodMaxRGB,
            HeightConversionMethodColorSspace,
            //HeightConversionMethodNormalize,
            HeightConversionMethodCount
        }

        public enum NormalAlphaResult
        {
            NormalAlphaResultNoChange = 0,
            NormalAlphaResultHeight,
            NormalAlphaResultBlack,
            NormalAlphaResultWhite,
            NormalAlphaResultCount
        }

        public enum ResizeMethod
        {
            ResizeMethodNearestPowerTwo = 0,
            ResizeMethodBiggestPowerTwo,
            ResizeMethodSmallestPowerTwo,
            ResizeMethodSet,
            ResizeMethodCount
        }

        public enum ResourceFlag : uint
        {
            ResourceFlagNoDataChunk = 0x02,
            ResourceFlagCount = 1
        }

        public enum ResourceType : uint
        {
            ResourceTypeLowResolutionImage = 0x01,
            ResourceTypeImage = 0x30,
            ResourceTypeSheet = 0x10,
            ResourceTypeCRC = 'C' | ('R' << 8) | ('C' << 24) | (ResourceFlag.ResourceFlagNoDataChunk << 32),
            ResourceTypeLODControl = 'L' | ('O' << 8) | ('D' << 24) | (ResourceFlag.ResourceFlagNoDataChunk << 32),
            ResourceTypeTextureSettingsEx = 'T' | ('S' << 8) | ('O' << 24) | (ResourceFlag.ResourceFlagNoDataChunk << 32),
            ResourceTypeKeyValueData = 'K' | ('V' << 8) | ('D' << 24)
        }

        public enum Proc
        {
            ProcReadClose = 0,
            ProcReadOpen,
            ProcReadRead,
            ProcReadSeek,
            ProcReadTell,
            ProcReadSize,
            ProcWriteClose,
            ProcWriteOpen,
            ProcWriteWrite,
            ProcWriteSeek,
            ProcWriteSize,
            ProcWriteTell
        }

        public enum SeekMode
        {
            Begin = 0,
            Current,
            End
        }
        #endregion

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlInitialize();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlShutdown();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlCreateImage(uint* uiImage);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlDeleteImage(uint uiImage);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlBindImage(uint uiImage);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageConvert(byte* lpSource, byte* lpDest, uint uiWidth, uint uiHeight, ImageFormat SourceFormat, ImageFormat DestFormat);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageLoadLump(void* lpData, uint uiBufferSize, [MarshalAs(UnmanagedType.U1)]bool bHeaderOnly);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetWidth();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetHeight();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ImageFormat vlImageGetFormat();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte* vlImageGetData(uint uiFrame, uint uiFace, uint uiSlice, uint uiMipmapLevel);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern IntPtr vlGetLastError();
        
    }
}
