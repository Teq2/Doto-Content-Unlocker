using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.VPK;
using Doto_Unlocker.VDF;
using Doto_Unlocker.Properties;

namespace Doto_Unlocker.Model
{
    class Huds : IContentProvider
    {
        private class Hud
        {
            public string Name; // to upper layers
            public string NameInternal;
            public string Style;
            public List<VpkEntry> Files; // all files for current style
            public Image Thumbnail;
        }

        const string hud_category = "huds";
        const string defaultHudName = "default";
        const string basePath = "resource/flash3/images/";
        const string hudsPath = basePath + "hud_skins/";
        const string iconsPath = basePath + "econ/huds";
        readonly string dotaPath;

        private VpkArchive arc;
        VdfNode schema;
        private VpkEntry[] hudsFiles;
        private VpkEntry[] hudsIcons;
        private VdfNode dota_res;
        private List<Hud> huds;
        private Hud defaultHud;

        public string ID 
        {
            get
            {
                return hud_category;
            }
        }

        private Huds(VpkArchive vpk, VdfNode schema, string dotaPath)
        {
            arc = vpk;
            this.schema = schema;
            this.dotaPath = dotaPath + "/dota/";
        }

        public static IContentProvider CreateInstance(VPK.VpkArchive vpk, VDF.VdfNode contentSchema, string contentFilesPath)
        {
            return new Huds(vpk, contentSchema, contentFilesPath);
        }

        public IEnumerable<IGraphicContent> GetContentList()
        {
            if (huds == null || huds.Count == 0) Initialize();

            int id = 0;
            return from h in huds select new GraphicContent { ID = id++, Name = h.Name, Image = h.Thumbnail };
        }

        private void Initialize()
        {
            huds = new List<Hud>();
            var hudsInfo = from item in schema["items"].ChildNodes
                           let
                                prefab = item["prefab"]
                           where
                                prefab != null && prefab == "hud_skin"
                           let
                                created = item["creation_date"]
                           orderby
                                created != null ? DateTime.Parse(created) : DateTime.MinValue
                           descending
                           select item;
            hudsFiles = arc.FindFiles(hudsPath, "png");
            hudsIcons = arc.FindFiles(iconsPath, "png");
            dota_res = LoadStringResources()["Tokens"];

            foreach (var hudInfo in hudsInfo) // 300 +
            {
                var visuals = hudInfo["visuals"];
                var styles = visuals["styles"];
                if (styles == null)
                {
                    var hud = HudCreate(hudInfo, visuals, null);
                    huds.Add(hud);
                }
                else
                {
                    foreach (var style in styles.ChildNodes)
                    {
                        var hud = HudCreate(hudInfo, visuals, style);
                        huds.Add(hud);
                    }
                }
            }

            defaultHud = LoadDefaultHud();
            huds.Add(defaultHud);
        }

        private Hud LoadDefaultHud()
        {
            string hudRelPath = hudsPath + defaultHudName;

            Hud hud = new Hud();
            hud.Name = char.ToUpper(defaultHudName[0]) + defaultHudName.Substring(1);
            hud.NameInternal = defaultHudName;
            hud.Files = hudsFiles.Where((file) => file.Path.StartsWith(hudRelPath, StringComparison.OrdinalIgnoreCase)).ToList();
            hud.Thumbnail = Resources.hud_default_icon;
            return hud;
        }

        private Hud HudCreate(VDF.VdfNode hudInfo, VDF.VdfNode visuals, VDF.VdfNode style)
        {
            Hud hud = new Hud();
            hud.Name = hudInfo["name"];
            hud.NameInternal = visuals[0]["asset"];

            // thumbnail          
            string icon = hudInfo["image_inventory"];
            var iconFileInfo = hudsIcons.Single((file) => file.Name.Equals(Path.GetFileName(icon), StringComparison.OrdinalIgnoreCase));
            var infoFileRaw = arc.ReadFile(iconFileInfo);
            hud.Thumbnail = Image.FromStream(new MemoryStream(infoFileRaw), false, false);

            // files
            string filesPath = hudsPath + hud.NameInternal;
            var hudFilesSet = hudsFiles.Where((file_entry) => file_entry.Path.StartsWith(filesPath, StringComparison.OrdinalIgnoreCase));
            hud.Files = hudFilesSet.ToList();

            // apply style
            if (style != null)
            {
                // change name
                var token = style["name"].Val.Substring(1); // skip '#' symbol
                hud.Name += ": " + dota_res[token]; // string from dota-resources
                
                if (style.Key != "0") // "0" is "default" style
                {
                    hud.Style = "style" + style.Key;
                }
            }
            return hud;
        }

        public Image GetContentImage(int ID)
        {
            //if (huds == null || huds.Count == 0) Initialize();

            Hud hud = huds[ID];
            HudConstructor hudCtor = new HudConstructor(new ArchivedHudReader(arc, defaultHud.Files, hud.Files, hud.Style));
            return hudCtor.Construct();
        }

        public Image GetInstalledContentImage()
        {
            if (huds == null || huds.Count == 0) Initialize();

            string hudRelPath = hudsPath + defaultHudName + "/";
            HudConstructor hud = new HudConstructor(new DefaultHudReader(arc, defaultHud.Files, dotaPath, hudRelPath));
            return hud.Construct();
        }

        public IGraphicContent InstallContent(int ID)
        {
            Hud hud = huds[ID];
            var hudFiles = hud.Files;

            if (hud.Style != null)
            {
                var styledFiles = from file in hudFiles
                                  where file.Path.EndsWith(hud.Style, StringComparison.OrdinalIgnoreCase)
                                  select file;
                hudFiles = styledFiles.Union(hudFiles, new VpkEntryNameComparer()).ToList();
            }

            string hudDir = dotaPath + hudsPath + defaultHudName + "/";
            // remove all files from folder, without deleting folders
            if (Directory.Exists(hudDir)) Utils.ClearDir(hudDir);

            foreach (var file in hudFiles)
            {
                var last = file.Path.LastIndexOf('/');
                string subDir = file.Path.Substring(last + 1);
                
                if (subDir.StartsWith("style", StringComparison.OrdinalIgnoreCase))
                {
                    if (subDir.Equals(hud.Style, StringComparison.OrdinalIgnoreCase)) // it's current style
                    {
                        // step one level higher
                        var prev = file.Path.LastIndexOf('/', last - 1) + 1;
                        subDir = file.Path.Substring(prev, last - prev);
                    }
                    else continue; // other style (current is default)
                }
                if (subDir.Equals(hud.NameInternal, StringComparison.OrdinalIgnoreCase)) continue; // skip root files 

                // dir
                string fileDir = hudDir + subDir;
                if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                // file
                string filePath = fileDir + '/' + file.Name + '.' +  file.Ext;
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    var newData = arc.ReadFile(file);
                    fs.Write(newData, 0, newData.Length);
                }
            }

            return new GraphicContent() { ID = ID, Name = hud.Name, Image = hud.Thumbnail };
        }

        VDF.VdfNode LoadStringResources()
        {
            var text = File.ReadAllText(dotaPath + "resource\\dota_english.txt");
            return VDF.VdfParser.Parse(text);
        }
    }
}
