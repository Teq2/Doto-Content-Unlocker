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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.Properties;
using Doto_Unlocker.VPK;

namespace Doto_Unlocker.Model
{
    class HudConstructor
    {
        const string actionpanel = "actionpanel/";
        const string center_left_wide = actionpanel + "center_left_wide.png";
        const string center_right = actionpanel + "center_right.png";
        const string light_16_9 = actionpanel + "light_16_9.png";
        const string minimapborder = actionpanel + "minimapborder.png";
        const string portrait_wide = actionpanel + "portrait_wide.png";
        const string spacer_16_9 = actionpanel + "spacer_16_9.png";

        const string inventory = "inventory/";
        const string background_wide = inventory + "background_wide.png";
        const string light_right_16_9 = inventory + "light_right_16_9.png";
        const string rocks_16_9 = inventory + "rocks_16_9.png";
        const string spacer = inventory + "spacer.png";

        const string scoreboard = "scoreboard/";
        const string daynight = scoreboard + "daynight.png";
        const string topbar = scoreboard + "topbar.png";

        IHudReader reader;

        public HudConstructor(IHudReader reader)
        {
            this.reader = reader;
        }

        public Image Construct()
        {
            var pic = Resources.bkg_hud;
            var canvas = Graphics.FromImage(pic);
            var width = pic.Width;
            var height = pic.Height;
            canvas.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            float ratio;
            int newWidth;

            // LAYOUT FOR 1600x900

            var spacer_left = reader.ReadImage(spacer_16_9);
            newWidth = 152;
            canvas.DrawImage(spacer_left, 243, height - 227, newWidth, 227);

            var minimap = reader.ReadImage(minimapborder);
            ratio = minimap.GetRatio();
            newWidth = 264;
            canvas.DrawImage(minimap, 0, height - ratio * newWidth - 5, newWidth, ratio * newWidth + 5);

            var center_left = reader.ReadImage(center_left_wide);
            ratio = center_left.GetRatio();
            newWidth = 209;
            canvas.DrawImage(center_left, 539, height - ratio * newWidth-1, newWidth, ratio * newWidth+1);

            var center_right_i = reader.ReadImage(center_right);
            ratio = center_right_i.GetRatio();
            newWidth = 386;
            canvas.DrawImage(center_right_i, 748, height - ratio * newWidth-4 , newWidth, ratio * newWidth +4);

            var portrait = reader.ReadImage(portrait_wide);
            ratio = portrait.GetRatio();
            newWidth = 254;
            canvas.DrawImage(portrait, 333, height - ratio * newWidth, newWidth, ratio * newWidth);

            var background_inv = reader.ReadImage(background_wide);
            ratio = background_inv.GetRatio();
            newWidth = 391;
            canvas.DrawImage(background_inv, width - newWidth, height - ratio * newWidth, newWidth, ratio * newWidth);

            var spacer_inv = reader.ReadImage(spacer);
            ratio = spacer_inv.GetRatio();
            newWidth = 90;
            canvas.DrawImage(spacer_inv, 1129, height - ratio * newWidth - 10, newWidth, ratio * newWidth + 10);

            var rocks = reader.ReadImage(rocks_16_9);
            ratio = rocks.GetRatio();
            newWidth = 222;
            canvas.DrawImage(rocks, 1090, height - ratio * newWidth, newWidth, ratio * newWidth );

            // top
            var top = reader.ReadImage(topbar);
            newWidth = width;
            canvas.DrawImage(top, 0, 0, width, 35);

            var clocks = reader.ReadImage(daynight);
            ratio = clocks.GetRatio();
            newWidth = 80;
            canvas.DrawImage(clocks, 760, -40, newWidth, ratio * newWidth);

            // overlay
            var overlay = Resources.overlay;
            canvas.DrawImage(overlay, 0, -100, 1600, 1000);

            return pic;
        }
    }
}
