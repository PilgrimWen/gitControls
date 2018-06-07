using SalesBoss.src.utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SalesBoss.src.controls
{
    /// <summary>
    /// 自绘制按钮
    ///     支持:
    ///         常规，鼠标移动及聚焦的默认样式和背景 或 可以自定义样式和背景
    ///         支持图片+文字 现有排列方式 图左字右，图上字下，及图中
    ///         支持点击
    ///     不支持
    ///         系统默认聚焦，需自己手动编码
    /// </summary>
    public class DrawButton : AModule
    {
        private string text;

        public string Text
        {
            get { return text; }
            set
            {
                if (!text.Equals(value))
                {
                    text = value;
                    Invalidate();
                }
            }
        }

        private bool focused;

        public bool Focused
        {
            get => focused;
            set
            {
                if (value != focused)
                {
                    focused = value;
                    Invalidate(ClientRectangle);
                }
            }
        }

        private bool mouseEntered;

        public bool IsFillet { get; set; }

        public bool MouseEntered
        {
            get => mouseEntered;
            set
            {
                if (value != mouseEntered)
                {
                    mouseEntered = value;
                    Invalidate(ClientRectangle);
                }
            }
        }

        public Padding Padding { get; set; }

        public Color MouseEnteredBgColor { get; set; }

        public Color FocusedBgColor { get; set; }

        public Color ForeColor { get; set; }

        public Color MouseEnteredFrColor { get; set; }

        public Color FocusedFrColor { get; set; }

        public Font Font { get; set; }

        public Font MouseEnteredFont { get; set; }

        public Font FocusedFont { get; set; }

        public Image Icon { get; set; }

        public Image MouseEnterIcon { get; set; }

        public StringFormat TextFormat { get; set; }

        public Action<DrawButton, Graphics, Rectangle, Color> CustomFocusedBg { get; set; }

        public Action<DrawButton, Graphics, Rectangle, Color> CustomBg { get; set; }

        public Action<DrawButton, Graphics, Rectangle, Color> CustomEnterBg { get; set; }

        public ContentAlignment PicAlign { get; set; }

        public event EventHandler<EventArgs> Click;

        public void OnClick(EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public DrawButton(Control control) : base(control)
        {
            BackColor = UIColors.CrBtnBack;
            Init();
        }

        public void Init()
        {
            Focused = false;
            IsFillet = true;
            text = string.Empty;
            Font = Control.Font;
            Padding = new Padding(0, 0, 0, 0);

            MouseEnteredFont = Font;
            FocusedFont = Font;
            TextFormat = StringFormates.MiddleCenter;
            MouseEnteredBgColor = UIColors.CrBtnMouseEntered;
            FocusedBgColor = UIColors.CrBtnFocused;

            ForeColor = Color.WhiteSmoke;
            MouseEnteredFrColor = ForeColor;
            FocusedFrColor = ForeColor;

            PicAlign = ContentAlignment.TopCenter;
            Control.MouseMove += Control_MouseMove;
            Control.MouseClick += Control_MouseClick;
        }

        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            if (ClientRectangle.Contains(e.Location))
            {
                Click?.Invoke(this, e);
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (ClientRectangle.Contains(e.Location))
            {
                MouseEntered = true;
            }
            else
                MouseEntered = false;
        }

        public override void Draw(Graphics g, Rectangle rect)
        {
            ClientRectangle = rect;
            if (rect.Height <= 0 || rect.Width <= 0) return;
            var font = Font;
            var foreColor = ForeColor;
            var bgColor = BackColor;
            var icon = Icon;
            var left = ClientRectangle.Left;
            var top = ClientRectangle.Top;
            var right = ClientRectangle.Right;
            var bottom = ClientRectangle.Bottom;
            var txtHeight = 1 + (int)g.MeasureString("0华", Font).Height;
            //var path = DrawUtil.DrawRoundRect(left + 0.5F, top + 0.5F, right - left - 1.5F, bottom - top - 1.5F, 4F);
            var isDrawBg = false;
            if (Focused)
            {
                font = FocusedFont;
                foreColor = FocusedFrColor;
                bgColor = FocusedBgColor;
                if (null != CustomFocusedBg)
                {
                    CustomFocusedBg(this, g, ClientRectangle, bgColor);
                    isDrawBg = true;
                }
            }
            else if (MouseEntered)
            {
                font = MouseEnteredFont;
                foreColor = MouseEnteredFrColor;
                bgColor = MouseEnteredBgColor;
                if (null != MouseEnterIcon)
                    icon = MouseEnterIcon;
                if (!isDrawBg && null != CustomEnterBg)
                {
                    CustomEnterBg(this, g, ClientRectangle, bgColor);
                    isDrawBg = true;
                }
            }
            else if (!isDrawBg && null != CustomBg)
            {
                CustomBg(this, g, ClientRectangle, bgColor);
                isDrawBg = true;
            }
            if (!isDrawBg)
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    if (IsFillet)
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        var gPath = new RectangleF(ClientRectangle.Left,
                     ClientRectangle.Top, ClientRectangle.Width, ClientRectangle.Height);
                        if (null != gPath)
                            g.FillRectangle(brush, gPath);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                    }
                    else
                    {
                        g.FillRectangle(brush, ClientRectangle);
                    }
                }
                if (Focused)
                {
                    using (var p = new Pen(Color.DarkGray)
                    {
                        DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
                    })
                        g.DrawRectangle(p, left + 2, top + 2, ClientRectangle.Width - 4, ClientRectangle.Height - 4);
                }
            }
            RectangleF txtRect = ClientRectangle;
            txtRect.X += Padding.Left;
            txtRect.Width -= Padding.Horizontal;
            txtRect.Y += Padding.Top + (ClientRectangle.Height - txtHeight) / 2F + 1;
            txtRect.Height = txtHeight;
            if (null != icon)
            {
                switch (PicAlign)
                {
                    case ContentAlignment.TopCenter:
                        {
                            var picRect = new RectangleF(
                                    left + ClientRectangle.Width * 0.25F,
                                    top + ClientRectangle.Height * 0.15F,
                                    ClientRectangle.Width * 0.5F,
                                    ClientRectangle.Height * 0.5F
                                );
                            g.DrawImage(Icon, picRect);
                            txtRect = new RectangleF(
                                    left,
                                    picRect.Bottom,
                                    ClientRectangle.Width,
                                    ClientRectangle.Height * 0.35F
                                );
                            using (var brush = new SolidBrush(foreColor))
                                g.DrawString(Text, font, brush, txtRect, StringFormates.MiddleCenter);
                        }
                        break;
                    case ContentAlignment.MiddleLeft:
                        {
                            var picRect = new RectangleF(
                                        left + ClientRectangle.Height * 0.15F,
                                        top + ClientRectangle.Height * 0.15F,
                                        ClientRectangle.Height * 0.7F,
                                        ClientRectangle.Height * 0.7F
                                    );
                            g.DrawImage(Icon, picRect);
                            txtRect.X += ClientRectangle.Height;
                            txtRect.Width -= ClientRectangle.Height;
                            using (var brush = new SolidBrush(foreColor))
                                g.DrawString(Text, font, brush, txtRect, StringFormates.MiddleLeft);
                        }
                        break;
                    case ContentAlignment.MiddleCenter:
                        {
                            var picRect = new RectangleF(
                                        left + ClientRectangle.Width * 0.1F,
                                        top + ClientRectangle.Height * 0.1F,
                                        ClientRectangle.Width * 0.8F,
                                        ClientRectangle.Height * 0.8F
                                    );
                            g.DrawImage(Icon, picRect);
                        }
                        break;
                    default:
                        break;
                        //return;
                }
            }
            else
            {
                using (var brush = new SolidBrush(foreColor))
                    g.DrawString(Text, font, brush, txtRect, TextFormat);
            }

        }

    }
}
