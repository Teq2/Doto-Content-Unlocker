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
        public static extern int BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight, HandleRef hSrcDC, int xSrc, int ySrc, int dwRop);
        #endregion

        private bool isHovered = false;
        private const int titlePadding = 2;
        private const int underLinePadding = 1;
        private MetroColorStyle lastStyle;

        public ContentTile()
        {
            TabStop = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true); // to prevent background erasing on Invalidate();
            this.UseCustomBackColor = true;
            backbufferContext = BufferedGraphicsManager.Current;
           // RecreateBuffers(); // disabled, coz 1st resize always occurs before 1st paint
        }

        #region Properties
        private Image tileImageOriginal = null;
        private Image tileImageResized = null;
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

        private bool outline = true;
        [DefaultValue(true)]
        [Category(CotrolDefaults.PropertyCategory.Behaviour)]
        public bool DynamicOutline
        {
            get { return outline; }
            set { outline = value; }
        }

        private int outlineWidth = 1;
        [DefaultValue(1)]
        [Category(CotrolDefaults.PropertyCategory.Appearance)]
        public int OutlineWidth
        {
            get { return outlineWidth; }
            set { outlineWidth = value; }
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
                if (tileImageOriginal.Width != this.Width && tileImageOriginal.Height != Height - titleHeight)
                {
                    tileImageResized = new Bitmap(tileImageOriginal, new System.Drawing.Size(Width, Height - titleHeight));
                }
                else
                    tileImageResized = tileImageOriginal;
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
            drawingGraphics.Clear(this.BackColor);

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

            if (tileImageResized != null)
            {
                drawingGraphics.DrawImage(tileImageResized, imagePoint.X, imagePoint.Y, tileImageResized.Width, tileImageResized.Height);
            }

            // DRAW TITLE-RECT
            Font titleFont = MetroFonts.Tile(tileTextFontSize, tileTextFontWeight);
            Size textSize = TextRenderer.MeasureText(titleText, titleFont);

            // UNDER-LINE
            Color lineShadow = MetroPaint.GetStyleColor(Style);
            Color lineColor = MetroPaint.BorderColor.TabControl.Normal(Theme);
            Rectangle underLine = new Rectangle(outlineWidth, titleRectangle.Bottom - underLineWidth - outlineWidth - underLinePadding,
                titleRectangle.Right - outlineWidth * 2, underLineWidth);

            using (var baseColor = new SolidBrush(lineColor))
            {
                drawingGraphics.FillRectangle(baseColor, underLine);
                using (var highlighted = new SolidBrush(lineShadow))
                {
                    drawingGraphics.FillRectangle(highlighted, underLine.Left + underLinePadding, underLine.Top, textSize.Width + 10, underLineWidth);
                }
            }

            // TEXT
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;
            Color textColor = MetroPaint.ForeColor.TabControl.Normal(Theme);
            Point textPoint = new Point(underLine.Left, underLine.Top - textSize.Height - titlePadding);
            Rectangle textRect = new Rectangle(textPoint, new Size(underLine.Width, underLine.Top - textPoint.Y));
            TextRenderer.DrawText(drawingGraphics, titleText, titleFont, textRect, textColor, flags);

            // BORDER
            if (DesignMode || (outline && isHovered))
            {
                //Color borderColor = MetroPaint.BorderColor.Button.Hover(Theme);
                using (Pen p = new Pen(lineShadow))
                {
                    drawingGraphics.DrawRectangle(p, new Rectangle(0, 0, Width - outlineWidth, Height - outlineWidth));
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
                    new HandleRef(drawingGraphics, hdc_buffer), e.ClipRectangle.X, e.ClipRectangle.Y, 0xCC0020);
		    }
		    finally
		    {
			    e.Graphics.ReleaseHdcInternal(hdc);
                drawingGraphics.ReleaseHdcInternal(hdc_buffer);
		    }
        }


        protected override void OnResize(EventArgs e)
        {
            needRedraw = true;
            ResizeImage();
            RecreateBuffers();
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
