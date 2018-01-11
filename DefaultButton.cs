using SalesCounter.src.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesCounter.src.custom.control
{
    class DefaultButton:Button
    {
        //private bool isEnter;
        //public DefaultButton():base()
        //{
        //    SetStyle(ControlStyles.UserPaint, true);
        //    SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        //    SetStyle(ControlStyles.DoubleBuffer, true);
        //    SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        //    BackColor = Color.Transparent;

        //    MouseEnter += (sender, e) =>
        //    {
        //        isEnter = true;
        //    };
        //    MouseLeave += (sender, e) =>
        //    {
        //        isEnter = false;
        //    };
        //}
        //protected override void OnPaint(PaintEventArgs pevent)
        //{
        //    //Region
        //    base.OnPaintBackground(pevent);
        //    var rect = ClientRectangle;
        //    var radius = (int)((rect.Width > rect.Height ? rect.Height : rect.Width) * 0.2);
        //    var gPath = DrawUtil.GetRoundRect(rect, radius);
        //    var c = Color.FromArgb(0, 163, 255);
        //    if (isEnter)
        //    {
        //        c = Color.FromArgb(200,0, 163, 255);
        //    }

        //    pevent.Graphics.FillPath(new SolidBrush(c), gPath);
        //    TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle, ForeColor);
        //}
        //protected override void OnClick(EventArgs e)
        //{
        //    base.OnClick(e);
        //}

        #region -- Members --
        private RectangleF centerRect;
        private bool mouseOver;
        private bool pressed;
        private System.ComponentModel.IContainer components = null;
        #endregion

        #region -- Properties --

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        [Browsable(true), DefaultValue(10)]
        [Category("Appearance")]
        public int CornerRadius { get; set; }

        /// <summary>
        /// Gets or sets the color of the focus.
        /// </summary>
        /// <value>The color of the focus.</value>
        [Browsable(true), DefaultValue(typeof(Color), "Orange")]
        [Category("Appearance")]
        public Color FocusColor { get; set; }

 
        [Browsable(true), DefaultValue(typeof(Color), "White")]
        [Category("Appearance")]
        public  Color BtnColor
        {
            get;
            set;
        }

        #endregion

        #region -- Constructor --
        /// <summary>
        /// Initializes a new instance of the <see cref="PulseButton"/> class.
        /// </summary>
        public DefaultButton()
        {
            // Control styles
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            InitializeComponent();
            // Layout & initialization
            SuspendLayout();
            FocusColor = Color.Orange;
            CornerRadius = 10;
            Image = null;
            Size = new Size(40, 40);
            centerRect = new RectangleF(0,0,Width,Height);
            BtnColor = SystemColors.Control;
            ResumeLayout(true);
        }

        #endregion

        #region -- EventHandlers --

        #endregion

        #region -- Protected overrides --

        #region - Mouse -

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != MouseButtons.Left) return;
            pressed = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;
            pressed = true;
        }

        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.Control.OnMouseMove(System.Windows.Forms.MouseEventArgs)"/> event.
        /// </summary>
        /// <param name="mevent">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            mouseOver = centerRect.Contains(mevent.Location);
        }

        /// <summary>
        /// Raises the <see cref="Control.OnMouseLeave"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouseOver = false;
            pressed = false;
        }

        #endregion


        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            centerRect = new RectangleF(0, 0, Width, Height);
        }

 
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            base.OnPaintBackground(e);
            // Set Graphics interpolation and smoothing
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (centerRect.IsEmpty) return;
            // Draw center
            DrawCenter(g);

            // Draw border
           // DrawBorder(g);
            // Image
            if (Image != null)
                g.DrawImage(Image, centerRect);

            // Draw highlight
            if (mouseOver)
                DrawHighLight(g);

            // Text
            DrawText(g);
        }

        #endregion

        #region -- Protected virtual methods --

        /// <summary>
        /// Draws the border.
        /// </summary>
        /// <param name="g">The graphics object</param>
        protected virtual void DrawBorder(Graphics g)
        {
            using (var pen = new Pen(!Focused ? Color.FromArgb(60, Color.Black) : FocusColor, 2))
                PaintShape(g, pen, centerRect);
        }

        /// <summary>
        /// Draws the center.
        /// </summary>
        /// <param name="g">The graphics object</param>
        protected virtual void DrawCenter(Graphics g)
        {
            if (Enabled)
            {
                using (var lgb = new SolidBrush(BtnColor))
                {
                    PaintShape(g, lgb, centerRect);
                }
            }
            else
            {
                using (var lgb = new SolidBrush(Color.Gray))
                    PaintShape(g, lgb, centerRect);

            }
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="g">The graphics object</param>
        protected virtual void DrawText(Graphics g)
        {
            var format = new StringFormat(StringFormat.GenericDefault) { Trimming = StringTrimming.EllipsisCharacter };
            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            //format.FormatFlags ^= StringFormatFlags.LineLimit;
            format.HotkeyPrefix = HotkeyPrefix.Show;
            SizeF size = g.MeasureString(Text, Font, new SizeF(centerRect.Width-2, centerRect.Height-2), format);
            RectangleF textRect = GetAlignPlacement(TextAlign, centerRect, size);
            using (var sb = new SolidBrush(ForeColor))
                g.DrawString(Text, Font, sb, textRect, format);
        }

        /// <summary>
        /// Draws the high light.
        /// </summary>
        /// <param name="g">The graphics object</param>
        protected virtual void DrawHighLight(Graphics g)
        {
            RectangleF highlightRect = centerRect;
            highlightRect.Inflate(-1, -1);
            using (var pen = new Pen(Color.FromArgb(60, Color.White), 4))
            {
                g.DrawPath(pen, GetRoundRect(g, highlightRect, CornerRadius));
            }
        }

        /// <summary>
        /// Paints the shape.
        /// </summary>
        /// <param name="g">The graphics object</param>
        /// <param name="p">The pen</param>
        /// <param name="rectangle">The rectangle.</param>
        protected virtual void PaintShape(Graphics g, Pen p, RectangleF rectangle)
        {
                using (var path = GetRoundRect(g, rectangle, CornerRadius))
                g.DrawPath(p, path);
        }

        /// <summary>
        /// Paints the shape.
        /// </summary>
        /// <param name="g">The graphics object</param>
        /// <param name="b">The brush</param>
        /// <param name="rectangle">The rectangle.</param>
        protected virtual void PaintShape(Graphics g, Brush b, RectangleF rectangle)
        {
                using (var path = GetRoundRect(g, rectangle, CornerRadius))
                g.FillPath(b, path);
        }

        #endregion

        #region -- Public static methods --

        /// <summary>
        /// Gets a path of a rectangle with round corners.
        /// </summary>
        /// <param name="g">The graphics object</param>
        /// <param name="rect">The rectangle</param>
        /// <param name="radius">The corner radius</param>
        /// <returns></returns>
        public static GraphicsPath GetRoundRect(Graphics g, RectangleF rect, float radius)
        {
            var gp = new GraphicsPath();
            var diameter = radius * 2;
            gp.AddArc(rect.X + rect.Width - diameter, rect.Y, diameter, diameter, 270, 90);
            gp.AddArc(rect.X + rect.Width - diameter, rect.Y + rect.Height - diameter, diameter, diameter, 0, 90);
            gp.AddArc(rect.X, rect.Y + rect.Height - diameter, diameter, diameter, 90, 90);
            gp.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            gp.CloseFigure();
            return gp;
        }

        /// <summary>
        /// Gets the placement.
        /// </summary>
        /// <param name="align">The alignment of the element</param>
        /// <param name="rect">A retangle</param>
        /// <param name="element">The element to be placed</param>
        /// <returns></returns>
        public static RectangleF GetAlignPlacement(ContentAlignment align, RectangleF rect, SizeF element)
        {
            // Left & Top (default)
            float lft = rect.Left;
            float top = rect.Y;
            // Right
            if ((align & (ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight)) != 0)
                lft = rect.Right - element.Width;
            // Center
            else if ((align & (ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter)) != 0)
                lft = (rect.Width / 2) - (element.Width / 2) + rect.Left;
            // Bottom
            if ((align & (ContentAlignment.BottomCenter | ContentAlignment.BottomLeft | ContentAlignment.BottomRight)) != 0)
                top = rect.Bottom - element.Height;
            // Middle
            else if ((align & (ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft | ContentAlignment.MiddleRight)) != 0)
                top = (rect.Height / 2) - (element.Height / 2) + rect.Y;

            return new RectangleF(lft, top, element.Width, element.Height);
        }

        #endregion

        #region -- Private methods --


        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion
    }
}
