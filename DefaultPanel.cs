using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesBoss.src.controls
{
    class DefaultPanel:Panel
    {
        public const int WM_ERASEBKGND = 0x0014;
        public DefaultPanel()
        {
            SetStyle(
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer, true);
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    return;
            }
            base.WndProc(ref m);
        }
    }
}
