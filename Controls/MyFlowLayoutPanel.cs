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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Drawing;


namespace Doto_Unlocker.Controls
{
    class MyFlowLayoutPanel : Panel, IDisposable
    {
        #region native
        [DllImport("user32.dll")]
        public static extern bool ShowScrollBar(IntPtr hWnd, int bar, int cmd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

        private const int GW_STYLE = -16;
        private const uint WS_VSCROLL = 0x200000;
        private const uint WS_HSCROLL = 0x100000;
        private const int WM_WINDOWPOSCHANGING = 0x46;

        [System.Security.SecuritySafeCritical]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (!DesignMode)
            {
                //if (m.Msg == WM_WINDOWPOSCHANGING)
                {
                    uint style = GetWindowLong(this.Handle, GW_STYLE);
                    if ((style & 0x300000) != 0) // 0x300000 = WS_VSCROLL|WS_HSCROLL
                        SetWindowLong(this.Handle, GW_STYLE, (style & 0xFFCFFFFF));
                }
            }
        }
        #endregion

        public MyFlowLayoutPanel()
        {
            ShowScrollBar(Handle, 3, 0);
        }

        public void SimpleScroll(int x, int y)
        {
            SetDisplayRectLocation(x, y);
        }

        int CalcLineWidth()
        {
            int max = 0;
            foreach (Control control in Controls)
            {
                var margin = control.Margin;
                int cur = control.Location.X + control.Width + margin.Right;
                if (cur > max) max = cur;
            }

            return max != 0 ? max : Width;
        }

        public int MaxLineWidth
        {
            get
            {
                return CalcLineWidth();
            }
        }

        public override LayoutEngine LayoutEngine
        {
            get
            {
                return FlowLayout.Instance;
            }
        }
    }

    // Simple Left-to-Right layout with wrapping
    internal class FlowLayout : LayoutEngine
    {
        internal static readonly FlowLayout Instance = new FlowLayout();

        public override bool Layout( object container, LayoutEventArgs layoutEventArgs)
        {
            ScrollableControl cont = (ScrollableControl)container;
            Point newLoc = new Point();
            int rowOffset = cont.DisplayRectangle.Y;
            int maxHeight = 0;

            foreach (Control control in cont.Controls)
            {
                var margin = control.Margin;

                var width = margin.Left + margin.Right + control.Width;
                if (width > cont.ClientRectangle.Width - newLoc.X)
                {
                    // next row
                    rowOffset += maxHeight;
                    maxHeight = 0;
                    newLoc.X = 0;
                }

                var height = margin.Top + margin.Bottom + control.Height;
                if (height > maxHeight) maxHeight = height;

                newLoc.Y = rowOffset + margin.Top;
                newLoc.X += margin.Left;
                control.Location = newLoc;
                newLoc.X += control.Width + margin.Right;
            }
            return false;
        }
    }
}
