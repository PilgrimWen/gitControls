using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using SalesCounter.src.utils;

namespace SalesCounter.src.custom.control
{
    public enum HPType { HP,SP}
    public partial class DefaultHP : UserControl
    {
        int value = 0;
        [Browsable(true), DefaultValue(typeof(int), "0")]
        [Category("Value")]
        public int Value {
        get
            {
                return value;
            }
        set
            {
                if (value > 100)
                { value = 100; }
                else if (value < 0)
                    value = 0;
                this.value = value;
            }
        }
        public HPType hpType
        {
            get;
            set;
        }
        public DefaultHP()
        {
            InitializeComponent();
            hpType = HPType.HP;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Image image;
            if (hpType == HPType.HP)
            {
                image = Resource1.icon_个人中心_进度条_红;
            }
            else
                image = ResControl.icon_个人中心_进度条_绿;
            GraphicsPath path = DrawUtil.GetFootBallRectPath(new Rectangle(0, 0,Width*value/100 ,Height-1));
            GraphicsPath borderpath = DrawUtil.GetFootBallRectPath(new Rectangle(0, 0, Width-1 , Height - 1));
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.High;
            using (TextureBrush Txbrus = new TextureBrush(image))
            {
                Txbrus.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
                //g.FillRectangle(Txbrus, new Rectangle(0,0,100,57));
                g.FillPath(Txbrus, path);
                g.DrawPath(Pens.White, borderpath);
                //g.DrawImage(image, new Point(0, 0));
            }
        }
    }
}
