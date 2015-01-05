namespace Doto_Unlocker
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager(this.components);
            this.tabControl = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroStyleExtender1 = new MetroFramework.Components.MetroStyleExtender(this.components);
            this.btnSettings = new MetroFramework.Controls.MetroTile();
            this.btnRun = new MetroFramework.Controls.MetroTile();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.contentName = new MetroFramework.Controls.MetroLabel();
            this.rightPanel = new MetroFramework.Controls.MetroPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.currentContent = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRandom = new MetroFramework.Controls.MetroTile();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).BeginInit();
            this.tabControl.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentContent)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.Owner = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.metroTabPage1);
            this.tabControl.Controls.Add(this.metroTabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.FontSize = MetroFramework.MetroTabControlSize.Tall;
            this.tabControl.Location = new System.Drawing.Point(20, 60);
            this.tabControl.MinimumSize = new System.Drawing.Size(545, 300);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 1;
            this.tabControl.Size = new System.Drawing.Size(545, 623);
            this.tabControl.TabIndex = 0;
            this.tabControl.UseSelectable = true;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.metroTabControl1_SelectedIndexChanged);
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 44);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(537, 575);
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "metroTabPage1";
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            this.metroTabPage1.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.VerticalScrollbarSize = 10;
            // 
            // metroTabPage2
            // 
            this.metroTabPage2.HorizontalScrollbarBarColor = true;
            this.metroTabPage2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.HorizontalScrollbarSize = 10;
            this.metroTabPage2.Location = new System.Drawing.Point(4, 44);
            this.metroTabPage2.Name = "metroTabPage2";
            this.metroTabPage2.Size = new System.Drawing.Size(537, 575);
            this.metroTabPage2.TabIndex = 1;
            this.metroTabPage2.Text = "metroTabPage2";
            this.metroTabPage2.VerticalScrollbarBarColor = true;
            this.metroTabPage2.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.VerticalScrollbarSize = 10;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(277, 29);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(223, 19);
            this.metroLabel5.TabIndex = 4;
            this.metroLabel5.Text = "made by Teq (1371117@gmail.com)";
            // 
            // btnSettings
            // 
            this.btnSettings.ActiveControl = null;
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSettings.Location = new System.Drawing.Point(0, 0);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(178, 36);
            this.btnSettings.TabIndex = 7;
            this.btnSettings.Text = "Settings";
            this.btnSettings.TileImage = global::Doto_Unlocker.Properties.Resources.settings_32;
            this.btnSettings.TileImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSettings.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnSettings.UseSelectable = true;
            this.btnSettings.UseStyleColors = true;
            this.btnSettings.UseTileImage = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnRun
            // 
            this.btnRun.ActiveControl = null;
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(9, 569);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(363, 50);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "Run game";
            this.btnRun.TileImage = global::Doto_Unlocker.Properties.Resources.joystick_48;
            this.btnRun.TileImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRun.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnRun.UseSelectable = true;
            this.btnRun.UseStyleColors = true;
            this.btnRun.UseTileImage = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel4.Location = new System.Drawing.Point(9, 94);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(152, 25);
            this.metroLabel4.TabIndex = 9;
            this.metroLabel4.Text = "Currently Installed:";
            // 
            // contentName
            // 
            this.contentName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.contentName.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.contentName.Location = new System.Drawing.Point(0, 231);
            this.contentName.Name = "contentName";
            this.contentName.Size = new System.Drawing.Size(363, 25);
            this.contentName.TabIndex = 9;
            this.contentName.Text = "                       ";
            this.contentName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.panel1);
            this.rightPanel.Controls.Add(this.tableLayoutPanel1);
            this.rightPanel.Controls.Add(this.btnRun);
            this.rightPanel.Controls.Add(this.metroLabel4);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.HorizontalScrollbarBarColor = true;
            this.rightPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.rightPanel.HorizontalScrollbarSize = 10;
            this.rightPanel.Location = new System.Drawing.Point(565, 60);
            this.rightPanel.MinimumSize = new System.Drawing.Size(380, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(380, 623);
            this.rightPanel.TabIndex = 10;
            this.rightPanel.VerticalScrollbarBarColor = true;
            this.rightPanel.VerticalScrollbarHighlightOnWheel = false;
            this.rightPanel.VerticalScrollbarSize = 10;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.currentContent);
            this.panel1.Controls.Add(this.contentName);
            this.panel1.Location = new System.Drawing.Point(9, 122);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(363, 256);
            this.panel1.TabIndex = 13;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // currentContent
            // 
            this.currentContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentContent.Location = new System.Drawing.Point(0, 0);
            this.currentContent.Margin = new System.Windows.Forms.Padding(0);
            this.currentContent.Name = "currentContent";
            this.currentContent.Size = new System.Drawing.Size(363, 231);
            this.currentContent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.currentContent.TabIndex = 10;
            this.currentContent.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnSettings, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRandom, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 44);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(363, 36);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // btnRandom
            // 
            this.btnRandom.ActiveControl = null;
            this.btnRandom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRandom.Location = new System.Drawing.Point(184, 0);
            this.btnRandom.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnRandom.Name = "btnRandom";
            this.btnRandom.Size = new System.Drawing.Size(179, 36);
            this.btnRandom.TabIndex = 7;
            this.btnRandom.Text = "Select random";
            this.btnRandom.TileImage = global::Doto_Unlocker.Properties.Resources.dice_32;
            this.btnRandom.TileImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRandom.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnRandom.UseSelectable = true;
            this.btnRandom.UseStyleColors = true;
            this.btnRandom.UseTileImage = true;
            this.btnRandom.Click += new System.EventHandler(this.btnRandom_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 703);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.metroLabel5);
            this.MinimumSize = new System.Drawing.Size(965, 600);
            this.Name = "MainForm";
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.SystemShadow;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.StyleManager = this.metroStyleManager1;
            this.Text = "Doto Content Unlocker";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.rightPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.currentContent)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager1;
        private MetroFramework.Controls.MetroTabControl tabControl;
        private MetroFramework.Components.MetroStyleExtender metroStyleExtender1;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroTile btnSettings;
        private MetroFramework.Controls.MetroTile btnRun;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel contentName;
        private MetroFramework.Controls.MetroPanel rightPanel;
        private System.Windows.Forms.PictureBox currentContent;
        private MetroFramework.Controls.MetroTile btnRandom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private MetroFramework.Controls.MetroTabPage metroTabPage1;
        private MetroFramework.Controls.MetroTabPage metroTabPage2;
    }
}

