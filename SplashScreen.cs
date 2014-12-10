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
            SetWindowLong(Owner.Handle, GWL_STYLE, GetWindowLong(Owner.Handle, GWL_STYLE) &
                ~WS_DISABLED | (enabled ? 0 : WS_DISABLED));
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
        }
    }
}
