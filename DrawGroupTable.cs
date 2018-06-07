using SalesBoss.Model;
using SalesBoss.src.scroll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesBoss.src.controls
{
    public class DrawGroupTable<t> : AModule where t: IGroupModel
    {
        public List<MyColumnHeader> Columns { get; private set; }
        public List<TradeInfoByCode> DataSource { get; set; }
        public Font HeaderFont { get; private set; }
        public Color HeaderBackColor { get; private set; }
        public Color HeaderColor { get; private set; }
        public int RowHeight { get; private set; }
        public int HeadHeight { get; set; }
        public Padding CellPadding { get; private set; }
        public Color GridColor { get; private set; }
        public float GridWidth { get; private set; }
        public bool IsDrawHGrid { get; private set; }
        public StringFormat StringFormat { get; set; }

        public ScrollY ScrollY { get; set; }

        public ScrollX ScrollX { get; set; }
 
        float ScrolledWidth => 0;
        Control P;
        private List<float> cxColWidths;
        public DrawGroupTable(Control control,Rectangle rectangle):base(control)
        {
            P = control;
            ClientRectangle = rectangle;
            Init();
        }

        private void Init()
        {
            IsDrawHGrid = false;
            CellPadding = new Padding(0, 0, 0, 0);

            GridColor = Color.Black;
            StringFormat = StringFormates.MiddleCenter;

            cxColWidths = new List<float>();
            StringFormat = StringFormates.MiddleCenter;
            Columns = new List<MyColumnHeader>();
            HeaderColor = Color.Black;
            HeaderFont = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold,
               System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            HeaderBackColor = Color.Wheat;
            RowHeight = 23;
            HeadHeight = 26;
            GridWidth = 1F;
            ScrollY = new ScrollY(P);
            ScrollY.ScrollOffset = HeadHeight;
            ScrollY.ScrollWidth = 5;
            ScrollY.CalcHeight = () =>
            {
                var ret = ContentHeigh;
              
                return ret;
            };
        }
        private void ResetScroll()
        {
            var padding = ScrollY.Padding;
            padding.Top = ClientRectangle.Top;
            padding.Bottom = Control.ClientRectangle.Bottom - ClientRectangle.Bottom;
            padding.Right = Control.ClientRectangle.Right - ClientRectangle.Right;
            padding.Left = ClientRectangle.Left
                - Control.ClientRectangle.Left +300;

            padding.Left = ClientRectangle.Left - Control.ClientRectangle.Left;
 
            padding.Top += RowHeight;

            ScrollY.ResetScrollY(padding);
        }
 

        public override void Draw(Graphics g, Rectangle rect)
        {
            Slided = -(int)ScrollY.ScrolledHeight;
            ClientRectangle = rect;
            ResetScroll();
            foreach (var col in Columns)
            {
                cxColWidths.Add(col.Width);
            }
            DrawHeader(g);
            DrawItems(g,rect);
            ScrollY.Draw(g, ClientRectangle);
        }
        private int ContentHeigh;
        private List<GroupSource<t>> datasource;
        public List<GroupSource<t>> DataS {
            get { return datasource; }
            set { datasource = value;
                ContentHeigh = 0;
                foreach(var i in datasource)
                {
                    ContentHeigh += HeadHeight+(1+i.Items.Count)*RowHeight;
                }
            }
            
        }
        private int  _slide=0;
        public int Slided
        {
            get { return _slide;  }
            set {
                if ( value!= _slide)
                {
                    _slide = value;
                    Invalidate(new Rectangle(ClientRectangle.X, ClientRectangle.Y + HeadHeight, ClientRectangle.Width, ClientRectangle.Height - HeadHeight));
                }
            }
        }
        
        private void DrawItems(Graphics g, Rectangle rect)
        {
            Rectangle rowRec = new Rectangle(rect.Left,rect.Top+HeadHeight,rect.Width,HeadHeight);
            rowRec.Y -= Slided;
            if (DataS == null)
                return;
            foreach(var c in DataS)
            {
                List<string> content = c.Content;

                if (rowRec.Y > rect.Bottom - RowHeight)
                {
                    return;
                }
                //content
                DrawRow(g,rowRec,RowType.HeadItem,content);
                rowRec.Y += HeadHeight;

                Rectangle rectangle = new Rectangle(rect.Left + 5, rowRec.Top, rect.Width - 10, (c.Items.Count + 1) * RowHeight);
                //row head
                rowRec = new Rectangle(rowRec.X+5,rowRec.Top,rowRec.Width-10,RowHeight);
                DrawRow(g, rowRec, RowType.ItemHead,c.ItemCol);

                //rows
                rowRec.Y += RowHeight;
                foreach(var item in c.Items)
                {
                    List<string> iList = item.ToList();
                    DrawRow(g, rowRec, RowType.Item,iList);
                    rowRec.Y += RowHeight;
                }
                if(rectangle.Bottom>rect.Top+HeadHeight&&rect.Bottom>rectangle.Bottom)
                {
                    g.DrawLine(Pens.Gray,new Point(rectangle.Right,rectangle.Bottom),new Point(rectangle.Left,rectangle.Bottom));
                }
                if (rectangle.Top > rect.Top + HeadHeight && rect.Bottom > rectangle.Top)
                {
                    g.DrawLine(Pens.Gray, new Point(rectangle.Right, rectangle.Top), new Point(rectangle.Left, rectangle.Top));
                }
                //g.DrawRectangle(Pens.Gray, rectangle);

                rowRec.Offset(-5, 0);
                rowRec.Width += 10;
                rowRec.Height = HeadHeight;
            }
        }
        private void DrawRow(Graphics g,Rectangle rec,RowType rowType,List<string> list)
        {
            float left = rec.Left;
            float top = rec.Top;
            Rectangle realRec=rec;
            var txtHeight = 1 + (int)g.MeasureString("0", HeaderFont).Height;
            int cellWidth = rec.Width / Columns.Count;
            int remainder = rec.Width % Columns.Count;
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            if (rec.Bottom-ClientRectangle.Top- HeadHeight <= 0)
            {
                return;
            }
            if (rec.Top >= ClientRectangle.Bottom)
                return;
            if (rec.Bottom - ClientRectangle.Top-HeadHeight < RowHeight)
            {
                realRec = new Rectangle(rec.X,ClientRectangle.Top+HeadHeight,rec.Width,rec.Bottom- ClientRectangle.Top - HeadHeight);
            }
            if(ClientRectangle.Bottom- rec.Top < RowHeight)
            {
                realRec = new Rectangle(rec.X, rec.Top, rec.Width, ClientRectangle.Bottom - rec.Top);
            }
            switch (rowType)
            {
                case RowType.Item:
                    g.DrawLine(Pens.Gray,new Point(realRec.Left,realRec.Bottom),new Point(realRec.Left,realRec.Top));
                    g.DrawLine(Pens.Gray, new Point(realRec.Right, realRec.Bottom), new Point(realRec.Right, realRec.Top));
                    break;
                case RowType.ItemHead:
                    g.FillRectangle(Brushes.BurlyWood, realRec);
                    g.DrawLine(Pens.Gray, new Point(realRec.Left, realRec.Bottom), new Point(realRec.Left, realRec.Top));
                    g.DrawLine(Pens.Gray, new Point(realRec.Right, realRec.Bottom), new Point(realRec.Right, realRec.Top));
                    break;
                case RowType.HeadItem:
                    break;
            }

            using (var txtBrush = new SolidBrush(Color.Black))
            {
                for (var i = 0; i < Columns.Count; i++)
                {
                   
                    var txtRect = new RectangleF(
                        CellPadding.Left + left,
                        top + (rec.Height - txtHeight) / 2 + 1,
                        cellWidth,
                        txtHeight);
                    if (i < remainder)
                    {
                        txtRect.Width += 1;
                    }
                    if (i < Columns.Count)
                    {
                        if (txtRect.Top-ClientRectangle.Top-HeadHeight<0)
                        {
                            stringFormat.LineAlignment = StringAlignment.Far;
                            txtRect.Height = txtHeight-realRec.Top+rec.Top;
                            txtRect.Y += realRec.Top- rec.Top-2;
                        }
                        if (txtRect.Bottom > ClientRectangle.Bottom)
                        {
                            stringFormat.LineAlignment = StringAlignment.Near;
                            txtRect.Height = ClientRectangle.Bottom - txtRect.Top;
                        }
                        if (txtRect.Height <= 1)
                            return;
                        g.DrawString(list[i], HeaderFont, txtBrush, txtRect, stringFormat);
                    }
                    left += txtRect.Width;
                }
            }
        }
        private void DrawHeader(Graphics g)
        {

                DrawTitle(g, ClientRectangle.Top);
                using (var pen = new Pen(GridColor, GridWidth))
                {
                    float left = ClientRectangle.Left;
                    if (IsDrawHGrid)
                        g.DrawLine(pen, left, ClientRectangle.Top + HeadHeight, ClientRectangle.Right, ClientRectangle.Top + HeadHeight);
                }

        }
        private void DrawTitle(Graphics g, float top)
        {
            try
            {
                float left = ClientRectangle.Left;
                int cellWidth = ClientRectangle.Width / Columns.Count;
                int remainder = ClientRectangle.Width % Columns.Count;
                var txtHeight = 1 + (int)g.MeasureString("0", HeaderFont).Height;
                using (var backBrush = new SolidBrush(HeaderBackColor))
                using (var brush = new SolidBrush(HeaderColor))
                {
                    g.FillRectangle(backBrush, left, top, ClientRectangle.Width, HeadHeight);
 
                    var maxsRight = ClientRectangle.Right;

                    for (var i = 0; i < Columns.Count; i++)
                    {
                        var stringFormat = Columns[i].StringFormat ?? StringFormat;
                        var txtRect = new RectangleF(
                            CellPadding.Left + left,
                            top + (RowHeight - txtHeight) / 2 + 1,
                            cellWidth,
                            txtHeight);
                        txtRect.Offset(ScrolledWidth, 0);
                        if (i < remainder)
                        {
                            txtRect.Width += 1;
                        }
                        if (txtRect.Left > maxsRight)
                        {
                            left += cellWidth;
                            continue;
                        }
                        if (i < Columns.Count)
                        {
                            g.DrawString(Columns[i].Text, HeaderFont, brush, txtRect, stringFormat);
                        }
                        left += txtRect.Width;
                    }
                    left = ClientRectangle.Left;

                }
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.StackTrace);
#endif
            }
        }
        
    }
    public enum RowType
    {
        HeadItem,ItemHead,Item
    }
    public class GroupSource<t>
    {
        public List<string> Content;
        public List<string> ItemCol;
        public List<t> Items;
    }
    public class MyColumnHeader
    {

        public string Name { get; set; }

        public string Text { get; set; }

        public float Width { get; set; }

        public float Min { get; set; }

        public bool IsDrawCustom { get; set; }

        public StringFormat StringFormat { get; set; }

        public Color Color { get; set; }

        public MyColumnHeader(string name, float width = 100)
        {
            Name = name;
            Text = name;
            Width = width;
        }

        public MyColumnHeader(string name, string text, float width = 100) : this(name, width)
        {
            Text = text;
        }

        public MyColumnHeader(string name, string text, float width = 100, float min = 100) : this(name, text, width)
        {
            Min = min;
        }
    }
}
