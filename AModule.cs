using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesBoss.src.controls
{
    /// <summary>
    /// 基本的绘画模块抽象类
    /// 双缓冲
    /// 重绘区域
    /// </summary>
    public abstract class AModule 
    {
        public AModule(Control control)
        {
            Control = control;
            BackColor = control.BackColor;
        }

        public Color BackColor { get; set; }

        public Control Control { get; set; }

        public Rectangle ClientRectangle { get; set; }

        public abstract void Draw(Graphics g, Rectangle rect);

        public void DoubleBufferDraw(Graphics g, Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;
            using (var backBush = new SolidBrush(Control.BackColor))
            {
                BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
                BufferedGraphics bg = currentContext.Allocate(g, rect);
                var gBuffer = bg.Graphics;
                gBuffer.FillRectangle(backBush, rect);
                Draw(gBuffer, rect);
                bg.Render(g);
                bg.Dispose();
            }
        }

        public virtual void DrawBackground(Graphics g)
        {

        }

        protected void Invalidate(Rectangle rect)
        {
            Control.Invalidate(rect);
        }

        public void Invalidate()
        {
            Control.Invalidate(ClientRectangle);
        }

    }

}
