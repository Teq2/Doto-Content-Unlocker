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
