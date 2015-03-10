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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.VDF;
using Doto_Unlocker.VPK;

namespace Doto_Unlocker.Model
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

    class ScenesUnlocker
    {
        const uint VsifMagic = 0x46495356; // 'VSIF'
        const uint VsifVersion = 3;
        const string scenes_file = "scenes/scenes.image";
        const string default_killing_spree = "announcer_killing_spree";
        const string default_announcer = "announcer";
        const string prefix = "npc_dota_hero_";
        private VpkArchive arc;
        private VdfNode schema;
        private VpkEntry scenesFileEntry;
        private byte[] rawData;

        public ScenesUnlocker(VpkArchive vpk, VdfNode schema, string dotaPath)
        {
            arc = vpk;
            this.schema = schema;
            scenesFileEntry = arc.FindFile(scenes_file);
            rawData = arc.ReadFile(scenesFileEntry);
        }

        public void UnlockAnnouncers()
        {
            List<string> announcers = LoadAnnouncersList();
            FastBinaryReader reader = new FastBinaryReader(ref rawData);

            //header
            if (reader.ReadUInt32() != VsifMagic) throw new InvalidVsifFileStructureException() { FailType = VSIFFail.MagicFail };
            if (reader.ReadUInt32() != VsifVersion) throw new InvalidVsifFileStructureException() { FailType = VSIFFail.UnsupportedVer };
            reader.Pos += sizeof(UInt32); // ScenesCount
            var stringsCount = reader.ReadUInt32();
            reader.Pos += sizeof(UInt32); // SceneOffset

            // strings pool
            for (int i = 0; i < stringsCount; i++)
            {
                var stringOffest = reader.ReadUInt32();
                var saved = reader.Pos;
                reader.Pos = (int)stringOffest;
                string str = reader.ReadSring();

                if (announcers.Contains(str))
                {
                    if (str.EndsWith("killing_spree"))
                    {
                        System.Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes(default_killing_spree), 0, rawData, (int)stringOffest, default_killing_spree.Length);
                        rawData[stringOffest + default_killing_spree.Length] = 0;
                    }
                    else
                    {
                        rawData[stringOffest + default_announcer.Length] = 0;
                    }
                }
                reader.Pos = saved;
            }
            SaveData();
        }

        private List<string> LoadAnnouncersList()
        {
            var anncsInfo = from item in schema["items"].ChildNodes
                            let prefab = item["prefab"]
                            where prefab != null && prefab == "announcer"
                            select item["visuals"][0]["asset"].Val.Substring(prefix.Length);
            return anncsInfo.ToList();
        }

        private void SaveData()
        {
            arc.EditFile(scenesFileEntry, ref rawData, true);
        }
    }
}
