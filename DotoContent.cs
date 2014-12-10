using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.Model;
using Doto_Unlocker.VPK;
using MetroFramework;

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
        class AvailableProvider
        {
            public string Name { get; set;  }
            public ContentProviderFactory Ctor { get; set; }
        }

        const string schemaPath = "scripts/items/items_game.txt";

        MainForm view;
        Settings settings = Settings.Instance;
        Model.IContentProvider[] providers;
        readonly AvailableProvider[] availableProviders;

        public DotoContent(MainForm view) 
        {
            this.view = view;
            Settings.Instance.DotaPathChanged += Settings_DotaPathChanged;
            
            // content providers
            availableProviders = new AvailableProvider[] { 
                new AvailableProvider { Name = "Loading Screens", Ctor = Model.LoadingScreens.CreateInstance}, 
                new AvailableProvider { Name = "HUDs", Ctor = Model.Huds.CreateInstance}, 
                new AvailableProvider { Name = "Announcers", Ctor = Model.Announcers.CreateInstance}, 
                new AvailableProvider { Name = "Mega-kills", Ctor = Model.MegaKillsAnnouncers.CreateInstance} };
        }

        public List<string> ContentTypes()
        {
            return availableProviders.Select(p=>p.Name).ToList();
        }

        bool Initialized()
        {
            return providers != null;
        }

        void Settings_DotaPathChanged(PathEventArgs e)
        {
            UnInitialize(true);
        }

        void UnInitialize(bool reload = false)
        {
            providers = null;
            view.InvalidateContent(reload);
        }

        void Initialize()
        {
            bool failed = false;
            try
            {
                var arc = new VPK.VpkArchive(Settings.Instance.Dota2Path + @"\dota", "pak01");
                var schema = LoadSchema(arc);
                providers = new IContentProvider[availableProviders.Length];
                for (int i = 0; i < availableProviders.Length; i++)
                {
                    providers[i] = availableProviders[i].Ctor(arc, schema, Settings.Instance.Dota2Path);
                }

                // unlock
                var scenes = new Doto_Unlocker.Model.ScenesUnlocker(arc, schema, Settings.Instance.Dota2Path);
                scenes.UnlockAnnouncers();
            }
            catch (System.IO.FileNotFoundException)
            {
                view.ShowErr("Dota 2 file archive not found. Make sure game properly installed and check game cache integrity.");
                failed = true;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                view.ShowErr("Dota 2 content directory not found. Make sure game properly installed and check game cache integrity.");
                failed = true;
            }
            catch (InvalidVpkFileStructureException e)
            {
                view.ShowErr(string.Format("Invalid Dota 2 file archive structure. Fail type: {0}", e.FailType));
                failed = true;
            }
            finally
            {
                if (failed)
                {
                    UnInitialize();
                }
            }
        }

        public Task ContentRequest(int contentTypeIndex)
        {
            if (!Initialized())
            {
                if (Settings.Instance.SteamPath != null && Settings.Instance.Dota2Path != null)
                    Initialize();
                else
                    view.ShowErr("Current Steam Client and Dota 2 paths are invalid.\r\nPlease set correct paths in 'Settings' window.");
            }

            if (Initialized())
            {
                return LoadContentAsync(contentTypeIndex);
            }
            else
            {
                return Task.FromResult<object>(null);
            }
        }

        async Task LoadContentAsync(int providerId)
        {
            IEnumerable<Model.IGraphicContent> images = null;
            Model.IGraphicContent current = null;
            Image currentImg = null;

            try
            {
                await Task.Run(() =>
                {
                    images = providers[providerId].GetContentList();
                    currentImg = providers[providerId].GetInstalledContentImage();
                });
            }
            catch (Exception e)
            {    
                var inner = e is AggregateException? e.InnerException: e; // interested only in first exception
                Handler(inner);
            }

            if (currentImg != null)
            {
                string name;
                Settings.Instance.LastInstalled.TryGetValue(providers[providerId].ID, out name);
                current = new GraphicContent() { Name = name, Image = currentImg };
            }

            view.LoadContentCallback(providerId, images);
            view.CurrentContentCallback(providerId, current);
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
                        Settings.Instance.LastInstalled[providers[contentTypeIndex].ID] = cur.Name;
                        Settings.Instance.Save();
                        view.CurrentContentCallback(contentTypeIndex, cur);
                    }
                }
            }
            catch (System.IO.IOException ioEx)
            {
                view.ShowErr("System IO exception occured.\r\n\r\"" + ioEx.Message + "\"");
            }
            catch (Exception e)
            {
                Handler(e);
            }
        }

        void Handler(Exception ex)
        {
            if (ex is VpkFileIOException)
            {
                view.ShowErr("Access to vpk archive failed.\r\n\r\nFile name: \"" + ((VpkFileIOException)ex).VpkFileName + "\"");
            }
            else if (ex is VpkFileOutdatedException)
            {
                var outdated = (VpkFileOutdatedException)ex;
                view.ShowErr(string.Format("Vpk file had been updated after program has opened.\r\n\r\n" +
                    "File name: \"{0}\"\r\nVpk dir parsed: {1}\r\nFile updated: {2}",
                    outdated.VpkFileName, outdated.DirLoaded, outdated.LastUpdated));
            }
            else if (ex is ArgumentException)
            {
                view.ShowErr("Unknown format error occured.\r\n\r\"" + ((ArgumentException)ex).Message + "\"");
            }

            view.ShowInfo("\"Doto Content Unlocker\" restart is recommended");
            UnInitialize();
        }

        VDF.VdfNode LoadSchema(VpkArchive arc)
        {
            var raw = arc.ReadFile(schemaPath);
            var text = Encoding.ASCII.GetString(raw);
            return VDF.VdfParser.Parse(text);
        }
    }
}
