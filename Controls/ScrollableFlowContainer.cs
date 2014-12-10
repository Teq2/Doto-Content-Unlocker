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
