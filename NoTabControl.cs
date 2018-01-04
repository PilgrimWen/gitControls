using SalesCounter.src.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesCounter.src.custom.control
{
    public class NoTabControl:TabControl
    {

        private System.ComponentModel.Container components = null;
        private const int nMargin = 5;
        private Color mBackColor = SystemColors.Control;
        public NoTabControl()
        {
            InitializeComponent();

            // double buffering
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawControl(e.Graphics);
        }
        private void DrawControl(Graphics g)
        {
            if (!Visible)
                return;
            Rectangle TabControlArea = this.ClientRectangle;
            Rectangle TabArea = this.DisplayRectangle;

            //----------------------------
            // fill client area
            Brush br = new SolidBrush(Color.White); //(SystemColors.Control); UPDATED
            g.FillRectangle(br, TabControlArea);
            br.Dispose();
            //----------------------------

            //----------------------------
            // draw border
            int nDelta = SystemInformation.Border3DSize.Width;

            Pen border = new Pen(SystemColors.ControlDark);
            TabArea.Inflate(nDelta, nDelta);
            //g.DrawRectangle(border, TabArea);
            border.Dispose();
            //----------------------------

            //----------------------------
            // clip region for drawing tabs
            Region rsaved = g.Clip;
            Rectangle rreg;

            int nWidth = TabArea.Width + nMargin;

            rreg = new Rectangle(TabArea.Left, TabControlArea.Top, nWidth - nMargin, TabControlArea.Height);

            g.SetClip(rreg);

            // draw tabs
            //for (int i = 0; i < this.TabCount; i++)
            //    DrawTab(g, this.TabPages[i], i);

            g.Clip = rsaved;
            //----------------------------

            //----------------------------
            // draw background to cover flat border areas
            if (this.SelectedTab != null)
            {
                TabPage tabPage = this.SelectedTab;
                Color color = tabPage.BackColor;
                border = new Pen(color);

                TabArea.Offset(1, 1);
                TabArea.Width -= 2;
                TabArea.Height -= 2;

                g.DrawRectangle(border, TabArea);
                TabArea.Width -= 1;
                TabArea.Height -= 1;
                g.DrawRectangle(border, TabArea);

                border.Dispose();
            }
            //----------------------------
        }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }


        [Browsable(false)]
        new public TabSizeMode SizeMode
        {
            get { return base.SizeMode; }
            set { base.SizeMode = TabSizeMode.Fixed; }
        }
        new public Size ItemSize
        {
            get { return base.ItemSize; }
            set { base.ItemSize = new Size(100,50); }
        }

    }
}
