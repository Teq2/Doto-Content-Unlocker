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
    class LoadingScreens : IContentProvider
    {
        class LS
        {
            public string Name;
            public string InternalName;
            public VpkEntry TextureFile;
            public Image Thumbnail;
        }

        const string ls_category = "loadingscreens";
        const string defaultScreenPath = "materials/console/dashboard_loading_embers.vtf";
        readonly string dotaPath;

        VpkArchive arc;
        VdfNode schema;
        List<LS> screens;
        Thumbnails cache;

        #region properties

        public string ID
        {
            get
            {
                return ls_category;
            }
        }

        int thumbWidht = 256;
        public int ThumbWidth
        {
            get
            {
                return thumbWidht;
            }
            set
            {
                thumbWidht = value;
            }
        }

        int thumbHeight = 170;
        public int ThumbHeight
        {
            get
            {
                return thumbHeight;
            }
            set
            {
                thumbHeight = value;
            }
        }

        public int Count
        {
            get 
            {
                return screens.Count; 
            }
        }

        #endregion

        private LoadingScreens(VpkArchive vpk, VdfNode schema, string dotaPath)
        {
            arc = vpk;
            this.schema = schema;
            this.dotaPath = dotaPath + "/dota/";
            cache = new Thumbnails(ls_category); 
        }

        public static IContentProvider CreateInstance(VPK.VpkArchive vpk, VDF.VdfNode contentSchema, string contentFilesPath)
        {
            return new LoadingScreens(vpk, contentSchema, contentFilesPath);
        }
        
        void LoadLoadingScreens()
        {
            var lssInfo =  from item in schema["items"].ChildNodes
                              let prefab = item["prefab"]
                              where prefab != null && prefab == "loading_screen"
                              //let created = item["creation_date"]
                              //orderby created != null ? DateTime.Parse(created) : DateTime.MinValue
                              //descending
                              // {USING VTF-ORDER INSTEAD}
                          select item;
            var lssInfoList = lssInfo.ToList();
            var vtfFiles = arc.FindFiles("materials/console/loadingscreens/", "vtf");
            screens = new List<LS>();
            var tasks = new List<Task<Image>>();

            //foreach (var lsInfo in lssInfo)
            for (int i = 0; i < vtfFiles.Length; i++)
            {
                // trying to get schema-info
                var path = vtfFiles[i].FullPath;
                var lsInfo = lssInfoList.FirstOrDefault(entry => path.EndsWith(entry["visuals"][0]["asset"] + ".vtf", StringComparison.OrdinalIgnoreCase));
                if (lsInfo == null) continue; // present in 'loadingscreens' folder but not in schema
                var screen = new LS();
                screen.Name = lsInfo["name"].Val.Replace(" Loading Screen", "");
                string imgInv = lsInfo["image_inventory"];
                screen.InternalName = imgInv.Substring(imgInv.LastIndexOf('/') + 1);
                screen.TextureFile = vtfFiles[i];
                //string path = lsInfo["visuals"][0]["asset"] + ".vtf";
                //screen.TextureFile = vtfFiles.Single((file_entry) => file_entry.FullPath.EndsWith(path, StringComparison.OrdinalIgnoreCase));
                var task = LoadThumbnail(screen); // can't use await here

                tasks.Add(task);
                screens.Add(screen);
                if (task.IsFaulted) break;// instant VPK exception         
            }
            Task.WaitAll(tasks.ToArray());
            for (int i = 0; i < tasks.Count; i++)
            {
                screens[i].Thumbnail = tasks[i].Result;
            }
        }

        /* Timeline:
         * 
         *  [READ1][READ2][READ3]..[READn]
         *         [CNV1] [CNV2]...[CNV-n1][CNVn]
         * 
         */
        async Task<Image> LoadThumbnail(LS screen)
        {
            var thumb = cache.GetThumbnail(screen.InternalName);
            //Image thumb = null;
            if (thumb != null)
            {
                return thumb;
            }
            else // create new thumbnail
            {
                var vtfData = arc.ReadFile(screen.TextureFile);

                await Task<Image>.Run(() =>
                {
                    Image imgFull = null;
                    imgFull = VTF.Vtf.VtfToImage(vtfData);
                    thumb = new Bitmap(imgFull, thumbWidht, thumbHeight);

                    cache.StoreThumbnail(screen.InternalName, thumb);
                });
                return thumb;                
            }     
        }

        public IEnumerable<IGraphicContent> GetContentList()
        {
            if (screens == null || screens.Count == 0) LoadLoadingScreens();

            int id = 0;
            return from ls in screens select new GraphicContent { ID = id++, Name = ls.Name, Image = ls.Thumbnail };
        }

        public Image GetInstalledContentImage()
        {
            byte[] vtfData;
            if (File.Exists(dotaPath + defaultScreenPath))
            {
                vtfData = File.ReadAllBytes(dotaPath + defaultScreenPath);
            }
            else
            {
                vtfData = arc.ReadFile(defaultScreenPath);
            }
            var img = VTF.Vtf.VtfToImage(vtfData);

            return img;
        }

        public IGraphicContent InstallContent(int ID)
        {
            var filePath = dotaPath + defaultScreenPath;

            if (!File.Exists(filePath))
            {
                var dir = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(dir);
            }

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                var newData = arc.ReadFile(screens[ID].TextureFile);
                fs.Write(newData, 0, newData.Length);
            }

            return new GraphicContent() { ID = ID, Name = screens[ID].Name, Image = screens[ID].Thumbnail };
        }
    }
}
