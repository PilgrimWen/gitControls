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
using static System.Windows.Forms.ComboBox;

namespace SalesCounter.src.custom.control
{
    public class BsItem
    {
        public BsItem(string txt)
        {
            text = txt;
        }
        public string text
        { get; set; }
    }
    public partial class DefaultTabBar : UserControl
    {
        private int _btnCount=3;
        private int _selectedIndex=0;
        private int btnWidth;
        private int _radius=5;
        private List<BsItem> items = new List<BsItem>();
        public int SelectedIndex {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                Invalidate();
            }
        }
        [TypeConverter(typeof(System.ComponentModel.CollectionConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<BsItem> Items
        {
            get
            {
                return items;
            }
        }
        public int BtnCount {
            get
            {
                return _btnCount;
            }
            set
            {
                _btnCount = value;
                btnWidth = Width / _btnCount;
                //if(items.Count<value)
                //{
                //    for(int i = 0; i < value - items.Count; i++)
                //    {
                //        items.Add(new BsItem() {name="button",desc="desc" });
                //    }
                //}
                //else
                //{
                //    for(int i = items.Count; i > BtnCount; i--)
                //    {
                //        items.RemoveAt(i-1);
                //    }
                //}
            }
        }
        public Color BtnColor { get; set; }
        public Color SelectColor { get; set; }
        public int Radius { get { return _radius; }
            set { _radius = value; } }
        private Dictionary<int, Rectangle> recList = new Dictionary<int, Rectangle>();
        public DefaultTabBar()
        {
            InitializeComponent();
            SetStyles();
            items.Clear();
        }
        private void SetStyles()
        {
            base.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            base.UpdateStyles();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawButtons(e.Graphics);
        }
        private void DrawButtons(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            Point p= new Point(0, 0);
            BtnCount = BtnCount;
            for(int i = 0; i < BtnCount; i++)
            {
                Size size;
                
                if (i != BtnCount - 1)
                {
                    size = new Size(btnWidth, Height);
                }
                else
                    size = new Size(Width-btnWidth*(BtnCount-1),Height);
                Rectangle rec = new Rectangle(p, size);
                if (recList.ContainsKey(i))
                    recList[i] = rec;
                else
                    recList.Add(i, rec);
                p.Offset(size.Width,0);
                Brush brush;
                Brush strBrush;
                if (SelectedIndex == i)
                {
                    brush = new SolidBrush(SelectColor);
                    strBrush = new SolidBrush(Color.FromArgb(34, 162, 250));
                }
                else
                {
                    brush = new SolidBrush(BtnColor);
                    strBrush = new SolidBrush(this.ForeColor);
                }
                GraphicsPath path = DrawUtil.GetTopRoundedRectPath(rec, Radius);
                g.FillPath(brush, path);
                ////border
                //Pen pen = new Pen(Color.Black);
                //pen.Alignment = PenAlignment.Inset;
                //g.DrawRectangle(pen, rec);
                //pen.Dispose();
                //字
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                if(i<items.Count)
                g.DrawString(items[i].text, this.Font, strBrush, rec,format);
                strBrush.Dispose();
                brush.Dispose();
            }
            
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Point p = new Point(e.X, e.Y);
            //Point tP = this.PointToClient(p);
            for(int i=0;i<BtnCount;i++)
            {
                if (recList[i].Contains(p))
                {
                    SelectedIndex = i;
                    break;
                }
            }
        }
    }
}
