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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Drawing;

namespace Doto_Unlocker.Controls
{
    public enum TitleDocking { Top, Bottom };

    public class ContentTile : MetroUserControl
    {
        #region native
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        private static extern int BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight, HandleRef hSrcDC, int xSrc, int ySrc, int dwRop);

        private const int SRCCOPY = 0x00CC0020; /* dest = source */
        #endregion

        private bool isHovered = false;
        private const int titleVerticalIndent = 2;
        private const int underlineVerticalIndent = 1;
        private MetroColorStyle lastStyle;

        public ContentTile()
        {
            TabStop = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true); // ControlStyles.Opaque prevents background from erasing on Invalidate();
            backbufferContext = BufferedGraphicsManager.Current;
        }

        #region Properties
        private Image tileImageOriginal = null;
        private Image tileImage = null;
        [DefaultValue(null)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public Image TileImage
        {
            get { return tileImageOriginal; }
            set 
            { 
                tileImageOriginal = value;
                ResizeImage();
            }
        }

        private bool drawBorder = true;
        [DefaultValue(true)]
        [Category(CotrolDefaults.PropertyCategory.Behaviour)]
        public bool DynamicBorder
        {
            get { return drawBorder; }
            set { drawBorder = value; }
        }

        private int borderWidth = 1;
        [DefaultValue(1)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public int BorderWidth
        {
            get { return borderWidth; }
            set { borderWidth = value; }
        }

        private int underLineWidth = 4;
        [DefaultValue(4)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public int UnderLineWidth
        {
            get { return underLineWidth; }
            set { underLineWidth = value; }
        }

        private int titleHeight = 25;
        [DefaultValue(25)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public int TitleLineHeight
        {
            get { return titleHeight; }
            set { titleHeight = value; }
        }

        private string titleText = string.Empty;
        [DefaultValue("")]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public string TitleText
        {
            get { return titleText; }
            set { titleText = value!=null?value:string.Empty; }
        }

        private TitleDocking titleDock = TitleDocking.Top;
        [DefaultValue(TitleDocking.Top)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public TitleDocking TitleDock
        {
            get { return titleDock; }
            set { titleDock = value; }
        }

        private MetroTileTextSize tileTextFontSize = MetroTileTextSize.Medium;
        [DefaultValue(MetroTileTextSize.Medium)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public MetroTileTextSize TileTextFontSize
        {
            get { return tileTextFontSize; }
            set { tileTextFontSize = value; Refresh(); }
        }

        private MetroTileTextWeight tileTextFontWeight = MetroTileTextWeight.Light;
        [DefaultValue(MetroTileTextWeight.Light)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public MetroTileTextWeight TileTextFontWeight
        {
            get { return tileTextFontWeight; }
            set { tileTextFontWeight = value; Refresh(); }
        }

        #endregion

        private BufferedGraphicsContext backbufferContext;
        private BufferedGraphics backbufferGraphics;
        private Graphics drawingGraphics;
        bool needRedraw = true;

        void ResizeImage()
        {
            if (tileImageOriginal != null)
            {
                if (tileImageOriginal.Width != this.Width && tileImageOriginal.Height != this.Height - titleHeight)
                {
                    tileImage = new Bitmap(tileImageOriginal, new Size(this.Width, this.Height - titleHeight));
                }
                else
                    tileImage = tileImageOriginal;
            }
        }

        private void RecreateBuffers()
        {
            if (backbufferGraphics != null) backbufferGraphics.Dispose();
            backbufferGraphics = backbufferContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, Width, Height));
            drawingGraphics = backbufferGraphics.Graphics;
            drawingGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (backbufferGraphics != null) backbufferGraphics.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Redraw()
        {
            Color styleColor = MetroPaint.GetStyleColor(Style);
            Font titleFont = MetroFonts.Tile(tileTextFontSize, tileTextFontWeight);
            
            drawingGraphics.Clear(MetroPaint.BackColor.Form(Theme));

            // Positioning
            Point imagePoint;
            Rectangle titleRectangle;
            switch (titleDock)
            {
                case TitleDocking.Top:
                    imagePoint = new Point(0, titleHeight);
                    titleRectangle = new Rectangle(new Point(0, 0), new Size(Width, titleHeight));
                    break;
                case TitleDocking.Bottom:
                default:
                    imagePoint = new Point(0, 0);
                    titleRectangle = new Rectangle(new Point(0, Height - titleHeight), new Size(Width, titleHeight));
                    break;
            }

            // DRAW IMAGE
            if (tileImage != null)
                drawingGraphics.DrawImage(tileImage, imagePoint.X, imagePoint.Y, tileImage.Width, tileImage.Height);

            // UNDERLINE
            Size textSize = TextRenderer.MeasureText(titleText, titleFont);
            Color lineColor = MetroPaint.BorderColor.TabControl.Normal(Theme);
            Rectangle underLine = new Rectangle(borderWidth, titleRectangle.Bottom - underLineWidth - borderWidth - underlineVerticalIndent, 
                titleRectangle.Right - borderWidth * 2, underLineWidth);

            using (var baseColor = new SolidBrush(lineColor))
            {
                drawingGraphics.FillRectangle(baseColor, underLine);
                // highlight
                using (var highlighted = new SolidBrush(styleColor))
                {
                    drawingGraphics.FillRectangle(highlighted, underLine.Left + underlineVerticalIndent, underLine.Top, textSize.Width + 10, underLineWidth);
                }
            }

            // TEXT
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;
            Color textColor = MetroPaint.ForeColor.TabControl.Normal(Theme);
            Point textPoint = new Point(underLine.Left, underLine.Top - textSize.Height - titleVerticalIndent);
            Rectangle textBounds = new Rectangle(textPoint, new Size(underLine.Width - textPoint.X, underLine.Top - textPoint.Y));
            TextRenderer.DrawText(drawingGraphics, titleText, titleFont, textBounds, textColor, flags);

            // BORDER
            if (DesignMode || (drawBorder && isHovered))
            {
                using (Pen p = new Pen(styleColor))
                {
                    drawingGraphics.DrawRectangle(p, new Rectangle(0, 0, Width - borderWidth, Height - borderWidth));
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (lastStyle != Style)
            {
                lastStyle = Style;
                needRedraw = true;
            }

            if (needRedraw) 
            {
                needRedraw = false;
                Redraw();
            }
            
            // FAST PARTIAL RENDER
		    IntPtr hdc = e.Graphics.GetHdc();
            IntPtr hdc_buffer = backbufferGraphics.Graphics.GetHdc();
		    try
		    {
                BitBlt(new HandleRef(e.Graphics, hdc), e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height,
                    new HandleRef(drawingGraphics, hdc_buffer), e.ClipRectangle.X, e.ClipRectangle.Y, SRCCOPY);
		    }
		    finally
		    {
			    e.Graphics.ReleaseHdcInternal(hdc);
                drawingGraphics.ReleaseHdcInternal(hdc_buffer);
		    }
        }

        protected override void OnLoad(EventArgs e)
        {
            RecreateBuffers();
            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            needRedraw = true;
            ResizeImage();
            if (backbufferGraphics != null) RecreateBuffers();
            base.OnResize(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            needRedraw = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            needRedraw = true;
            Invalidate();
            base.OnMouseLeave(e);
        }
    }
}
