using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesCounter.src.custom.control
{
     class DefaultVScroll : UserControl
    {
        protected Color mChannelColor = Color.FromArgb(75, 85, 104);
        protected Image mUpArrowImage = null;

        protected Image mDownArrowImage = null;

        protected int mLargeChange = 1;
        protected int mSmallChange = 1;
        protected int mMinimum = 0;
        protected int mMaximum = 100;
        protected int mValue = 0;
        protected int mThumbTop = 0;

        private int nClickPoint;

        private int imgHeight = 0;

        private bool mThumbDown = false;
        private bool mThumbDragging = false;

        public new event EventHandler Scroll = null;
        public event EventHandler ValueChanged = null;
        //计算滑块高
        private int GetThumbHeight()
        {
            //高度
            int nTrackHeight = (this.Height - (imgHeight + imgHeight));
            //
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;

            if (nThumbHeight > nTrackHeight)
            {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }
            return nThumbHeight;
        }

        public DefaultVScroll()
        {

            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            mChannelColor = Color.FromArgb(75, 85, 104);
            UpArrowImage = Resource.up;
            DownArrowImage = Resource.down;
            imgHeight = 0;

            this.Width = 4;
            base.MinimumSize = new Size(4, imgHeight + imgHeight + GetThumbHeight());
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), Category("Behavior"), Description("LargeChange")]
        public int LargeChange
        {
            get { return mLargeChange; }
            set
            {
                mLargeChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), Category("Behavior"), Description("SmallChange")]
        public int SmallChange
        {
            get { return mSmallChange; }
            set
            {
                mSmallChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Minimum")]
        public int Minimum
        {
            get { return mMinimum; }
            set
            {
                mMinimum = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Maximum")]
        public int Maximum
        {
            get { return mMaximum; }
            set
            {
                mMaximum = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Value")]
        public int Value
        {
            get { return mValue; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > Maximum)
                    value = Maximum;
                if(mValue==value)
                {
                    return;
                }
                mValue = value;

                int nTrackHeight = (this.Height - (imgHeight + imgHeight));
                float fThumbHeight = ((float)LargeChange / ((float)Maximum+1)) * nTrackHeight;
                int nThumbHeight = (int)fThumbHeight;

                //if (nThumbHeight > nTrackHeight)
                //{
                //    nThumbHeight = nTrackHeight;
                //    fThumbHeight = nTrackHeight;
                //}
                //if (nThumbHeight < 56)
                //{
                //    nThumbHeight = 56;
                //    fThumbHeight = 56;
                //}

                //figure out value
                int nPixelRange = nTrackHeight - nThumbHeight;
                int nRealRange = (Maximum - Minimum);
                float fPerc = 0.0f;
                if (nRealRange != 0)
                {
                    fPerc = (float)mValue / (float)nRealRange;

                }
                float fTop = fPerc * nPixelRange;
                mThumbTop = (int)fTop;
                if (ValueChanged != null)
                    ValueChanged(this, new EventArgs());
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), Category("Skin"), Description("Channel Color")]
        public Color ChannelColor
        {
            get { return mChannelColor; }
            set { mChannelColor = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Image UpArrowImage
        {
            get { return mUpArrowImage; }
            set { mUpArrowImage = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Image DownArrowImage
        {
            get { return mDownArrowImage; }
            set { mDownArrowImage = value; }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            if (UpArrowImage != null)
            {
                e.Graphics.DrawImage(UpArrowImage, new Rectangle(new Point(0, 0), new Size(this.Width, imgHeight)));
            }

            Brush brush = new SolidBrush(mChannelColor);
            Brush borderBrush = new SolidBrush(Color.FromArgb(75, 85, 104));

            //draw channel left and right border colors
            e.Graphics.FillRectangle(borderBrush, new Rectangle(0, imgHeight, 1, (this.Height - imgHeight)));
            e.Graphics.FillRectangle(borderBrush, new Rectangle(this.Width - 1, imgHeight, 1, (this.Height - imgHeight)));

            //draw channel
            e.Graphics.FillRectangle(brush, new Rectangle(1, imgHeight, this.Width - 2, (this.Height - imgHeight)));

            //draw thumb
            int nTrackHeight = (this.Height - (imgHeight + imgHeight));
            float fThumbHeight = ((float)LargeChange / ((float)Maximum + 1)) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;

            if (nThumbHeight > nTrackHeight)
            {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }

            int nTop = mThumbTop;
            nTop += imgHeight;

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(154, 154, 166)), new Rectangle(1, nTop, this.Width - 2, nThumbHeight));


            if (DownArrowImage != null)
            {
                e.Graphics.DrawImage(DownArrowImage, new Rectangle(new Point(0, (this.Height - imgHeight)), new Size(this.Width, imgHeight)));
            }

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Scrollbar_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Scrollbar_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Scrollbar_MouseUp);
            this.MouseClick += new MouseEventHandler(this.Scrollbar_Click);

            this.ResumeLayout(false);
        }

        private void Scrollbar_Click(object sender, MouseEventArgs e)
        {

            Rectangle rec;
            if (this.UpArrowImage != null)
            {
                rec = new Rectangle(0, this.imgHeight, Width, this.Height - (imgHeight + imgHeight));
            }
            else
                rec = new Rectangle(0, 0, Width, Height);
            if (!rec.Contains(new Point(e.X, e.Y)))
            {return; }
            if (this.mThumbDown)
                return;
            else if(e.Y<mThumbTop)
                Value -= this.LargeChange;
            else
            {
                Value += this.LargeChange;
            }

        }

        private void Scrollbar_MouseDown(object sender, MouseEventArgs e)
        {
            Point ptPoint = this.PointToClient(Cursor.Position);
            int nTrackHeight = (this.Height - (imgHeight + imgHeight));
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;

            int nTop = mThumbTop;
            nTop += imgHeight;

            //滑块
            Rectangle thumbrect = new Rectangle(new Point(1, nTop), new Size(this.Width, nThumbHeight));
            if (thumbrect.Contains(ptPoint))
            {
                //hit the thumb
                nClickPoint = (ptPoint.Y - nTop);
                this.mThumbDown = true;
            }

            Rectangle uparrowrect = new Rectangle(new Point(1, 0), new Size(UpArrowImage.Width, imgHeight));
            if (uparrowrect.Contains(ptPoint))
            {

                Value -= SmallChange;
            }

            Rectangle downarrowrect = new Rectangle(new Point(1, imgHeight + nTrackHeight), new Size(UpArrowImage.Width, imgHeight));
            if (downarrowrect.Contains(ptPoint))
            {
                Value += SmallChange;
            }
        }

        private void Scrollbar_MouseUp(object sender, MouseEventArgs e)
        {
            this.mThumbDown = false;
            this.mThumbDragging = false;
        }
        private void Scrollbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (mThumbDown == true)
            {
                this.mThumbDragging = true;
            }

            if (this.mThumbDragging)
            {

                MoveThumb(e.Y);
            }

            if (Scroll != null)
                Scroll(this, new EventArgs());
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if(!this.Visible)
            { return; }
            base.OnMouseWheel(e);
            Value += e.Delta / (-120)*LargeChange;
        }
        private void MoveThumb(int y)
        {
            int nRealRange = Maximum - Minimum;
            int nTrackHeight = (this.Height - (imgHeight + imgHeight));
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;

            int nSpot = nClickPoint;

            int nPixelRange = (nTrackHeight - nThumbHeight);
            if (mThumbDown && nRealRange > 0 && nPixelRange > 0)
            {

                int nNewThumbTop = y - (imgHeight + nSpot);

                if (nNewThumbTop < 0)
                {
                    nNewThumbTop = 0;
                }
                else if (nNewThumbTop > nPixelRange)
                {
                    nNewThumbTop = nPixelRange;
                }
                else
                {
                   
                }

                mThumbTop = nNewThumbTop;
                
                
                //figure out value
                float fPerc = (float)mThumbTop / (float)nPixelRange;
                float fValue = fPerc * (Maximum);
                Value = (int)fValue + 1;

                Application.DoEvents();

                Invalidate();

            }
        }
    }
}
