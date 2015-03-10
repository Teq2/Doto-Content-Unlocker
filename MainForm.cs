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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Doto_Unlocker.Controls;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Drawing;
using MetroFramework.Forms;

namespace Doto_Unlocker
{
    internal partial class MainForm : MetroForm
    {
        const double aspect = 1.75;
        const MetroColorStyle defaultStyle = MetroColorStyle.Green;

        Random rnd = new Random();
        DotoContent controller; // 'presenter' in fact
        SplashScreen ss;
        Containers<ScrollableFlowContainer> containers = new Containers<ScrollableFlowContainer>();

        public MainForm()
        {
            InitializeComponent();
            controller = new DotoContent(this);
            createContainers();
            scaleContentPanel();

            #region style
            if (Settings.Instance.Style == MetroColorStyle.Default)
            {
                Settings.Instance.Style = defaultStyle;
            }
            StyleManager.Style = Settings.Instance.Style;
            Settings.Instance.StyleChanged += (s) => StyleManager.Style = s.NewStyle;
            #endregion
        }

        public void ShowErr(string msg)
        {
            MetroMessageBox.Show(this, "\r\n"+msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowInfo(string msg)
        {
            MetroMessageBox.Show(this, "\r\n" + msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void InvalidateContent(bool forceReload = false)
        {
            // clear containers
            foreach (var container in containers) container.Clear();
            CurrentContentCallback(0, null);

            if (forceReload)
            {
                contentRequestAsync(containers.ActiveIndex);
            }
            else
            {
                // display button for manually reload
                var reloadBtn = new ButtonOverlay();
                {
                    EventHandler<ContainerSwitchEventArgs> handler = null;
                    var containerId = containers.ActiveIndex;
                    reloadBtn.StyleManager.Theme = this.StyleManager.Theme;
                    reloadBtn.StyleManager.Style = this.StyleManager.Style;
                    reloadBtn.ButtonText = "Reload Content";
                    reloadBtn.ButtonClick += (s, e) =>
                        {
                            // remove button -> request content
                            reloadBtn.Dispose();
                            contentRequestAsync(containerId);
                        };
                    handler = (s, e) =>
                        {
                            // tab changed. -> remove button
                            reloadBtn.Dispose();
                            containers.SwitchHandler -= handler;
                        };
                    containers.SwitchHandler += handler;
                    // install into cur. container
                    containers.Current.Controls.Add(reloadBtn);
                    reloadBtn.BringToFront();
                }
            }
        }

        private async void contentRequestAsync(int idx)
        {
            bool needSplash = containers[idx].Count == 0;

            if (needSplash) SplashShow();
            await controller.ContentRequest(idx);
            if (needSplash) SplashHide();
        }

        public void LoadContentCallback(int contentId, IEnumerable<Model.IGraphicContent> content)
        {
            var container = containers[contentId];
            if (container.Count == 0 && content != null) // container is empty, content isn't
            {
                int imgWidth = 0, imgHeight = 0;

                foreach (var item in content)
                {
                    // make all tiles same size
                    if (imgWidth == 0) imgWidth = item.Image.Width;
                    if (imgHeight == 0) imgHeight = item.Image.Height;

                    var tile = new ContentTile();
                    tile.TitleDock = TitleDocking.Bottom;
                    tile.TitleText = item.Name;
                    tile.StyleManager = metroStyleManager1;
                    tile.Width = imgWidth;
                    tile.Height = imgHeight + tile.TitleLineHeight;
                    tile.TileImage = item.Image;
                    tile.Tag = item.ID; // for randomizing
                    tile.Click += (sender, e) => getCurrentContentAsync((Control)sender, contentId, item.ID);
                    container.Add(tile);
                }
            }
        }

        public void CurrentContentCallback(int contentId, Model.IGraphicContent current)
        {
            if (current != null)
            {
                currentContent.Image = current.Image;
                contentName.Text = current.Name;
            }
            else
            {
                currentContent.Image = null;
                contentName.Text = string.Empty;
            }
        }

        private async void getCurrentContentAsync(Control ctrl, int contentTypeID, int contentID)
        {
            using (var spin = new ProgressOverlay())
            {
                spin.StyleManager.Theme = this.StyleManager.Theme;
                spin.StyleManager.Style = this.StyleManager.Style;
                // display transparent spinner while content is installing
                ctrl.Controls.Add(spin);
                ctrl.Enabled = false;
                await controller.SetCurrent(contentTypeID, contentID);
                ctrl.Enabled = true;
            }
        }

        private void createContainers()
        {
            // clear
            this.tabControl.SelectedIndex = -1;
            this.tabControl.TabPages.Clear();

            var types = controller.ContentTypes();
            foreach (var type in types)
            {
                registerContentType(type);
            }
        }

        /// <summary>
        /// Adds new page in tab-control and new container for content
        /// </summary>
        private void registerContentType(string contentTypeName)
        {
            var page = new TabPage(contentTypeName);
            var container = new ScrollableFlowContainer();

            // apply style
            container.StyleManager = this.metroStyleManager1;
            foreach (var ctrl in container.Controls)
            {
                var metroCtrl = ctrl as MetroFramework.Interfaces.IMetroControl;
                if (metroCtrl != null) metroCtrl.StyleManager = this.metroStyleManager1;
            }
            container.BackColor = MetroPaint.BackColor.Form(Theme);
            container.Dock = System.Windows.Forms.DockStyle.Fill;
            container.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);

            containers.Add(container);
            page.Controls.Add(container);
            this.tabControl.TabPages.Add(page);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!(SteamPaths.CheckSteamPath(Settings.Instance.SteamPath) && SteamPaths.CheckDota2Path(Settings.Instance.Dota2Path)))
            {
                // force set paths
                ShowInfo("Current Steam Client and Dota 2 paths are invalid and can't be detected automatically.\r\n" +
                "Please select correct Steam Client folder in next window.");
                openSettingsWindow(true);
            }
            else
            {
                contentRequestAsync(containers.ActiveIndex);
            }
        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idx = tabControl.SelectedIndex;
            if (idx >= 0)
            {
                containers.Switch(idx);
                contentRequestAsync(idx);
            }
        }

        private void openSettingsWindow(bool forceBrowse = false)
        {
            var form = new SettingsForm();
            form.metroStyleManager1.Theme = this.metroStyleManager1.Theme;
            form.metroStyleManager1.Style = this.metroStyleManager1.Style;
            form.StyleManager = form.metroStyleManager1;
            form.ForceBrowse = forceBrowse;
            form.ShowDialog(this);
            form.Dispose(); // must be called after 'ShowDialog'
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            openSettingsWindow();
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            var container = containers.Current;
            if (container != null)
            {
                var max = container.Count;
                if (max > 0)
                    getCurrentContentAsync(currentContent, containers.ActiveIndex, (int)container[rnd.Next(max)].Tag);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                // alternate way: steam://run/<appid>//?param1=value1;param2=value2;param3=value3
                if (Process.Start(Settings.Instance.SteamPath + "/steam.exe", "-applaunch 570 -vpk_override") != null) Close();
            }
            catch (Win32Exception ex)
            {
                ShowErr("System returned an error:\r\n\"" + ex.Message + "\"");
            }
        }

        #region splash_window
        void SplashShow()
        {
            ss = new SplashScreen();

            ss.StyleManager.Theme = this.StyleManager.Theme;
            ss.StyleManager.Style = this.StyleManager.Style;
            ss.Show(this);
        }

        void SplashHide()
        {
            if (ss != null)
            {
                ss.Close();
                ss = null;
            }
        }
        #endregion

        #region resizing
        FormWindowState lastState;
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowState == FormWindowState.Maximized && WindowState != lastState)
            {
                var container = containers.Current;
                if (container != null)
                    this.rightPanel.Width += container.Width - container.EffectiveWidth;
            }
            else
            {
                this.rightPanel.Width = (int)((this.Width - this.Padding.Horizontal) * 0.35);
            }
            lastState = WindowState;
        }

        private void scaleContentPanel()
        {
            this.panel1.Height = (int)(currentContent.Width / aspect) + contentName.Height;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            scaleContentPanel();
        }
        #endregion

        class ContainerSwitchEventArgs : EventArgs
        {
            public int PrevIndex { get; set; }
            public int NewIndex { get; set; }
        }
        class Containers<T>
        {
            List<T> containers = new List<T>();
            int activeContainer = 0;

            public int ActiveIndex
            {
                get { return activeContainer; }
            }

            public T Current
            {
                get { return containers[activeContainer]; }
            }

            public void Switch(int newIdx)
            {
                int old = activeContainer;
                activeContainer = newIdx;
                if (SwitchHandler != null) SwitchHandler(null, new ContainerSwitchEventArgs() { PrevIndex = old, NewIndex = newIdx });
            }

            public event EventHandler<ContainerSwitchEventArgs> SwitchHandler;

            public void Add(T c)
            {
                containers.Add(c);
            }

            public int Count
            {
                get { return containers.Count; }
            }

            public T this[int idx]
            {
                get { return containers[idx]; }
            }

            public List<T>.Enumerator GetEnumerator()
            {
                return containers.GetEnumerator();
            }
        }
    }
}