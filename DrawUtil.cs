using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBoss.src.utils
{
    public static class DrawUtil
    {
        public static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var ret = new GraphicsPath();
            var arcRect = new Rectangle(rect.Location, new Size(radius, radius));

            ret.AddArc(arcRect, 180, 90);

            arcRect.X = rect.Right - radius;
            ret.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - radius;
            ret.AddArc(arcRect, 0, 90);

            arcRect.X = rect.Left;
            ret.AddArc(arcRect, 90, 90);
            ret.CloseFigure();
            return ret;
        }

        public static GraphicsPath GetRectPath(Rectangle rect, int radius)
        {
            var ret = new GraphicsPath();
            Point[] pts = new Point[8];
            pts[0] = new Point(rect.Left, rect.Bottom - radius);
            pts[1] = new Point(rect.Left, rect.Top + radius);
            pts[2] = new Point(rect.Left + radius, rect.Top);
            pts[3] = new Point(rect.Right - radius, rect.Top);
            pts[4] = new Point(rect.Right, rect.Top + radius);
            pts[5] = new Point(rect.Right, rect.Bottom - radius);
            pts[6] = new Point(rect.Right - radius, rect.Bottom);
            pts[7] = new Point(rect.Left + radius, rect.Bottom);
            ret.AddPolygon(pts);
            return ret;
        }
        public static GraphicsPath GetTopRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            if (rect == null && rect.IsEmpty)
                return null;
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
        public static GraphicsPath GetLeftRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            if (rect == null && rect.IsEmpty)
                return null;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            // 左上角
            path.AddArc(arcRect, 180, 90);

            // 右上角
            arcRect.X = rect.Right - diameter;
            path.AddLine(new Point(rect.Left - radius, rect.Top), new Point(rect.Right, rect.Top));
            path.AddLine(new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom));

            // 右下角
            arcRect.Y = rect.Bottom - diameter;
            path.AddLine(new Point(rect.Right, rect.Bottom), new Point(rect.Left - radius, rect.Bottom));

            // 左下角
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();//闭合曲线
            return path;
        }
        public static GraphicsPath GetRightRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            if (rect == null && rect.IsEmpty)
                return null;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            // 左上角
            path.AddLine(new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top));
            path.AddLine(new Point(rect.Left, rect.Top), new Point(rect.Right - radius, rect.Top));

            // 右上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // 右下角
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            // 左下角
            arcRect.X = rect.Left;
            path.AddLine(new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top - diameter));
            path.CloseFigure();//闭合曲线
            return path;
        }
        public static GraphicsPath GetFootBallRectPath(Rectangle rect)
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
        /// <summary>
        /// 获取button的圆角path
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static GraphicsPath GetRoundRect(RectangleF rect, float radius)
        {
            var gp = new GraphicsPath();
            var diameter = radius;
            gp.AddArc(rect.X + rect.Width - diameter - 1, rect.Y, diameter, diameter, 270, 90);
            gp.AddArc(rect.X + rect.Width - diameter - 1, rect.Y + rect.Height - diameter - 1, diameter, diameter, 0, 90);
            gp.AddArc(rect.X, rect.Y + rect.Height - diameter - 1, diameter, diameter, 90, 90);
            gp.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            gp.CloseFigure();
            return gp;
        }
        public static Image CutEllipse(Image img, Rectangle rec, Size size)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (TextureBrush br = new TextureBrush(img, System.Drawing.Drawing2D.WrapMode.Clamp, rec))
                {
                    br.ScaleTransform(bitmap.Width / (float)rec.Width, bitmap.Height / (float)rec.Height);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.FillEllipse(br, new Rectangle(Point.Empty, size));
                }
            }
            return bitmap;
        }
        public static Bitmap CutHead(Image img)
        {
            Bitmap b = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                using (System.Drawing.Drawing2D.GraphicsPath p = new System.Drawing.Drawing2D.GraphicsPath(System.Drawing.Drawing2D.FillMode.Alternate))
                {
                    p.AddEllipse(0, 0, img.Width, img.Height);
                    g.FillPath(new TextureBrush(img), p);
                }
                //g.FillEllipse(new TextureBrush(i), 0, 0, i.Width, i.Height);
            }
            return b;
        }

        public static void DrawText(Graphics g, Rectangle rec, string content,Font font)
        {
            var format = new StringFormat(StringFormat.GenericDefault) { Trimming = StringTrimming.EllipsisCharacter };
            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            //format.FormatFlags ^= StringFormatFlags.LineLimit;

            SizeF size = g.MeasureString(content, font, new Size(rec.Width - 2, rec.Height - 2), format);
            using (var sb = new SolidBrush(Color.Black))
                g.DrawString(content, font, sb, rec, format);
        }
    }
}
