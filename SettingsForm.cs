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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace Doto_Unlocker
{
    public partial class SettingsForm : MetroForm
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        public bool ForceBrowse
        {
            get; set;
        }

        private void tileStyle_Click(object sender, EventArgs e)
        {
            metroStyleManager1.Style = (MetroColorStyle) Enum.Parse(typeof(MetroColorStyle), (string)boxStyles.Items[boxStyles.SelectedIndex]);
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (SteamPaths.CheckSteamPath(txtSteamPath.Text))
            {
                if (SteamPaths.CheckDota2Path(txtDotoPath.Text))
                {
                    Settings.SteamPath = txtSteamPath.Text;
                    Settings.Dota2Path = txtDotoPath.Text;
                    Settings.Style = StyleManager.Style;
                    try
                    {
                        Settings.Save();
                    }
                    catch (Exception ex)
                    {
                        if (ex is System.IO.IOException || ex is UnauthorizedAccessException)
                            MetroMessageBox.Show(this, string.Format("\r\nAn error occurred while saving settings: \r\n\"{0}\"", ex.Message),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Close();
                }
                else
                    MetroMessageBox.Show(this, "\r\nDota 2 not found in steam-library,\r\n make sure game properly installed",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            MetroMessageBox.Show(this, "\r\nSteam client not found\r\nPlease select another directory.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            txtSteamPath.Text = Settings.SteamPath;
            txtDotoPath.Text = Settings.Dota2Path;

            var styles = Enum.GetNames(typeof(MetroColorStyle));
            for (int i = 1; i < styles.Length - 1; i++) // skip 1st element
                boxStyles.Items.Add(styles[i]);
            boxStyles.SelectedIndex = (int) StyleManager.Style - 1;

            if (ForceBrowse) Browse();
        }

        private void Browse()
        {
            var curPath = txtSteamPath.Text;
            if (curPath != null && curPath != String.Empty)
                folderBrowserDialog.SelectedPath = curPath.Replace('/', '\\'); // dialog doensn't likes direct slashes

            var result = folderBrowserDialog.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var newSteamPath = folderBrowserDialog.SelectedPath;
                if (SteamPaths.CheckSteamPath(newSteamPath))
                    if (DetectDota2Path(newSteamPath)) txtSteamPath.Text = newSteamPath.Replace('\\', '/');
                else
                    MetroMessageBox.Show(this, "\r\nSteam client not found\r\nPlease select another directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            Browse();
        }

        bool DetectDota2Path(string newSteamPath = null)
        {
            if (newSteamPath == null) newSteamPath = txtSteamPath.Text;
            var newDotaPath = SteamPaths.GetDota2Path(newSteamPath);
            return SteamPaths.CheckDota2Path(newDotaPath);
        }
    }
}
