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
using System.Diagnostics;
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
    class ButtonOverlay : MetroUserControl
    {
        MetroStyleManager style = new MetroStyleManager();
        MetroButton btn = new MetroButton();
        float opacity = 35;

        #region properties
        public event EventHandler ButtonClick
        {
            add
            {
                btn.Click += value;
            }
            remove
            {
                btn.Click -= value;
            }
        }

        [DefaultValue(120)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public int ButtonWidth
        {
            get { return btn.Width; }
            set
            {
                btn.Width = value;
            }
        }

        [DefaultValue(50)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public int ButtonHeight
        {
            get { return btn.Height; }
            set
            {
                btn.Height = value;
            }
        }

        [DefaultValue("")]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public string ButtonText
        {
            get { return btn.Text; }
            set
            {
                btn.Text = value;
            }
        }
        #endregion

        public ButtonOverlay()
        {
            this.style.Owner = this;
            this.StyleManager = style;
            this.Dock = DockStyle.Fill;
            this.UseCustomBackColor = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);

            this.Controls.Add(btn);
            btn.Width = 120;
            btn.Height = 50;
            btn.FontSize = MetroFramework.MetroButtonSize.Tall;
            btn.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            btn.TabStop = false;
        }      

        protected override void OnResize(EventArgs e)
        {
            btn.Location = new Point(Width / 2 - btn.Width / 2, Height / 2 - btn.Height / 2);
            btn.Show();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float opacity = 255f / 100f * this.opacity;
            this.BackColor = Color.FromArgb((int)opacity, MetroPaint.BorderColor.TabControl.Normal(Theme)); // MetroPaint.BackColor.Form(Theme)
            e.Graphics.Clear(BackColor);
        }
    }
}
