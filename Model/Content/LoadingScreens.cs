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
    class LoadingScreens : ContentProvider
    {
        class LS
        {
            public string Name;
            public string InternalName;
            public VpkFile TextureFile;
            public Image Thumbnail;
        }

        const string ls_category = "loadingscreens";
        const string materialTypeExt = "vmt";
        const string installedScreenPath = "materials/console/dashboard_loading_dcu.vtf";
        const string vguiFileText = "\"UnlitGeneric\" { " +
            "\"$basetexture\"	\"console\\dashboard_loading_dcu\"\r\n" +
            "\"$translucent\" \"1\"\r\n" +
            "\"$ignorez\" \"1\"\r\n" +
            "\"$vertexcolor\" \"1\"\r\n" +
	        "\"$vertexalpha\" \"1\" }";
        List<LS> screens;
        Thumbnails cache;

        #region properties

        public override string ID
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

        public LoadingScreens(VpkArchive vpk, VdfNode schema, string dotaDataPath)
            : base(vpk, schema, dotaDataPath)
        {
            cache = new Thumbnails(ls_category); 
        }

        public static IContentProvider CreateInstance(VPK.VpkArchive vpk, VDF.VdfNode contentSchema, string contentFilesPath)
        {
            return new LoadingScreens(vpk, contentSchema, contentFilesPath);
        }
        
        void LoadLoadingScreens()
        {
            var lsSchema =  from item in schema["items"].ChildNodes
                              let prefab = item["prefab"]
                              where prefab == "loading_screen" /* equals: prefab.Val.Equal("...") */
                              //let created = item["creation_date"]
                              //orderby created != null ? DateTime.Parse(created) : DateTime.MinValue
                              //descending
                              /* Using vtf-files order is better, "creation_date" isn't always correct */
                          select item;
            var lsSchemaList = lsSchema.ToList();
            var vtfFiles = arc.FindFiles("materials/console/loadingscreens/", "vtf");
            this.screens = new List<LS>();
            var tasks = new List<Task<Image>>();

            for (int i = 0; i < vtfFiles.Length; i++)
            {
                // trying to get schema-info
                var path = vtfFiles[i].Path + '/' + vtfFiles[i].Name;
                // find schema's entry for current vtf (there can be >1 similar entries for 1 loadingscreen)
                VdfNode lsInfo = lsSchemaList.FirstOrDefault(entry => path.EndsWith(entry["visuals"][0]["asset"], StringComparison.OrdinalIgnoreCase));
                // presented in 'loadingscreens' directory but not in the schema
                if (lsInfo == null) continue; 

                var screen = new LS();
                screen.Name = lsInfo["name"].Val.Replace(" Loading Screen", string.Empty);
                string imgInv = lsInfo["image_inventory"];
                screen.InternalName = imgInv.Substring(imgInv.LastIndexOf('/') + 1);
                screen.TextureFile = vtfFiles[i];
                var task = LoadThumbnail(screen); /* can't use await here */

                tasks.Add(task);
                screens.Add(screen);
                // instant VPK exception 
                if (task.IsFaulted) break;         
            }
            // thumbnails building
            Task.WaitAll(tasks.ToArray());
            for (int i = 0; i < tasks.Count; i++)
                screens[i].Thumbnail = tasks[i].Result;
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
            if (thumb == null) {
                var vtfData = arc.ReadFile(screen.TextureFile);
                await Task<Image>.Run(() =>
                {
                    Image imgFull = VTF.Vtf.VtfToImage(vtfData);
                    thumb = new Bitmap(imgFull, thumbWidht, thumbHeight);

                    cache.StoreThumbnail(screen.InternalName, thumb);
                });              
            }
            return thumb;
        }

        public override IEnumerable<IGraphicContent> GetContentList()
        {
            if (screens == null || screens.Count == 0) LoadLoadingScreens();
            int id = 0;
            return from ls in screens select new GraphicContent { ID = id++, Name = ls.Name, Image = ls.Thumbnail };
        }

        public override Image GetInstalledContentImage()
        {
            byte[] vtfData;
            var filePath = Path.Combine(dotaDataPath, installedScreenPath);
            if (File.Exists(filePath)) {
                vtfData = File.ReadAllBytes(filePath);
            }
            else
                vtfData = arc.ReadFile(installedScreenPath);

            var img = VTF.Vtf.VtfToImage(vtfData);
            return img;
        }

        public override IGraphicContent InstallContent(int ID)
        {
            var filePath = Path.Combine(dotaDataPath, installedScreenPath);

            if (!File.Exists(filePath)) {
                var dir = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(dir);
            }

            // .vtf
            using (var fs = new FileStream(filePath, FileMode.Create)) {
                var newData = arc.ReadFile(screens[ID].TextureFile);
                fs.Write(newData, 0, newData.Length);
            }

            // .vmt
            filePath = Path.ChangeExtension(filePath, materialTypeExt);
            if (!File.Exists(filePath))
                using (var fs = new StreamWriter(filePath, false))
                    fs.Write(vguiFileText);

            return new GraphicContent() { ID = ID, Name = screens[ID].Name, Image = screens[ID].Thumbnail };
        }
    }
}
