using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesBoss.src.controls
{
    class DrawSimpleButton : AModule
    {
        private bool pressed=false;
        public bool Round { get; set; }
        public bool Focused
        {
            get { return pressed; }
            set
            {
                pressed = value;
                Control.Invalidate(ClientRectangle);
            }
        }
        public Color ForeColor { get; set; }
        public Color BorderColor { get; set; }
        public Color FocusedBorderColor { get; set; }
        public string Text { get; set; }
        public bool Bordered { get; set; }
        public Font BtnFont { get; set; }
        public List<DrawSimpleButton> Groups { get; set; }
        public event EventHandler<EventArgs> Click;
        public DrawSimpleButton(Control control):base(control)
        {
            Control.MouseClick += Control_MouseClick;
            Groups = new List<DrawSimpleButton>();
            InitGroupBtn();
        }
        private void InitGroupBtn()
        {
            ForeColor = Color.Black;
            BackColor = Color.White;
            var font = new System.Drawing.Font("微软雅黑",
                11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            BtnFont = font;
            BorderColor = Color.Black;
            FocusedBorderColor = Color.FromArgb(237, 164, 38);
            Bordered = true;
            Round = false;
        }
        public override void Draw(Graphics g, Rectangle rect)
        {
            ClientRectangle = rect;
            Color txtColor = ForeColor;
            Pen p;
            if (!Focused)
            {
                p = new Pen(BorderColor);
                txtColor = ForeColor;
            }
            else
            {
                p = new Pen(FocusedBorderColor);
                txtColor = FocusedBorderColor;
            }
            using (var brush = new SolidBrush(BackColor))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                if (!Round)
                    g.FillRectangle(brush, rect);
                else
                    g.FillPath(brush, GetRoundRect(rect, 5));
            }
            rect.Width -=1;
            rect.Height -= 1;

            if (Bordered)
                g.DrawRectangle(p, rect);
            
            rect.Width += 1;
            rect.Height += 1;
            var txtHeight = 1 + (int)g.MeasureString("0华", BtnFont).Height;
            RectangleF txtRect = ClientRectangle;
            txtRect.Y += (ClientRectangle.Height - txtHeight) / 2F + 1;
            txtRect.Height = txtHeight;
            using (var brush = new SolidBrush(txtColor))
                g.DrawString(Text, BtnFont, brush, txtRect, StringFormates.MiddleCenter);
        }
        public  GraphicsPath GetRoundRect(RectangleF rect, float radius)
        {
            var gp = new GraphicsPath();
            var diameter = radius;
            gp.AddArc(rect.X + rect.Width - diameter-1, rect.Y, diameter, diameter, 270, 90);
            gp.AddArc(rect.X + rect.Width - diameter-1, rect.Y + rect.Height - diameter-1, diameter, diameter, 0, 90);
            gp.AddArc(rect.X, rect.Y + rect.Height - diameter-1, diameter, diameter, 90, 90);
            gp.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            gp.CloseFigure();
            return gp;
        }
        public  GraphicsPath GetFootBallRectPath(Rectangle rect)
        {
            if (rect == null && rect.IsEmpty)
                return null;
            Rectangle arcRect = new Rectangle(rect.Left, rect.Top, rect.Height, rect.Height);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(arcRect, 90, 180);
            arcRect.X += rect.Width - rect.Height;
            path.AddArc(arcRect, 270, 180);
            path.CloseFigure();
            return path;
        }
        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (var i in Groups)
            {
                if (i.ClientRectangle.Contains(e.Location))
                {
                    Focused = false;
                }
            }
            if (ClientRectangle.Contains(e.Location))
            {
                Focused = true;
                Click?.Invoke(this, e);
            }
        }

        public void OnSomeoneClick(object sender, EventArgs e)
        {
            Focused = false;
        }
    }
}
