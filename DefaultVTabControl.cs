using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.TabControl;

namespace SalesCounter.src.custom.control
{
    public partial class DefaultVTabControl : UserControl
    {
        private int _itmecount;
        public Color SelectedBtnColor
        {
            get { return this.defaultTabBar1.SelectColor; }
            set { this.defaultTabBar1.SelectColor = value; }
        }
        public Color BtnColor
        {
            get
            {
                return this.defaultTabBar1.BtnColor;
            }
            set
            {
                this.defaultTabBar1.BtnColor = value;
            }
        }
        public int SelectedIndex
        {
            get {return this.defaultTabBar1.SelectedIndex; }
        }
        public int BarHeight
        {
            get { return this.defaultTabBar1.Height; }
            set { this.defaultTabBar1.Height = value; }
        }
        public int ItemCount
        {
            get
            {
                return _itmecount;
            }
            //set {
            //    if (value <= 0)
            //        value = 1;
            //    _itmecount = value;
            //    this.defaultTabBar1.BtnCount = value;
            //    if (this.noTabControl1.TabPages.Count < value)
            //    {
            //        for(int i=value-this.noTabControl1.TabPages.Count;i>0;i--)
            //        {
                        
            //        }
            //    }
            //    else
            //    {
            //        for (int i = this.noTabControl1.TabPages.Count; i > value; i--)
            //        {
            //            this.noTabControl1.TabPages.RemoveAt(i);
            //        }
            //    }
            //}
        }

        public TabPageCollection TabPages
        {
            get
            {
                return this.noTabControl1.TabPages;
            }           
        }
        public DefaultVTabControl()
        {
            InitializeComponent();
            _itmecount = this.noTabControl1.TabCount;
            this.noTabControl1.ControlAdded += NoTabControl1_ControlAdded;
            this.noTabControl1.ControlRemoved += NoTabControl1_ControlRemoved;
            SetBtnText();
        }
        private void SetBtnText()
        {
            if (_itmecount != this.defaultTabBar1.Items.Count)
            {
                this.defaultTabBar1.Items.Clear();
                for (int i = 0; i < _itmecount; i++)
                {
                    this.defaultTabBar1.Items.Add(new BsItem(this.TabPages[i].Text));
                }
            }
        }
        
        private void NoTabControl1_ControlRemoved(object sender, ControlEventArgs e)
        {
            this._itmecount--;
            this.defaultTabBar1.BtnCount = _itmecount;
            SetBtnText();
        }

        private void NoTabControl1_ControlAdded(object sender, ControlEventArgs e)
        {
            this._itmecount++;
            this.defaultTabBar1.BtnCount = _itmecount;
            SetBtnText();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        new public Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                this.defaultTabBar1.BackColor = value;
            }
        }
    }
}
