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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.Model;
using Doto_Unlocker.VPK;
using MetroFramework;
using System.IO;

namespace Doto_Unlocker
{
    //public class InvalidPathException : Exception, ISerializable
    //{
    //    public InvalidPathException() : base() { }
    //    public InvalidPathException(string message) : base() { }
    //    public InvalidPathException(string message, Exception inner) : base() { }
    //    protected InvalidPathException(SerializationInfo info, StreamingContext context) : base() { }
    //}

    class DotoContent
    {
        const string dataSubDir = "dota";
        const string archiveName = "pak01";
        const string schemaPath = "scripts/items/items_game.txt";

        MainForm view;
        Model.IContentProvider[] providers;
        readonly AvailableProvider[] availableProviders;

        public DotoContent(MainForm view) 
        { 
            this.view = view;
            Settings.DotaPathChanged += Settings_DotaPathChanged;
            
            // content providers
            availableProviders = new AvailableProvider[] { 
                new AvailableProvider { Name = "Loading Screens", CreateInstance = Model.LoadingScreens.CreateInstance}, 
                new AvailableProvider { Name = "HUDs", CreateInstance = Model.Huds.CreateInstance}, 
                new AvailableProvider { Name = "Announcers", CreateInstance = Model.GameAnnouncers.CreateInstance}, 
                new AvailableProvider { Name = "Mega-kills", CreateInstance = Model.MegaKillsAnnouncers.CreateInstance} };
        }

        public IEnumerable<string> ContentTypes()
        {
            return availableProviders.Select(p => p.Name);
        }

        public async Task ContentRequest(int contentTypeIndex)
        {
            if (!Initialized()) Initialize();
            try {
                if (Initialized())
                     await LoadContentAsync(contentTypeIndex);
            }
            catch (Exception e) {
                var inner = e is AggregateException ? e.InnerException : e; // interested only in first exception, Wait() used only in LoadingScreens.LoadLoadingScreens()
                CommonHandler(inner);
            }
        }

        public async Task SetCurrent(int contentTypeIndex, int contentIndex)
        {
            try
            {
                var cur = await Task<IGraphicContent>.Run(() => providers[contentTypeIndex].InstallContent(contentIndex));
                if (cur != null)
                {
                    var imgFull = await Task<Image>.Run(() => providers[contentTypeIndex].GetInstalledContentImage());
                    if (imgFull != null)
                    {
                        cur = new GraphicContent { Name = cur.Name, Image = imgFull };
                        Settings.LastInstalled[providers[contentTypeIndex].ID] = cur.Name;
                        Settings.Save();
                        view.CurrentContentCallback(contentTypeIndex, cur);
                    }
                }
            }
            catch (Exception e)
            {
                CommonHandler(e);
            }
        }

        private async Task LoadContentAsync(int providerId)
        {
            IEnumerable<Model.IGraphicContent> images = null;
            Model.IGraphicContent current = null;
            Image currentImg = null;

            await Task.Run(() =>
            {
                images = providers[providerId].GetContentList();
                currentImg = providers[providerId].GetInstalledContentImage();
            });

            if (currentImg != null) {
                string name;
                Settings.LastInstalled.TryGetValue(providers[providerId].ID, out name);
                current = new GraphicContent() { Name = name, Image = currentImg };
            }

            view.LoadContentCallback(providerId, images);
            view.CurrentContentCallback(providerId, current);
        }

        private bool Initialize()
        {
            bool success = false;
            try
            {
                if (Settings.SteamPath != null && Settings.Dota2Path != null) /* they can't be empty, only null */
                {
                    var dotaDataDir = Path.Combine(Settings.Dota2Path, dataSubDir);
                    var arc = new VPK.VpkArchive(dotaDataDir, archiveName);
                    var schema = LoadSchema(arc);

                    providers = new IContentProvider[availableProviders.Length];
                    for (int i = 0; i < availableProviders.Length; i++)
                        providers[i] = availableProviders[i].CreateInstance(arc, schema, dotaDataDir);

                    success = true;
                }
                else
                    ShowErr("Current Steam Client and Dota 2 paths are invalid.\r\nPlease set correct paths in 'Settings' window.");
            }
            catch (System.IO.FileNotFoundException)
            {
                ShowErr("Dota 2 file archive not found. Make sure game properly installed and check game cache integrity.");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                ShowErr("Dota 2 content directory not found. Make sure game properly installed and check game cache integrity.");
            }
            catch (VpkFileIOException e)
            {
                ShowErr("Access to vpk archive failed", "File name: \"" + ((VpkFileIOException)e).VpkFileName +
                    "\"\r\nMake sure Dota 2 client isn't running, if it, close game client and try again.", false);
            }
            catch (InvalidVpkFileStructureException e)
            {
                ShowErr(string.Format("Invalid Dota 2 file archive structure. Fail type: {0}", e.FailType));
            }
            finally
            {
                if (!success)
                {
                    UnInitialize();
                }
            }
            return success;
        }

        private bool Initialized()
        {
            return providers != null;
        }

        private void Settings_DotaPathChanged(PathEventArgs e)
        {
            UnInitialize(true);
        }

        private void UnInitialize(bool reload = false)
        {
            providers = null;
            view.InvalidateContent(reload);
        }

        private void CommonHandler(Exception ex)
        {
            if (ex is System.IO.IOException)
                ShowErr("System IO error:", ex.Message);

            else if (ex is UnauthorizedAccessException)
                ShowErr("System security error:", ex.Message);

            else if (ex is VpkFileIOException)
                ShowErr("Access to vpk archive failed.", "File name: \"" + ((VpkFileIOException)ex).VpkFileName + "\"", false);

            else if (ex is VpkFileOutdatedException)
            {
                var outdated = (VpkFileOutdatedException)ex;
                ShowErr("Vpk file had been updated after program has opened.", string.Format("File name: \"{0}\"\r\nVpk dir parsed: {1}\r\nFile updated: {2}",
                    outdated.VpkFileName, outdated.DirLoaded, outdated.LastUpdated), false);
            }
            else if (ex is ArgumentException)
                ShowErr("Unknown format error:", ex.Message);

            else
                ShowErr("Unexpected error:", string.Format("Message: {0}\r\nStacktrace:{1}", ex.Message, ex.StackTrace));

            view.ShowInfo("\"Doto Content Unlocker\" restart is recommended");
            UnInitialize();
        }

        private void ShowErr(string err, string desc=null, bool quotted=true)
        {
            if (string.IsNullOrEmpty(desc))
                view.ShowErr(err);
            else
            {
                string quote = quotted ? "\"" : string.Empty;
                view.ShowErr(string.Format("{0}\r\n{2}{1}{2}", err, desc, quote));
            }
        }

        private VDF.VdfNode LoadSchema(VpkArchive arc)
        {
            byte[] rawData = arc.ReadFile(schemaPath);
            if (rawData != null)
            {
                var text = Encoding.ASCII.GetString(rawData);
                return VDF.VdfParser.Parse(text);
            }
            else
                return null;
        }

        private delegate IContentProvider ContentProviderFactory(VPK.VpkArchive vpk, VDF.VdfNode contentSchema, string contentFilesPath);

        private class AvailableProvider
        {
            public string Name;
            public ContentProviderFactory CreateInstance;
        }
    }
}
