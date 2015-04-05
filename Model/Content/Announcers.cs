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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Doto_Unlocker.VDF;
using Doto_Unlocker.VPK;

namespace Doto_Unlocker.Model
{
    abstract class Announcers : ContentProvider
    {
        const string iconsPathBase = "resource/flash3/images/econ/announcer";
        const string iconDefaultDummy = "resource/flash3/images/econ/bundles/speech_bundle01_large.png";
        const string talkerScriptsPath = "scripts/talker/response_rules_";
        const string voScriptsPath = "scripts/voscripts/game_sounds_vo_";
        const string prefix = "npc_dota_hero_";
        const string installedMark = "// installed:";
        const string scriptExt = ".txt";

        readonly string fullDefaultTalkerScriptPath;
        readonly string fullDefaultVoScriptPath;
        Announcer defaultAnnouncer;

        private List<Announcer> announcers;
        private VpkFile[] anncsTumbnails;

        protected abstract string defaultName { get; }
        protected abstract string defaultNameInternal { get; }
        protected abstract string itemsSlot { get; }

        public override abstract string ID
        {
            get;
        }

        protected Announcers(VpkArchive vpk, VdfNode schema, string dotaDataPath)
            : base(vpk, schema, dotaDataPath)
        {
            fullDefaultTalkerScriptPath = string.Format("{0}/{1}{2}{3}", this.dotaDataPath, talkerScriptsPath, defaultName, scriptExt);
            fullDefaultVoScriptPath = string.Format("{0}/{1}{2}{3}", this.dotaDataPath, voScriptsPath, defaultName, scriptExt);
        }

        public override IEnumerable<IGraphicContent> GetContentList()
        {
            if (announcers == null || announcers.Count == 0) InitializeList();
            int id = 0;
            return from a in announcers select new GraphicContent { ID = id++, Name = a.Name, Image = a.Thumbnail };
        }

        public override System.Drawing.Image GetInstalledContentImage()
        {
            byte[] iconData = null;

            if (File.Exists(fullDefaultTalkerScriptPath))
            {
                using (StreamReader reader = new StreamReader(fullDefaultTalkerScriptPath))
                {
                    string firststLine = reader.ReadLine();
                    if (firststLine.StartsWith(installedMark))
                    {
                        string anouncer = firststLine.Substring(installedMark.Length);
                        var ann = announcers.SingleOrDefault(a => a.NameInternal == anouncer);
                        if (ann != null && ann.IconLarge != null)
                        {
                            iconData = arc.ReadFile(ann.IconLarge);
                        }
                    }
                }
            }
            if (iconData == null) iconData = arc.ReadFile(iconDefaultDummy);

            Image img = Image.FromStream(new MemoryStream(iconData), false, false);
            return img;
        }

        public override IGraphicContent InstallContent(int ID)
        {
            Announcer ann = announcers[ID];
            string name = ann.NameInternal.Substring(prefix.Length);

            talkerScriptInstall(name, ann.NameInternal);
            voScriptInstall(name, ann.NameInternal);
            return new GraphicContent() { ID = ID, Name = ann.Name, Image = ann.Thumbnail };
        }

        /// <param name="name">without "npc_dota_hero.." prefix</param>
        /// <param name="fullname">full internal name</param>
        private void talkerScriptInstall(string name, string fullname)
        {
            // read
            byte[] talkerScript = arc.ReadFile(talkerScriptsPath + name + scriptExt);
            string talkerScriptTxt = Encoding.ASCII.GetString(talkerScript);

            // edit
            /* Example of talker script part:
             * criterion "IsAnnouncerVoice_Axe" "announcer_voice" "npc_dota_hero_announcer_dlc_Axe" weight 5 required
             * 
             * Rule announcer_dlc_axe_CustomIsAnnouncerVoice_AxeIsSelectHero_Rule
             * {
             *    criteria Custom Isannouncer_dlc_axe IsAnnouncerVoice_Axe IsSelectHero
             *    response announcer_dlc_axe_CustomIsAnnouncerVoice_AxeIsSelectHero
             * }
             */
            List<string> criterionsNames = new List<string>();

            // edit criterions and rename defenitions
            talkerScriptTxt = Regex.Replace(talkerScriptTxt, "criterion\\s+\"(\\S+)\"\\s+(\\S+)\\s+\"(\\S+)\"(.*)",
                m => CriterionsEdit(m, fullname, criterionsNames), RegexOptions.IgnoreCase);
            // rename all criterions in all rules
            talkerScriptTxt = Regex.Replace(talkerScriptTxt, @"criteria\s+(?:(\S+)[ ]*)+",
                m => CriterionsRename(m, criterionsNames), RegexOptions.IgnoreCase);
            // rename all rules
            talkerScriptTxt = Regex.Replace(talkerScriptTxt, @"rule\s+(\S+)", "Rule $1_2", RegexOptions.IgnoreCase);

            // save
            Utils.CheckAndCreateDirectory(fullDefaultTalkerScriptPath);
            using (StreamWriter writer = new StreamWriter(fullDefaultTalkerScriptPath, false))
            {
                writer.WriteLine(installedMark + fullname); // mark this file with comment
                writer.Write(talkerScriptTxt);
            }
        }

        /// <param name="name">without "npc_dota_hero.." prefix</param>
        /// <param name="fullname">full internal name</param>
        private void voScriptInstall(string name, string fullname)
        {
            byte[] voScript = arc.ReadFile(voScriptsPath + name + scriptExt);
            Utils.CheckAndCreateDirectory(fullDefaultVoScriptPath);
            File.WriteAllBytes(fullDefaultVoScriptPath, voScript);
        }

        /// <summary>
        /// 1. Rename "criteria" to "criteria_2"
        /// 2. Rename announcer name "npc_dota_hero_dlc_announcer_name" to "npc_dota_hero_announcer"
        /// 3. Set empty value for "announcer_voice" key in criteria
        /// </summary>
        /// <param name="match"></param>
        /// <param name="annName">current announcer internal name</param>
        /// <param name="criterions">OUT: list of criterions names</param>
        /// <returns></returns>
        string CriterionsEdit(Match match, string annName, List<string> criterions)
        {
            var criterionName = match.Groups[1].Value;
            criterions.Add(criterionName); // save

            var key = match.Groups[2].Value;
            var value = match.Groups[3].Value;
            if (key == "\"announcer_voice\"")
                value = string.Empty;
            else if (value.Equals(annName, StringComparison.OrdinalIgnoreCase))
                value = prefix + defaultName;
            return string.Format("criterion \"{0}_2\" {1} \"{2}\" {3}", criterionName, key, value, match.Groups[4].Value);
        }

        /// <summary>
        /// Rename criteria "IsDlcAnnouncer" to "IsDlcAnnouncer_2" if it in the 'criterions' list
        /// </summary>
        /// <param name="match"></param>
        /// <param name="criterions">IN: list of criterions names to rename</param>
        /// <returns></returns>
        string CriterionsRename(Match match, List<string> criterions)
        {
            StringBuilder criteria = new StringBuilder();
            criteria.Append("criteria");
            for (int i = 0; i < match.Groups[1].Captures.Count; i++) // 
            {
                var name = match.Groups[1].Captures[i].Value;
                criteria.Append(' ');
                criteria.Append(name);
                if (criterions.Contains(name)) // rename
                {
                    criteria.Append("_2");
                }
            }
            return criteria.ToString();
        }

        private Announcer CreateDefaultAnnouncer()
        {
            return new Announcer()
            {
                Name = "Default", 
                NameInternal = defaultNameInternal, 
                Thumbnail = Image.FromStream(new MemoryStream(arc.ReadFile(iconDefaultDummy)), false, false)
            };
        }

        private void InitializeList()
        {
            announcers = new List<Announcer>();
            var anncsInfo = from item in schema["items"].ChildNodes
                            let prefab = item["prefab"]
                            where prefab == "announcer"
                            let created = item["creation_date"]
                            orderby
                                item.Key descending, 
                                created != null ? DateTime.Parse(created) : DateTime.MinValue descending
                            select item;
            anncsTumbnails = arc.FindFiles(iconsPathBase, "png");

            foreach (var annInfo in anncsInfo)
            {
                if (annInfo["item_slot"] != itemsSlot) continue;

                var ann = new Announcer();
                ann.Name = annInfo["name"];
                ann.NameInternal = annInfo["visuals"][0]["asset"];

                string icon = annInfo["image_inventory"];
                string iconFilename = Path.GetFileName(icon);
                string iconLargeFilename = iconFilename + "_large";
                var iconFileinfo = anncsTumbnails.Single(f => FileNameComparer(f, iconFilename));
                var iconLargeFileinfo = anncsTumbnails.SingleOrDefault(f => FileNameComparer(f, iconLargeFilename));
                byte[] infoFileRaw = arc.ReadFile(iconFileinfo);
                ann.Thumbnail = Image.FromStream(new MemoryStream(infoFileRaw), false, false);
                ann.IconLarge = iconLargeFileinfo != null ? iconLargeFileinfo : iconFileinfo;
                announcers.Add(ann);
            }

            defaultAnnouncer = CreateDefaultAnnouncer();
            announcers.Add(defaultAnnouncer);
        }

        bool FileNameComparer(VpkFile file, string filename)
        {
            return file.Name.Equals(filename, StringComparison.OrdinalIgnoreCase);
        }

        class Announcer
        {
            public string Name;
            public string NameInternal;
            public Image Thumbnail;
            public VpkFile IconLarge;
        }
    }
}
