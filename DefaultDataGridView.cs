using SalesCounter.src.utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SalesCounter.src.custom.control
{
    //datagridview
    public class DefaultDataGridView : DataGridView
    {
        DefaultVScroll scroll = new DefaultVScroll();
        #region 构造函数
        public DefaultDataGridView() : base()
        {

            
            SetStyle(ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor, true);


            this.EnableHeadersVisualStyles = false;
            this.ScrollBars = ScrollBars.None;
            this.BackgroundColor = Color.Black;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.BorderStyle = BorderStyle.None;
            //row
            this.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(29,29,29);
            this.RowsDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            this.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.RowsDefaultCellStyle.Padding = new Padding(0,0,0,0);
            this.RowTemplate.Height = 28;
            this.ForeColor = Color.White;
            this.AllowUserToResizeRows = false;
            //header
            this.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255,31,40,56);
            this.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ColumnHeadersHeight = 33;        
            this.AllowUserToResizeColumns = false;

            this.MultiSelect = false;
            this.GridColor = Color.Black;
            //this.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.CellBorderStyle = DataGridViewCellBorderStyle.None;
            this.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
            this.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
            SetStyle(ControlStyles.Selectable, false);
            this.AllowUserToAddRows = false;
            this.ReadOnly = true;          
            this.RowHeadersVisible = false;
            this.Controls.Add(scroll);
            scroll.Dock = DockStyle.Right;
            scroll.Visible = false;
            this.scroll.ValueChanged += Scroll_ValueChanged;
        }
        #endregion

        protected override void WndProc(ref Message m)
        {
            //const int WM_LBUTTONDOWN = 0x0201;
            //const int WM_LBUTTONDBLCLK = 0x0203;
            //switch (m.Msg)
            //{
            //    case WM_LBUTTONDOWN:
            //    case WM_LBUTTONDBLCLK:
            //        return;
            //}
            base.WndProc(ref m);
        }
        public override DataGridViewAdvancedBorderStyle AdjustColumnHeaderBorderStyle(
           DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
           DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceHolder,
           bool firstDisplayedColumn,
           bool lastVisibleColumn)
        {
            // Customize the left border of the first column header and the
            // bottom border of all the column headers. Use the input style for 
            // all other borders.
            dataGridViewAdvancedBorderStylePlaceHolder.Left = firstDisplayedColumn ?
                DataGridViewAdvancedCellBorderStyle.None :
                DataGridViewAdvancedCellBorderStyle.None;
            dataGridViewAdvancedBorderStylePlaceHolder.Bottom =
                DataGridViewAdvancedCellBorderStyle.Single;

            dataGridViewAdvancedBorderStylePlaceHolder.Right = lastVisibleColumn ?
                DataGridViewAdvancedCellBorderStyle.None :
                DataGridViewAdvancedCellBorderStyle.Single;
            dataGridViewAdvancedBorderStylePlaceHolder.Top =
                dataGridViewAdvancedBorderStyleInput.Top;

            return dataGridViewAdvancedBorderStylePlaceHolder;
        }
        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);
            SetScrollBarEx();
        }
        public override DataGridViewAdvancedBorderStyle AdjustedTopLeftHeaderBorderStyle
        {
            get
            {
                DataGridViewAdvancedBorderStyle newStyle =
                    new DataGridViewAdvancedBorderStyle();
                newStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                newStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
                newStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
                newStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
                return newStyle;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.Rows.GetRowsHeight(DataGridViewElementStates.None) > this.Rows.GetRowsHeight(DataGridViewElementStates.Displayed))
            {
                this.scroll.Visible = true;
                SetScrollBarEx();
            }
            else
                this.scroll.Visible = false;
        }
        private void SetScrollBarEx()
        {
            //scroll.Location = new Point(this.DisplayRectangle.Width, 0);
            //scroll.Height = this.DisplayRectangle.Height;
            scroll.LargeChange = 1;
            this.scroll.Maximum = this.Rows.Count - this.Rows.GetRowCount(DataGridViewElementStates.Displayed)+1;           
        }

        private void Scroll_ValueChanged(object sender, EventArgs e)
        {
            this.FirstDisplayedScrollingRowIndex = scroll.Value;
        }
        public void Wheel(MouseEventArgs e)
        {
            if(this.scroll.Visible)
            this.OnMouseWheel(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.scroll.Value += e.Delta / (-120);
        }
    }
    #region checkboxHeader
    //实现全选的事件参数类
    public class DataGridviewCheckboxHeaderEventHander : EventArgs
    {
        private bool checkedState = false;

        public bool CheckedState
        {
            get { return checkedState; }
            set { checkedState = value; }
        }
    }

    //与事件关联的委托
    public delegate void DataGridviewCheckboxHeaderCellEventHander(object sender, DataGridviewCheckboxHeaderEventHander e);
    //cell
    public class DataGridviewCheckboxHeaderCell : DataGridViewColumnHeaderCell
    {
        private bool isChecked = false;
        private Point cellLocation = new Point();
        private CheckBoxState cbState = CheckBoxState.UncheckedNormal;
        private Image checkedPic = ResControl.自选股;
        private bool isEnter = false;
        public event DataGridviewCheckboxHeaderCellEventHander OnCheckBoxClicked;
        
        
        //绘制列头checkbox
        protected override void Paint(Graphics g,
                                      Rectangle clipBounds,
                                      Rectangle cellBounds,
                                      int rowIndex,
                                      DataGridViewElementStates dataGridViewElementState,
                                      object value,
                                      object formattedValue,
                                      string errorText,
                                      DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            //base.Paint(g, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            SolidBrush backBursh = new SolidBrush(cellStyle.BackColor);
            
            g.FillRectangle(backBursh,cellBounds);
            base.PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            paintCheckBox(g, cellBounds);
            //Point p = new Point();
            //Size s = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
            ////列头checkbox的X坐标
            //p.X = cellBounds.Location.X + (cellBounds.Width / 2) - (s.Width / 2) - 1;
            ////列头checkbox的Y坐标
            //p.Y = cellBounds.Location.Y + (cellBounds.Height / 2) - (s.Height / 2);
            //cellLocation = cellBounds.Location;
            //checkBoxLocation = p;
            //checkBoxSize = s;
            //if (isChecked)
            //    cbState = CheckBoxState.CheckedNormal;
            //else
            //    cbState = CheckBoxState.UncheckedNormal;
            ////绘制复选框
            //CheckBoxRenderer.DrawCheckBox(g, checkBoxLocation, cbState);
        }

        private void paintCheckBox(Graphics g, Rectangle clientRectangle)
        {
            var rect = clientRectangle;
            //计算边框尺寸 
            
            var picRect = new Rectangle(rect.X+5,rect.Y+(rect.Height-checkedPic.Height)/2, checkedPic.Width,checkedPic.Height);
            var textRect = new Rectangle(rect.X,rect.Y,rect.Width-checkedPic.Width-5,rect.Height);
            if (!isChecked)
            {
                var c = Color.FromArgb(225, 200, 200, 200);
                if (isEnter)
                {
                    c = Color.FromArgb(225, 48, 118, 193);
                }
                GraphicsPath path = DrawUtil.GetRoundedRectPath(picRect, 5);
                g.DrawPath(new Pen(c), path);
            }
            else
            {
                g.DrawImage(checkedPic, picRect);
            }
            TextRenderer.DrawText(g, "自选", new Font("PingFangSC-Regular",14,GraphicsUnit.Pixel), textRect, Color.White);
            //if (Focused)
            //{
            //    // var r = new Rectangle(Location.X - Margin.Left, Location.Y - Margin.Top,
            //    //   Size.Width + 2 * Margin.Left, Size.Height + 2 * Margin.Top);
            //    //SendMessage(FindForm().Handle,AppDefine.WM_DRAWDOT, 0, ref r);
            //    var border = ClientRectangle;
            //    border.Width -= 1;
            //    border.Height -= 1;
            //    pevent.Graphics.DrawRectangle(new Pen(Color.Gray) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot }, border);
            //}
        }

        /// <summary>
        /// 响应点击列头checkbox单击事件
        /// </summary>
        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            Point p = new Point(e.X + cellLocation.X, e.Y + cellLocation.Y);
            Rectangle paintCellBounds = DataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);//坐标要转换
            Rectangle recDetail = new Rectangle(5, (paintCellBounds.Height - checkedPic.Height) / 2, checkedPic.Width, checkedPic.Height);
            if (recDetail.Contains(new Point(e.X, e.Y))&&e.Button==MouseButtons.Left)
            {
                isChecked = !isChecked;
                this.DataGridView.InvalidateCell(this);
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (base.DataGridView == null) return;
            var nowColIndex = e.ColumnIndex;
            var nowRowIndex = e.RowIndex;

            Rectangle paintCellBounds = DataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);//坐标要转换
            Rectangle recDetail = new Rectangle(5, (paintCellBounds.Height - checkedPic.Height) / 2, checkedPic.Width, checkedPic.Height);
            if (recDetail.Contains(new Point(e.X, e.Y))) // 鼠标移动到按钮上
            {
                DataGridView.Cursor = Cursors.Hand;
            }
            else
                DataGridView.Cursor = Cursors.Default;
        }

    }

    #endregion
    //select
    public class DataGridViewSelectColumn : DataGridViewColumn
    {
        public DataGridViewSelectColumn()
        {
            this.CellTemplate = new DataGridViewSelectCell();
        }
    }

    /// <summary>
    /// DataGridView操作按钮单元格，DataGridViewCheckBoxCell。使用本自定义列或单元格前，请
    /// 确保应用程序的Properties.Resources资源文件中，包含了分别名为：的图片。
    /// </summary>
    public class DataGridViewSelectCell : DataGridViewCheckBoxCell
    {
        private bool mouseOnDetailButton = false; // 鼠标是否移动到查看详细按钮上
        private static Image ImageDetail = Properties.Resources.icon_搜索结果_自选股_勾; // 背景图片
        private static Pen penDetail = new Pen(Color.FromArgb(135, 163, 193)); // 边框颜色
        private static int nowColIndex = 0; // 当前列序号
        private static int nowRowIndex = 0; // 当前行序号

        public DataGridViewSelectCell()
            :base()
        {
        }
        /// <summary>
        /// 对单元格的重绘事件进行的方法重写。
        /// </summary>
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
            object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
             base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
             PrivatePaint(graphics, cellBounds, rowIndex, cellStyle, true);
             //base.PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            //nowColIndex = this.DataGridView.Columns.Count - 1;
        }

        /// <summary>
        /// 私有的单元格重绘方法，根据鼠标是否移动到按钮上，对按钮的不同背景和边框进行绘制。
        /// </summary>
        private Rectangle PrivatePaint(Graphics graphics, Rectangle cellBounds, int rowIndex, DataGridViewCellStyle cellStyle, bool clearBackground)
        {
            Font selectFont=new Font("PingFangSC-Regular",14,GraphicsUnit.Pixel);
            Brush selectbrush;
            if ((bool)GetValue(rowIndex)) 
            {
                ImageDetail = Properties.Resources.icon_搜索结果_自选股_勾;
                penDetail = new Pen(Color.FromArgb(162, 144, 77));
                selectbrush = new SolidBrush(Color.FromArgb(34,162,250));
            }
            else
            {
                ImageDetail = Properties.Resources.icon_搜索结果_自选股_加;
                penDetail = new Pen(Color.FromArgb(135, 163, 193));
                selectbrush = new SolidBrush(Color.White);
            }


            if (clearBackground) // 是否需要重绘单元格的背景颜色
            {
                Brush brushCellBack = (rowIndex == this.DataGridView.CurrentRow.Index) ? new SolidBrush(cellStyle.SelectionBackColor) : new SolidBrush(cellStyle.BackColor);
               // Brush brushCellBack = (rowIndex == this.DataGridView.SelectedRows[0].Index) ? new SolidBrush(cellStyle.SelectionBackColor) : new SolidBrush(cellStyle.BackColor);
                graphics.FillRectangle(brushCellBack, cellBounds.X + 1, cellBounds.Y + 1, cellBounds.Width - 2, cellBounds.Height - 2);
            }

            Rectangle recDetail = new Rectangle(cellBounds.Location.X +5, cellBounds.Location.Y + (cellBounds.Height-ImageDetail.Height)/2, ImageDetail.Width, ImageDetail.Height);
  
            
            graphics.DrawImage(ImageDetail, recDetail);
            graphics.DrawString("自选",selectFont , selectbrush, new Point(recDetail.Right+5, recDetail.Top+(recDetail.Height-selectFont.Height)/2));
            //graphics.DrawRectangle(penDetail, recDetail.X, recDetail.Y - 1, recDetail.Width, recDetail.Height);
            return cellBounds;
        }

        /// <summary>
        /// 鼠标移动到单元格内时的事件处理，通过坐标判断鼠标是否移动到了修改或删除按钮上，并调用私有的重绘方法进行重绘。
        /// </summary>
        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            if (base.DataGridView == null) return;
            
            nowColIndex = e.ColumnIndex;
            nowRowIndex = e.RowIndex;

            //Rectangle cellBounds = ContentBounds;

            Rectangle paintCellBounds = DataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);//坐标要转换
            Rectangle recDetail = new Rectangle(5, (paintCellBounds.Height - ImageDetail.Height) / 2, ImageDetail.Width, ImageDetail.Height);
            paintCellBounds.Width = DataGridView.Columns[nowColIndex].Width;
            paintCellBounds.Height = DataGridView.Rows[nowRowIndex].Height;
            paintCellBounds.Location = new Point(0, 0);
            if (IsInRect(e.X, e.Y, recDetail)) // 鼠标移动到按钮上
            {
                if (!mouseOnDetailButton)
                {
                    mouseOnDetailButton = true;
                    //PrivatePaint(this.DataGridView.CreateGraphics(), paintCellBounds, e.RowIndex, this.DataGridView.RowTemplate.DefaultCellStyle, false);
                    DataGridView.Cursor = Cursors.Hand;
                }
            }
            else
            {
                if (mouseOnDetailButton)
                {
                    mouseOnDetailButton = false;
                    //PrivatePaint(this.DataGridView.CreateGraphics(), paintCellBounds, e.RowIndex, this.DataGridView.RowTemplate.DefaultCellStyle, false);
                    DataGridView.Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// 鼠标从单元格内移出时的事件处理，调用私有的重绘方法进行重绘。
        /// </summary>
        protected override void OnMouseLeave(int rowIndex)
        {
            if (mouseOnDetailButton != false)
            {
                mouseOnDetailButton = false;

                Rectangle paintCellBounds = DataGridView.GetCellDisplayRectangle(nowColIndex, nowRowIndex, true);

                paintCellBounds.Width = DataGridView.Columns[nowColIndex].Width;
                paintCellBounds.Height = DataGridView.Rows[nowRowIndex].Height;

                //PrivatePaint(this.DataGridView.CreateGraphics(), paintCellBounds, nowRowIndex, this.DataGridView.RowTemplate.DefaultCellStyle, false);
                DataGridView.Cursor = Cursors.Default;
            }
        }
        /// <summary>
        /// 判断用户是否单击了按钮，DataGridView发生CellMouseClick事件时，
        /// 本方法通过坐标判断用户是否单击了按钮。
        /// </summary>
        public  bool IsSelectButtonClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return false;
            if (sender is DataGridView)
            {
                DataGridView DgvGrid = (DataGridView)sender;
                if (DgvGrid.Columns[e.ColumnIndex] is DataGridViewSelectColumn)
                {
                    Rectangle rec=DgvGrid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    Rectangle cellBounds = DgvGrid[e.ColumnIndex, e.RowIndex].ContentBounds;
                    Rectangle recDetail = new Rectangle(2,1, ImageDetail.Width, ImageDetail.Height);
                    if (IsInRect(e.X, e.Y, recDetail))
                    {
                        SetValue(e.RowIndex,true);
                        return true;                   
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断鼠标坐标是否在指定的区域内。
        /// </summary>
        private static bool IsInRect(int x, int y, Rectangle area)
        {
            if (x > area.Left && x < area.Right && y > area.Top && y < area.Bottom)
                return true;
            return false;
        }
        public override DataGridViewAdvancedBorderStyle AdjustCellBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput, 
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder, 
            bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
        {
            //return base.AdjustCellBorderStyle(dataGridViewAdvancedBorderStyleInput, dataGridViewAdvancedBorderStylePlaceholder, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
            dataGridViewAdvancedBorderStylePlaceholder.Left = 
                DataGridViewAdvancedCellBorderStyle.None;
            dataGridViewAdvancedBorderStylePlaceholder.Bottom =
                DataGridViewAdvancedCellBorderStyle.Single;

            dataGridViewAdvancedBorderStylePlaceholder.Right = 
                DataGridViewAdvancedCellBorderStyle.None ;
            dataGridViewAdvancedBorderStylePlaceholder.Top =
                DataGridViewAdvancedCellBorderStyle.None; 

            return dataGridViewAdvancedBorderStylePlaceholder;
        }

    }
}

