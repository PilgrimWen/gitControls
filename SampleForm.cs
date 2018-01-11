using SalesCounter.src.custom.control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesCounter.src.custom.form
{
    public class SampleForm : Form
    {

        private Point mPoint;
        private Rectangle maxRec;
        private Rectangle minRec;
        private Rectangle closeRec;
        private bool change;

        private string _title;
        public Image maxImage = ResControl.icon_窗口最大化_normal;
        public Image minImage = ResControl.icon_窗口最小化_normal;
        public Image closeImage = ResControl.icon_关闭窗口_normal;
        [Browsable(true), Category("Appearance")]
        public string Title { get => _title; set => _title = value; }
        [Browsable(true), Category("Appearance")]
        public bool AllowMaximized { get; set; }
        public bool AllowMinimized { get; set; }
        public bool AllowResize { get; set; }
        public SampleForm() : base()
        {

            SetStyle(ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.ResizeRedraw |
               ControlStyles.SupportsTransparentBackColor, true);
            //this.SuspendLayout();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.ResumeLayout(false);
            this.Font = new Font("PingFangSC - Semibold", 16);
            this.AllowMaximized = true;
            this.AllowResize = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ControlBox = false;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width < 80)
                Width = 50;
            if (Height < 50)
                Height = 50;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            if (AllowMaximized)
            {
                maxRec = new Rectangle(this.Width - 60, 12, 22, 19);
                minRec = new Rectangle(this.Width - 88, 12, 22, 19);
                g.DrawImage(maxImage, maxRec);
                g.DrawImage(minImage, minRec);
            }
            else if(AllowMinimized)
            {
                minRec = new Rectangle(this.Width - 60, 12, 22, 19);
                g.DrawImage(minImage, minRec);
            }
            closeRec = new Rectangle(this.Width - 32, 12, 22, 19);
            if (ShowIcon)
            {
                Icon ico = Icon;
                Rectangle icoRec = new Rectangle(8, 8, 23, 23);
                g.DrawIcon(ico, icoRec);
            }
            g.DrawImage(closeImage, closeRec);
            g.DrawString(Text, Font, new SolidBrush(this.ForeColor), new Point(35, 9));
            if (!string.IsNullOrEmpty(Title))
            {
                var font = new Font("PingFangSC-Semibold", 18, GraphicsUnit.Pixel);
                Rectangle rec = new Rectangle(0, 7, this.Width, 40-7);
                StringFormat format = new StringFormat(StringFormatFlags.LineLimit);
                format.Alignment = StringAlignment.Center;
                g.DrawString(Title, font, new SolidBrush(this.ForeColor), rec, format);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }


        #region 无边框窗体移动、放大、缩小
        const int Guying_HTLEFT = 10;
        const int Guying_HTRIGHT = 11;
        const int Guying_HTTOP = 12;
        const int Guying_HTTOPLEFT = 13;
        const int Guying_HTTOPRIGHT = 14;
        const int Guying_HTBOTTOM = 15;
        const int Guying_HTBOTTOMLEFT = 0x10;
        const int Guying_HTBOTTOMRIGHT = 17;
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0084:
                    base.WndProc(ref m);
                    //获取鼠标位置
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                        (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (AllowResize)
                    {
                        if (vPoint.X <= 5)
                            if (vPoint.Y <= 5)
                                m.Result = (IntPtr)Guying_HTTOPLEFT;
                            else if (vPoint.Y >= ClientSize.Height - 5)
                                m.Result = (IntPtr)Guying_HTBOTTOMLEFT;
                            else m.Result = (IntPtr)Guying_HTLEFT;
                        else if (vPoint.X >= ClientSize.Width - 5)
                            if (vPoint.Y <= 5)
                                m.Result = (IntPtr)Guying_HTTOPRIGHT;
                            else if (vPoint.Y >= ClientSize.Height - 5)
                                m.Result = (IntPtr)Guying_HTBOTTOMRIGHT;
                            else m.Result = (IntPtr)Guying_HTRIGHT;
                        else if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOP;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOM;
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mPoint = new Point(e.X, e.Y);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point p = e.Location;

            if (minRec.Contains(p))
            {
                minImage = ResControl.icon_窗口最小化;
                Invalidate(minRec);
                change = true;
            }
            else if (maxRec.Contains(p))
            {
                maxImage = ResControl.icon_窗口最大化;
                Invalidate(maxRec);
                change = true;
            }
            else if (closeRec.Contains(p))
            {
                closeImage = ResControl.icon_关闭窗口;
                Invalidate(closeRec);
                change = true;
            }
            else
            {
                if (change)
                {
                    maxImage = ResControl.icon_窗口最大化_normal;
                    minImage = ResControl.icon_窗口最小化_normal;
                    closeImage = ResControl.icon_关闭窗口_normal;
                    change = false;
                    Invalidate();
                }
            }
            if (e.Button == MouseButtons.Left)
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
        }
        #endregion
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Point p = e.Location;
            if (minRec.Contains(p))
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else if (maxRec.Contains(p))
            {
                this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                Invalidate();
            }
            else if (closeRec.Contains(p))
            {
                this.Close();
            }
        }
        private GraphicsPath GetTopRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            // 左上角
            path.AddArc(arcRect, 180, 90);

            // 右上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // 右下角
            arcRect.Y = rect.Bottom - diameter;
            path.AddLine(new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Top - diameter));
            path.AddLine(new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Bottom));
            // 左下角
            arcRect.X = rect.Left;
            path.AddLine(new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top - diameter));
            path.CloseFigure();//闭合曲线
            return path;
        }
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            // 左上角
            path.AddArc(arcRect, 180, 90);

            // 右上角
            arcRect.X = rect.Right - diameter - 1;
            path.AddArc(arcRect, 270, 90);

            // 右下角
            arcRect.Y = rect.Bottom - diameter - 1;
            path.AddArc(arcRect, 0, 90);

            // 左下角
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();//闭合曲线
            return path;
        }
    }
}
