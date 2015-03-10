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
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Drawing;

namespace Doto_Unlocker.Controls
{
    class ScrollableFlowContainer: MetroUserControl
    {
        public MetroScrollBar verticalScrollbar = new MetroScrollBar2();
        MyFlowLayoutPanel contentPanel = new MyFlowLayoutPanel();

        public int EffectiveWidth
        {
            get
            {
                return contentPanel.MaxLineWidth + verticalScrollbar.Width;
            }
        }

        public int Count
        {
            get
            {
                return contentPanel.Controls.Count;
            }
        }

        public Control this[int idx]
        {
            get
            {
                return contentPanel.Controls[idx];
            }
        }

        public ScrollableFlowContainer()
        {
            verticalScrollbar.Scroll += verticalScrollbar_Scroll;
            contentPanel.Paint += contentPanel_Paint;
            contentPanel.MouseWheel += contentPanel_MouseWheel;
            contentPanel.MouseHover += contentPanel_MouseHover;
            contentPanel.MouseEnter += contentPanel_MouseEnter;

            contentPanel.Dock = DockStyle.Fill;
            verticalScrollbar.Dock = DockStyle.Right;
            contentPanel.AutoScroll = true;
            Controls.Add(contentPanel);
            Controls.Add(verticalScrollbar);
        }

        void verticalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            contentPanel.SimpleScroll(0, -e.NewValue);
        }

        void contentPanel_Paint(object sender, PaintEventArgs e)
        {
            if (contentPanel.VerticalScroll.Visible)
            {
                verticalScrollbar.Visible = true;

                verticalScrollbar.Minimum = contentPanel.VerticalScroll.Minimum;
                verticalScrollbar.Maximum = contentPanel.VerticalScroll.Maximum;
                verticalScrollbar.SmallChange = contentPanel.VerticalScroll.SmallChange;
                verticalScrollbar.LargeChange = contentPanel.VerticalScroll.LargeChange;
            }
            else
            {
                verticalScrollbar.Visible = false;
            }
        }

        void contentPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            verticalScrollbar.Value = Math.Abs(contentPanel.VerticalScroll.Value);
        }

        void contentPanel_MouseHover(object sender, EventArgs e)
        {
            contentPanel.Focus();
        }

        void contentPanel_MouseEnter(object sender, EventArgs e)
        {
            contentPanel.Focus();
        }

        public void Add(Control c)
        {
            c.MouseEnter += (sender, e) => ((Control)sender).Focus();
            contentPanel.Controls.Add(c);
        }

        public void Clear()
        {
            for (int i = contentPanel.Controls.Count - 1; i >= 0 ; i--)
            {
                contentPanel.Controls[i].Dispose();
            }
            contentPanel.Controls.Clear();
        }
    }
}
