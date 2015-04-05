namespace Doto_Unlocker
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOk = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.txtSteamPath = new MetroFramework.Controls.MetroTextBox();
            this.btnBrowse = new MetroFramework.Controls.MetroButton();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.boxStyles = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.tileStyle = new MetroFramework.Controls.MetroTile();
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager(this.components);
            this.txtDotoPath = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).BeginInit();
            this.metroPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(287, 189);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseSelectable = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(368, 189);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtSteamPath
            // 
            this.txtSteamPath.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.txtSteamPath.Lines = new string[0];
            this.txtSteamPath.Location = new System.Drawing.Point(3, 24);
            this.txtSteamPath.MaxLength = 255;
            this.txtSteamPath.Name = "txtSteamPath";
            this.txtSteamPath.PasswordChar = '\0';
            this.txtSteamPath.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtSteamPath.SelectedText = "";
            this.txtSteamPath.Size = new System.Drawing.Size(358, 23);
            this.txtSteamPath.TabIndex = 1;
            this.txtSteamPath.UseSelectable = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(367, 24);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseSelectable = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(3, 2);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(79, 19);
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Text = "Steam path:";
            // 
            // boxStyles
            // 
            this.boxStyles.FormattingEnabled = true;
            this.boxStyles.ItemHeight = 23;
            this.boxStyles.Location = new System.Drawing.Point(3, 118);
            this.boxStyles.Name = "boxStyles";
            this.boxStyles.Size = new System.Drawing.Size(213, 29);
            this.boxStyles.TabIndex = 3;
            this.boxStyles.UseSelectable = true;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(3, 96);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(86, 19);
            this.metroLabel2.TabIndex = 2;
            this.metroLabel2.Text = "Color theme:";
            // 
            // tileStyle
            // 
            this.tileStyle.ActiveControl = null;
            this.tileStyle.Location = new System.Drawing.Point(223, 118);
            this.tileStyle.Name = "tileStyle";
            this.tileStyle.Size = new System.Drawing.Size(138, 29);
            this.tileStyle.TabIndex = 5;
            this.tileStyle.Text = "Set new theme";
            this.tileStyle.UseSelectable = true;
            this.tileStyle.Click += new System.EventHandler(this.tileStyle_Click);
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.Owner = this;
            // 
            // txtDotoPath
            // 
            this.txtDotoPath.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.txtDotoPath.Lines = new string[0];
            this.txtDotoPath.Location = new System.Drawing.Point(3, 70);
            this.txtDotoPath.MaxLength = 256;
            this.txtDotoPath.Name = "txtDotoPath";
            this.txtDotoPath.PasswordChar = '\0';
            this.txtDotoPath.PromptText = "Location will be detected automatically";
            this.txtDotoPath.ReadOnly = true;
            this.txtDotoPath.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtDotoPath.SelectedText = "";
            this.txtDotoPath.Size = new System.Drawing.Size(358, 23);
            this.txtDotoPath.TabIndex = 1;
            this.txtDotoPath.UseSelectable = true;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(3, 48);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(71, 19);
            this.metroLabel3.TabIndex = 2;
            this.metroLabel3.Text = "Doto path:";
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.boxStyles);
            this.metroPanel1.Controls.Add(this.tileStyle);
            this.metroPanel1.Controls.Add(this.txtSteamPath);
            this.metroPanel1.Controls.Add(this.metroLabel2);
            this.metroPanel1.Controls.Add(this.btnOk);
            this.metroPanel1.Controls.Add(this.metroLabel3);
            this.metroPanel1.Controls.Add(this.btnBrowse);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.Controls.Add(this.btnCancel);
            this.metroPanel1.Controls.Add(this.txtDotoPath);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(20, 60);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(446, 215);
            this.metroPanel1.TabIndex = 6;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Please select directory where \"Steam Client\" has been installed.";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(486, 295);
            this.Controls.Add(this.metroPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.SystemShadow;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Default;
            this.Text = "Settings";
            this.Theme = MetroFramework.MetroThemeStyle.Default;
            this.TransparencyKey = System.Drawing.Color.Empty;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).EndInit();
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroButton btnOk;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroTextBox txtSteamPath;
        private MetroFramework.Controls.MetroButton btnBrowse;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroComboBox boxStyles;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroTile tileStyle;
        public MetroFramework.Components.MetroStyleManager metroStyleManager1;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroTextBox txtDotoPath;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}