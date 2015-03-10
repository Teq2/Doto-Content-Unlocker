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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Controls;
using MetroFramework.Drawing;

namespace Doto_Unlocker.Controls
{
    class ProgressOverlay : MetroUserControl
    {
        class MetroProgressSpinnerTransparent : MetroProgressSpinner
        {
            public MetroProgressSpinnerTransparent()
            {
                SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                SetStyle(ControlStyles.Opaque, true);
                this.BackColor = Color.Transparent;
            }
        }

        int spinnerWidth = 80;
        [DefaultValue(80)]
        public int SpinnerWidth
        {
            get { return spinnerWidth; }
            set { spinnerWidth = value; }
        }

        int spinnerHeight = 80;
        [DefaultValue(80)]
        public int SpinnerHeight
        {
            get { return spinnerHeight; }
            set { spinnerHeight = value; }
        }

        int opacity = 60;
        [DefaultValue(60)]
        public int Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        MetroStyleManager style;
        MetroProgressSpinnerTransparent spinner;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public ProgressOverlay()
        {
            InitializeComponent();
            
            spinner.Maximum = 100;
            spinner.Value = 50;
            spinner.UseCustomBackColor = true;
            this.Dock = DockStyle.Fill;
            this.UseCustomBackColor = true;
            this.Controls.Add(spinner);
        }

        void InitializeComponent()
        {
            style = new MetroStyleManager();
            style.Owner = this;
            this.StyleManager = style;

            spinner = new MetroProgressSpinnerTransparent();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            double opacity = 255.0 / 100.0 * this.opacity;
            this.BackColor = Color.FromArgb((int)opacity, MetroPaint.BackColor.Form(Theme));
            spinner.Location = new System.Drawing.Point(this.Width / 2 - spinnerWidth / 2, this.Height / 2 - spinnerHeight / 2);
            spinner.Size = new System.Drawing.Size(spinnerWidth, spinnerHeight);

            base.OnPaint(e);
        }
    }
}
