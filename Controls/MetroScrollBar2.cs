using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Drawing;

namespace Doto_Unlocker.Controls
{
    /*
        BUGFIXES:
            1. Scrolling value out of REAL range
            2. Unable to decrease Value if step > curValue
    */

    class MetroScrollBar2: MetroScrollBar
    {
        public MetroScrollBar2()
        {
            Scroll += ScrollFix;
        }

        private void ScrollFix(object sender, ScrollEventArgs e)
        {
            var maxVal = Maximum - LargeChange + 1;
            if (e.NewValue > maxVal)
            {
                Value = e.NewValue = maxVal;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            int v = e.Delta / 120 * (Maximum - Minimum) / MouseWheelBarPartitions;

            if (Orientation == MetroScrollOrientation.Vertical)
            {
                if (v < Value)
                    Value -= v;
                else
                    Value = 0;
            }
            else
            {
                Value += v;
            }
        }
    }
}
