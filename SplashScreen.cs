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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace Doto_Unlocker
{
    public partial class SplashScreen : MetroForm
    {
        const int GWL_STYLE = -16;
        const int WS_DISABLED = 0x08000000;

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern int SetForegroundWindow(IntPtr hWnd);

        void SetOwnerEnabled(bool enabled)
        {
            // WS_DISABLED doesn't makes form and all her components 'grayed'
            SetWindowLong(Owner.Handle, GWL_STYLE, GetWindowLong(Owner.Handle, GWL_STYLE) & ~WS_DISABLED | (enabled ? 0 : WS_DISABLED));
            SetForegroundWindow(Owner.Handle);
        }

        public SplashScreen()
        {
            InitializeComponent();
            StyleManager = this.metroStyleManager1;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetOwnerEnabled(false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            SetOwnerEnabled(true);
        }
    }
}
