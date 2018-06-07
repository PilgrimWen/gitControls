using SalesBoss.src.scroll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YSTZUI.src.extends.eventargs;

namespace SalesBoss.src.controls
{
    /// <summary>
    /// 自绘制表格
    ///     支持
    ///         支持行，单元格自定义及默认绘制
    ///         支持行，单元格的前景色，背景色的自定义及默认绘制
    ///         支持上下滚动，左右滚动
    ///         支持单选
    ///         支持单选，鼠标移动行颜色变化
    ///         支持点击单元格时间
    ///         支持表头，表尾
    ///         支持网格绘制
    ///         支持表格部分列作为列头
    ///         支持自适应长度        
    /// 
    ///     不支持
    ///         暂不支持按键左右滚动
    ///         暂不支持多选，单选上下移动需自己编码
    ///         暂不支持改变列长度，列排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DrawTable<T> : AModule
    {
        public DrawTable(Control control) : base(control)
        {
            Init();
        }

        private void Init()
        {
            DateTimeFormat = string.Empty;
            OrderBy = "ASC";
            CellPadding = new Padding(0, 0, 0, 0);
            RowHeaderColor = MouseEnterColor = ForeColor = Color.Black;
            IsHaveHeader = true;
            IsHaveTail = false;
            IsResetOnDataSourceChange = true;
            SingleSelctedIdx = 0;
            MouseEnterColIdx = -1;
            muiltSelctedIdxs = new HashSet<int>();

            StringFormat = StringFormates.MiddleCenter;

            cxColWidths = new List<float>();
            Columns = new List<ColumnHeader>();
            HeaderColor = Color.Black;
            HeaderFont = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold,
               System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            SelectedFont = MouseEnterFont = Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular,
               System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            RowHeight = 23;
            GridWidth = 1F;
            ScrollY = new ScrollY(Control)
            {
                ScrollOffset = RowHeight,
            };
            ScrollY.CalcHeight = () =>
            {
                var ret = 0;
                if (null != DataSource)
                    ret = (int)(RowHeight * DataSource.Count());
                return ret;
            };

            ScrollX = new ScrollX(Control)
            {
                CalcWidth = () =>
                {
                    return cxColWidths.Skip(RowHeaderIndex).Sum();
                },
            };
            ResetScroll();
            ScrollBar = ScrollBars.Vertical;
            Control.MouseMove += Control_MouseMove;
            Control.MouseClick += Control_MouseClick;
        }

        #region private

        private IEnumerable<T> dataSource;

        private IEnumerable<T> DataSourceReverse { get; set; }

        private IEnumerable<T> currentEnumable;

        private float ScrolledHeight
        {
            get => ScrollY.ScrolledHeight;
        }

        private float ScrolledWidth
        {
            get => ScrollX.ScrolledWidth;
        }

        private List<float> cxColWidths;

        //private int mouseEnterIdx;

        public int MouseEnterIdx
        {
            get; set;
        }

        public int MouseEnterColIdx
        {
            get; set;
        }

        private float MinScrollTop { get; set; }

        private float TableBottom { get; set; }

        #endregion

        #region public 

        public void ResetItem(int index)
        {

        }

        public void ResetAll()
        {
            Invalidate();
        }

        public bool IsResetOnDataSourceChange { get; set; }

        public IEnumerable<T> DataSource
        {
            get => dataSource;
            set
            {
                dataSource = value;
                if (null != dataSource)
                {
                    DataSourceReverse = dataSource.Reverse();
                    if (IsResetOnDataSourceChange)
                        ResetTable();
                }
                Invalidate();
            }
        }

        private void ResetTable()
        {
            ScrollY.Scroll(0F);
            ScrollY.ScrolledHeight = 0;
            SingleSelctedIdx = 0;
            muiltSelctedIdxs.Clear();
        }

        public event EventHandler<EventArgs> CellMouseClick;

        public event EventHandler<EventArgs> SingleSelctedIndexChange;

        public event EventHandler BeginDraw;

        public event EventHandler EndDraw;

        #endregion

        #region property

        public bool AllowUserToResizeColumns { get; set; }

        public bool AllowUserToSelctedRows { get; set; }

        public bool MuiltSelect { get; set; }

        public int FirstIdx { get; set; }

        private int singleSelctedIdx;

        public int SingleSelctedIdx
        {
            get => singleSelctedIdx;
            set
            {
                if (singleSelctedIdx != value)
                {
                    singleSelctedIdx = value;
                    if (null != currentEnumable && singleSelctedIdx >= 0 && singleSelctedIdx < currentEnumable.Count())
                        SingleSelctedIndexChange?.Invoke(this,
                            new EventArgs());
                    Invalidate();
                }
            }
        }

        private HashSet<int> muiltSelctedIdxs;

        public ScrollY ScrollY { get; set; }

        public ScrollX ScrollX { get; set; }

        private ScrollBars customScroll;

        public ScrollBars CustomScroll
        {
            get => customScroll;
            set
            {
                customScroll = value;
                switch (customScroll)
                {
                    case ScrollBars.None:
                        ScrollY.CustomScroll = false;
                        ScrollX.CustomScroll = false;
                        break;
                    case ScrollBars.Horizontal:
                        ScrollY.CustomScroll = false;
                        ScrollX.CustomScroll = true;
                        break;
                    case ScrollBars.Vertical:
                        ScrollY.CustomScroll = true;
                        ScrollX.CustomScroll = false;
                        break;
                    case ScrollBars.Both:
                        ScrollY.CustomScroll = true;
                        ScrollX.CustomScroll = true;
                        break;
                }
            }
        }

        public ScrollBars ScrollBar { get; set; }

        public int RowHeaderIndex { get; set; }

        public List<ColumnHeader> Columns { get; private set; }

        public Color HeaderColor { get; set; }

        public Color HeaderBackColor { get; set; }

        public Color RowHeaderBackColor { get; set; }

        public Color RowHeaderColor { get; set; }

        public Color ForeColor { get; set; }

        public Color MouseEnterColor { get; set; }

        public Color SelectedColor { get; set; }

        public Color ItemBackColor { get; set; }

        public Font HeaderFont { get; set; }

        public Font Font { get; set; }

        public Font MouseEnterFont { get; set; }

        public Font SelectedFont { get; set; }

        public string DateTimeFormat { get; set; }

        public string OrderBy { get; set; }

        public int RowHeight { get; set; }

        public Func<int, T, Color> CustomItemColor { get; set; }

        public Func<int, T, Color> CustomItemBackColor { get; set; }

        public Action<int, RectangleF, Graphics> CustomTitle { get; set; }

        public Func<int, int, T, RectangleF, bool, Graphics, string> CustomDrawCell { get; set; }

        public Func<int, int, T, Color> CustomCellColor { get; set; }

        public Func<int, T, bool, bool, RectangleF, Graphics, bool> CustomDrawRow { get; set; }

        public Action<int, T, RectangleF, Graphics> CustomMouseEnterRowBg { get; set; }

        public Action<int, T, RectangleF, Graphics> CustomMouseEnterRowBgAfter { get; set; }

        public Action<int, T, RectangleF, Graphics> CustomSelectedRowBg { get; set; }

        public Action<int, T, RectangleF, Graphics> CustomSelectedRowBgAfter { get; set; }

        public Action<int, int, T, RectangleF, Graphics> CustomMouseEnterCell { get; set; }

        public Padding CellPadding { get; set; }

        private bool isHaveHeader;

        public bool IsHaveHeader
        {
            get
            {
                return isHaveHeader;
            }
            set
            {
                isHaveHeader = value;
            }
        }

        private bool isHaveTail;

        public bool IsHaveTail
        {
            get
            {
                return isHaveTail;
            }
            set
            {
                isHaveTail = value;
            }
        }

        public bool ShowWholeFirst { get; set; }

        public bool IsDrawHGrid { get; set; }

        public bool IsDrawVGrid { get; set; }

        public bool IsDrawMouseEnter { get; set; }

        public bool IsDrawMouseSelect { get; set; }

        public Color GridColor { get; set; }

        public float GridWidth { get; set; }

        public bool IsSupportShrink { get; set; }

        public bool IsShrink { get; set; }

        public Rectangle ShrinkRect { get; set; }

        public Func<bool, bool> CustomShrink { get; set; }

        public StringFormat StringFormat { get; set; }

        #endregion

        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsSupportShrink && ShrinkRect.Contains(e.Location))
            {
                if (null != CustomShrink)
                {
                    IsShrink = CustomShrink(IsShrink);
                    ScrollY.Scroll(0);
                    Control.Invalidate();
                }
            }
            if (null != DataSource)
            {
                var rect = ClientRectangle;
                if (IsHaveHeader)
                {
                    rect.Y += RowHeight;
                    rect.Height -= RowHeight;
                }
                if (isHaveTail)
                {
                    rect.Height -= RowHeight;
                }
                if (rect.Contains(e.Location))
                {
                    var rowIndex = (e.Y - rect.Top - (int)ScrolledHeight) / RowHeight;
                    var columnsIndex = -1;
                    float left = rect.Left;
                    RectangleF rctCell = RectangleF.Empty;
                    for (var i = 0; i < cxColWidths.Count; i++)
                    {
                        if (e.X >= left && e.X < left + cxColWidths[i])
                        {
                            columnsIndex = i;
                            rctCell = new RectangleF(
                                left, rect.Top + rowIndex * RowHeight + ScrolledHeight,
                                cxColWidths[i],
                                RowHeight
                                );
                            break;
                        }
                        left += cxColWidths[i];
                    }
                    var enumable = currentEnumable;
                    if (rowIndex >= 0 && null != enumable && enumable.Count() > rowIndex)
                    {
                        T elem = enumable.ElementAt(rowIndex);
                        CellMouseClick?.Invoke(this, new EventArgs());
                    }
                }
            }

        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var rect = ClientRectangle;
            if (IsHaveHeader)
            {
                rect.Y += RowHeight;
                rect.Height -= RowHeight;
            }
            if (isHaveTail)
            {
                rect.Height -= RowHeight;
            }
            if (rect.Contains(e.Location))
            {
                var mid = (int)(e.Location.Y - MinScrollTop - ScrolledHeight) / RowHeight;
                if (MouseEnterIdx != mid)
                {
                    MouseEnterIdx = mid;
                }
                var mcolId = -1;
                float len = e.X - ClientRectangle.Left;
                for (var i = 0; i < cxColWidths.Count; i++)
                {
                    len -= cxColWidths[i];
                    if (len < 0)
                    {
                        mcolId = i;
                        break;
                    }
                }
                if (MouseEnterColIdx != mcolId)
                {
                    MouseEnterColIdx = mcolId;
                }
               // Invalidate();
            }
            else
            {
                MouseEnterIdx = -1;
                MouseEnterColIdx = -1;
            }
            //CustomMouseEnterCell?.Invoke(MouseEnterIdx, MouseEnterColIdx,default(T), RectangleF.Empty, null);
        }

        #region Draw

        public override void Draw(Graphics g, Rectangle rect)
        {
            BeginDraw?.Invoke(this, null);
            switch (ScrollBar)
            {
                case ScrollBars.Horizontal:
                case ScrollBars.Both:
                    if (cxColWidths.Sum() > ClientRectangle.Width)
                        rect.Height -= ScrollX.ScrollHeight;
                    break;
            }
            ClientRectangle = rect;
            ResetColWidth(ClientRectangle);
            ResetScroll();
            DrawItmes(g);
            DrawHeader(g);
            DrawTail(g);
            DrawVGrid(g);
            DrawShrink(g);
            switch (ScrollBar)
            {
                case ScrollBars.Horizontal:
                    ScrollX.Draw(g, ClientRectangle);
                    break;
                case ScrollBars.Vertical:
                    ScrollY.Draw(g, ClientRectangle);
                    break;
                case ScrollBars.Both:
                    ScrollY.Draw(g, ClientRectangle);
                    ScrollX.Draw(g, ClientRectangle);
                    break;
            }
            EndDraw?.Invoke(this, null);
        }

        private void DrawVGrid(Graphics g)
        {
            if (IsDrawVGrid)
            {
                using (var pen = new Pen(GridColor, GridWidth))
                {
                    var minsLeft = ClientRectangle.Left + cxColWidths.Take(RowHeaderIndex).Sum();
                    var sLeft = minsLeft + ScrolledWidth;
                    var maxsRight = ClientRectangle.Right;
                    float left = ClientRectangle.Left;
                    for (var i = 0; i < RowHeaderIndex; i++)
                    {
                        left += cxColWidths[i];
                        g.DrawLine(pen,
                            left, MinScrollTop, left,
                            Math.Max(MinScrollTop, TableBottom));
                    }
                    left = minsLeft + ScrolledWidth;
                    for (var i = RowHeaderIndex; i < cxColWidths.Count(); i++)
                    {
                        if (left >= minsLeft)
                        {
                            g.DrawLine(pen,
                                left, MinScrollTop, left,
                                Math.Max(MinScrollTop, TableBottom));
                        }
                        left += cxColWidths[i];
                    }

                    //foreach (var width in cxColWidths)
                    //{
                    //    g.DrawLine(pen,
                    //        left, ClientRectangle.Top, left, Math.Max(ClientRectangle.Top, ClientRectangle.Bottom));
                    //    left += width;
                    //    if (left > ClientRectangle.Right)
                    //        break;
                    //}
                }
            }
        }

        private void DrawHeader(Graphics g)
        {
            if (IsHaveHeader)
            {
                DrawTitle(g, ClientRectangle.Top);
                using (var pen = new Pen(GridColor, GridWidth))
                {
                    float left = ClientRectangle.Left;
                    if (IsDrawHGrid)
                        g.DrawLine(pen, left, ClientRectangle.Top + RowHeight, ClientRectangle.Right, ClientRectangle.Top + RowHeight);
                }
            }
        }

        private void DrawItmes(Graphics g)
        {
            if (null != DataSource)
            {
                currentEnumable = DataSource;
                if (OrderBy.ToUpper().Equals("DESC"))
                {
                    currentEnumable = DataSourceReverse;
                }
                var props = GetPropertyInfos().ToList();
                //ResetColWidth(ClientRectangle);
                var top = ClientRectangle.Top + ScrolledHeight;
                MinScrollTop = ClientRectangle.Top;
                var maxBottom = ClientRectangle.Bottom;
                if (IsHaveHeader)
                {
                    top += RowHeight;
                    MinScrollTop = ClientRectangle.Top + RowHeight;
                }
                if (IsHaveTail)
                {
                    maxBottom = ClientRectangle.Bottom - RowHeight;
                }
                //var pMouse = Control.PointToClient(Control.MousePosition);
                var rowIdx = -1;
                try
                {
                    using (var gridPen = new Pen(GridColor, GridWidth))
                    {
                        var minsLeft = ClientRectangle.Left + cxColWidths.Take(RowHeaderIndex).Sum();
                        var sLeft = minsLeft + ScrolledWidth;
                        var maxsRight = ClientRectangle.Right;

                        foreach (var p in currentEnumable)
                        {
                            rowIdx++;
                            float left = ClientRectangle.Left;
                            if (top + RowHeight <= MinScrollTop || top > maxBottom)
                            {
                                top += RowHeight;
                                continue;
                            }
                            else if (top < MinScrollTop && ShowWholeFirst)
                            {
                                top = MinScrollTop;
                            }
                            var rctRow = new RectangleF(
                                left,
                                top,
                                ClientRectangle.Width,
                                Math.Min(RowHeight, maxBottom - top));
                            if (null != CustomItemColor)
                                ForeColor = CustomItemColor(rowIdx, p);
                            if (null != CustomItemBackColor)
                                ItemBackColor = CustomItemBackColor(rowIdx, p);
                            using (var backBrush = new SolidBrush(ItemBackColor))
                                g.FillRectangle(backBrush, rctRow);
                            var font = Font;
                            var fColor = ForeColor;
                            var isMouseEnter = false;
                            if (MouseEnterIdx == rowIdx)
                            {
                                isMouseEnter = true;
                                CustomMouseEnterRowBg?.Invoke(MouseEnterIdx, p, rctRow, g);
                                if (IsDrawMouseEnter)
                                {
                                    fColor = MouseEnterColor;
                                    font = MouseEnterFont;
                                }
                            }
                            var isSelected = false;
                            if (AllowUserToSelctedRows && ((!MuiltSelect && SingleSelctedIdx == rowIdx)
                                || (MuiltSelect || muiltSelctedIdxs.Contains(rowIdx))))
                            {
                                isSelected = true;
                                CustomSelectedRowBg?.Invoke(rowIdx, p, rctRow, g);
                                if (IsDrawMouseSelect)
                                {
                                    fColor = SelectedColor;
                                    font = SelectedFont;
                                }
                            }
                            if (null == CustomDrawRow || !CustomDrawRow(rowIdx, p, isSelected, isMouseEnter, rctRow, g))
                            {
                                using (var rowHBrush = new SolidBrush(RowHeaderColor))
                                {
                                    var txtHeight = 1 + (int)g.MeasureString("华", Font).Height;
                                    left = minsLeft;
                                    for (var i = RowHeaderIndex; i < Columns.Count; i++)
                                    {
                                        var cellColor = fColor;
                                        var stringFormat = Columns[i].StringFormat ?? StringFormat;
                                        var rctTxt = new RectangleF(
                                            CellPadding.Left + left,
                                            top + (RowHeight - txtHeight) / 2 + 1,
                                            cxColWidths[i],
                                            txtHeight);
                                        rctTxt.Offset(ScrolledWidth, 0);
                                        if (rctTxt.Right <= minsLeft || rctTxt.Left > maxsRight)
                                        {
                                            left += cxColWidths[i];
                                            continue;
                                        }
                                        rctTxt.Height = Math.Min(Control.ClientRectangle.Bottom - ScrollY.Padding.Bottom - top - (RowHeight - txtHeight) / 2 - 1, txtHeight);
                                        var szCell = string.Empty;
                                        var cellRect = new RectangleF(
                                                   CellPadding.Left + left,
                                                   top,
                                                   cxColWidths[i],
                                                   Math.Min(RowHeight, Math.Min(ClientRectangle.Bottom, top + RowHeight) - top));
                                        cellRect.Offset(ScrolledWidth, 0);
                                        if (MouseEnterIdx == rowIdx && MouseEnterColIdx == i)
                                        {
                                            CustomMouseEnterCell?.Invoke(rowIdx, i, p, cellRect, g);
                                        }
                                        if (Columns[i].IsDrawCustom)
                                        {
                                            if (null != CustomDrawCell)
                                            {
                                                szCell = CustomDrawCell(rowIdx, i, p, cellRect, isMouseEnter, g);
                                            }
                                            if (null != CustomCellColor)
                                            {
                                                var color = CustomCellColor(rowIdx, i, p);
                                                if (Color.Empty != color)
                                                {
                                                    cellColor = color;
                                                }
                                            }
                                        }
                                        if (string.IsNullOrEmpty(szCell))
                                        {
                                            var prop = props.Find(pr => pr.Name.Equals(Columns[i].Name));
                                            if (null != prop)
                                            {
                                                var val = prop.GetValue(p);
                                                if (val is DateTime)
                                                {
                                                    szCell = ((DateTime)val).ToString(DateTimeFormat);
                                                }
                                                else
                                                {
                                                    if (null != val)
                                                        szCell = val.ToString();
                                                    else
                                                        szCell = string.Empty;
                                                }
                                            }
                                        }
                                        if (rctTxt.Height > 0)
                                        {
                                            var oldFmt = stringFormat.LineAlignment;
                                            if (rctTxt.Height < txtHeight)
                                                stringFormat.LineAlignment = StringAlignment.Near;
                                            if (default(Color) != Columns[i].Color)
                                            {
                                                using (var cellBrush = new SolidBrush(Columns[i].Color))
                                                    g.DrawString(szCell, font, cellBrush, rctTxt, stringFormat);
                                            }
                                            else
                                            {
                                                using (var foreBrush = new SolidBrush(cellColor))
                                                    g.DrawString(szCell, font, foreBrush, rctTxt, stringFormat);
                                            }
                                            stringFormat.LineAlignment = oldFmt;
                                        }
                                        left += cxColWidths[i];
                                    }
                                    left = ClientRectangle.Left;
                                    //rowhead
                                    for (var i = 0; i < RowHeaderIndex; i++)
                                    {
                                        var stringFormat = Columns[i].StringFormat ?? StringFormat;
                                        var rctTxt = new RectangleF(
                                            CellPadding.Left + left,
                                            top + (RowHeight - txtHeight) / 2 + 1,
                                            cxColWidths[i],
                                            txtHeight)
                                        {
                                            Height = Math.Min(Control.ClientRectangle.Bottom - ScrollY.Padding.Bottom - top - (RowHeight - txtHeight) / 2 - 1, txtHeight)
                                        };
                                        var cellRect = new RectangleF(
                                           left,
                                           top,
                                           cxColWidths[i],
                                           Math.Min(RowHeight, maxBottom - top + 1));
                                        using (var rhbgBrush = new SolidBrush(RowHeaderBackColor))
                                            g.FillRectangle(rhbgBrush, cellRect);
                                        var szCell = string.Empty;
                                        if (Columns[i].IsDrawCustom)
                                        {
                                            if (null != CustomDrawCell)
                                                szCell = CustomDrawCell(rowIdx, i, p, new RectangleF(
                                                CellPadding.Left + left,
                                                top,
                                                cxColWidths[i],
                                                Math.Min(RowHeight, Math.Min(ClientRectangle.Bottom, top + RowHeight) - top)), isMouseEnter, g);
                                        }
                                        if (string.IsNullOrEmpty(szCell))
                                        {
                                            var prop = props.Find(pr => pr.Name.Equals(Columns[i].Name));
                                            if (null != prop)
                                            {
                                                var val = prop.GetValue(p);
                                                if (val is DateTime)
                                                {
                                                    szCell = ((DateTime)val).ToString(DateTimeFormat);
                                                }
                                                else
                                                {
                                                    if (null != val)
                                                        szCell = val.ToString();
                                                    else
                                                        szCell = string.Empty;
                                                }
                                            }
                                        }
                                        if (rctTxt.Height > 0)
                                        {
                                            var oldFmt = stringFormat.LineAlignment;
                                            if (rctTxt.Height < txtHeight)
                                                stringFormat.LineAlignment = StringAlignment.Near;
                                            if (default(Color) != Columns[i].Color)
                                            {
                                                using (var cellBrush = new SolidBrush(Columns[i].Color))
                                                    g.DrawString(szCell, font, cellBrush, rctTxt, stringFormat);
                                            }
                                            else
                                                g.DrawString(szCell, font, rowHBrush, rctTxt, stringFormat);

                                            stringFormat.LineAlignment = oldFmt;
                                        }
                                        left += cxColWidths[i];
                                    }
                                }
                                if (top > MinScrollTop && IsDrawHGrid)
                                {
                                    g.DrawLine(gridPen, ClientRectangle.Left, top, ClientRectangle.Right, top);
                                }
                            }
                            if (MouseEnterIdx == rowIdx)
                                CustomMouseEnterRowBgAfter?.Invoke(rowIdx, p, rctRow, g);
                            if (AllowUserToSelctedRows && ((!MuiltSelect && SingleSelctedIdx == rowIdx)
                                || (MuiltSelect || muiltSelctedIdxs.Contains(rowIdx))))
                            {
                                CustomSelectedRowBgAfter?.Invoke(rowIdx, p, rctRow, g);
                            }
                            top += RowHeight;
                            TableBottom = top - 1;
                            if (top > ClientRectangle.Bottom)
                            {
                                TableBottom = Math.Min(ClientRectangle.Bottom, TableBottom);
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(e.Message);
#endif
                }
            }
        }

        private void DrawTail(Graphics g)
        {
            if (IsHaveTail)
                DrawTitle(g, Math.Max(ClientRectangle.Top + 1 * RowHeight,
                    ClientRectangle.Bottom - RowHeight));
        }

        private void DrawTitle(Graphics g, float top)
        {
            try
            {
                var props = GetPropertyInfos();
                float left = ClientRectangle.Left;
                var txtHeight = 1 + (int)g.MeasureString("0", HeaderFont).Height;
                using (var backBrush = new SolidBrush(HeaderBackColor))
                using (var brush = new SolidBrush(HeaderColor))
                {
                    g.FillRectangle(backBrush, left, top, ClientRectangle.Width, RowHeight);
                    var minsLeft = ClientRectangle.Left + cxColWidths.Take(RowHeaderIndex).Sum();
                    var sLeft = minsLeft + ScrolledWidth;
                    var maxsRight = ClientRectangle.Right;
                    left = minsLeft;
                    for (var i = RowHeaderIndex; i < Columns.Count; i++)
                    {
                        var stringFormat = Columns[i].StringFormat ?? StringFormat;
                        var txtRect = new RectangleF(
                            CellPadding.Left + left,
                            top + (RowHeight - txtHeight) / 2 + 1,
                            cxColWidths[i],
                            txtHeight);
                        txtRect.Offset(ScrolledWidth, 0);
                        if (txtRect.Right <= minsLeft || txtRect.Left > maxsRight)
                        {
                            left += cxColWidths[i];
                            continue;
                        }
                        if (i < Columns.Count)
                        {
                            if (Columns[i].IsDrawCustom && null != CustomTitle)
                            {
                                CustomTitle(i, txtRect, g);
                            }
                            else
                                g.DrawString(Columns[i].Text, HeaderFont, brush, txtRect, stringFormat);
                        }
                        left += cxColWidths[i];
                    }
                    left = ClientRectangle.Left;
                    for (var i = 0; i < RowHeaderIndex; i++)
                    {
                        var stringFormat = Columns[i].StringFormat ?? StringFormat;
                        var cellHeader = new RectangleF(
                            left,
                            top,
                            cxColWidths[i],
                            RowHeight
                            );
                        g.FillRectangle(backBrush, cellHeader);
                        var txtRect = new RectangleF(
                            CellPadding.Left + left,
                            top + (RowHeight - txtHeight) / 2 + 1,
                            cxColWidths[i],
                            txtHeight);
                        if (i < Columns.Count)
                        {
                            if (Columns[i].IsDrawCustom && null != CustomTitle)
                            {
                                CustomTitle(i, txtRect, g);
                            }
                            else
                                g.DrawString(Columns[i].Text, HeaderFont, brush, txtRect, stringFormat);
                        }
                        left += cxColWidths[i];
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.StackTrace);
#endif
            }
        }

        private void DrawShrink(Graphics g)
        {
            if (IsSupportShrink)
            {
                var width = (int)(RowHeight * 0.3);
                ShrinkRect = new Rectangle(
                    ClientRectangle.Right - 12 - width,
                    ClientRectangle.Top + width,
                    12, (int)(width * 1.5));
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(0xD8, 0xD8, 0xD8)))
                {
                    if (IsShrink)
                        g.FillPolygon(brush, new Point[] {
                        new Point(ShrinkRect.Left +ShrinkRect.Width/2,ShrinkRect.Top),
                        new Point(ShrinkRect.Left,ShrinkRect.Bottom),
                        new Point(ShrinkRect.Right,ShrinkRect.Bottom)
                    }, System.Drawing.Drawing2D.FillMode.Winding);
                    else
                        g.FillPolygon(brush, new Point[] { ShrinkRect.Location,
                        new Point(ShrinkRect.Right,ShrinkRect.Top),
                        new Point(ShrinkRect.Left +ShrinkRect.Width/2,ShrinkRect.Bottom)
                    }, System.Drawing.Drawing2D.FillMode.Winding);
                }
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
        }

        private PropertyInfo[] GetPropertyInfos()
        {
            T t = Activator.CreateInstance<T>();
            return t.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        }

        private void ResetColWidth(Rectangle rect)
        {
            var props = GetPropertyInfos().ToList();
            var width = 0F;
            for (var i = Columns.Count - 1; i >= 0; i--)
            {
                var prop = props.Find(p => p.Name.Equals(Columns[i].Name));
                if (null != prop || Columns[i].IsDrawCustom)
                {
                    if (Columns[i].Width <= 0)
                    {
                        Columns[i].Width = 100;
                    }
                    width += Columns[i].Width;
                }
                else
                    Columns.RemoveAt(i);
            }
            if (rect.Width > 0)
            {
                cxColWidths.Clear();
                if (AllowUserToResizeColumns)
                {
                    foreach (var col in Columns)
                    {
                        cxColWidths.Add(col.Width);
                    }
                }
                else
                {
                    var bi = rect.Width / width;
                    foreach (var col in Columns)
                    {
                        cxColWidths.Add(Math.Max(col.Min, col.Width * bi));
                    }
                }
            }
        }

        private void ResetScroll()
        {
            var padding = ScrollY.Padding;
            padding.Top = ClientRectangle.Top;
            padding.Bottom = Control.ClientRectangle.Bottom - ClientRectangle.Bottom;
            padding.Right = Control.ClientRectangle.Right - ClientRectangle.Right;
            padding.Left = ClientRectangle.Left
                - Control.ClientRectangle.Left + (int)cxColWidths.Take(RowHeaderIndex).Sum();
            ScrollX.ResetScrollX(padding);
            padding.Left = ClientRectangle.Left - Control.ClientRectangle.Left;
            if (IsHaveHeader)
                padding.Top += RowHeight;
            if (IsHaveTail)
                padding.Bottom += RowHeight;
            ScrollY.ResetScrollY(padding);
        }

        #endregion

        public class ColumnHeader
        {

            public string Name { get; set; }

            public string Text { get; set; }

            public float Width { get; set; }

            public float Min { get; set; }

            public bool IsDrawCustom { get; set; }

            public StringFormat StringFormat { get; set; }

            public Color Color { get; set; }

            public ColumnHeader(string name, float width = 100)
            {
                Name = name;
                Text = name;
                Width = width;
            }

            public ColumnHeader(string name, string text, float width = 100) : this(name, width)
            {
                Text = text;
            }

            public ColumnHeader(string name, string text, float width = 100, float min = 100) : this(name, text, width)
            {
                Min = min;
            }
        }
    }
}
