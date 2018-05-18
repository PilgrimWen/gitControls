using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SalesCounter
{
    public class HotKey
    {
        //如果函数执行成功，返回值不为0。  
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。  
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,                 //要定义热键的窗口的句柄  
            int id,                      //定义热键ID（不能与其它ID重复）            
            KeyModifiers fsModifiers,    //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效  
            System.Windows.Forms.Keys vk                      //定义热键的内容  
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                 //要取消热键的窗口的句柄  
            int id                       //要取消热键的ID  
            );

        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）  
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }
    }

    //protected override void WndProc(ref Message m)
    //{
    //    const int WM_HOTKEY = 0x0312;
    //    //按快捷键  
    //    switch (m.Msg)
    //    {
    //        case WM_HOTKEY:
    //            switch (m.WParam.ToInt32())
    //            {
    //                case 100:     //按下的是Shift+S  
    //                    MessageBox.Show("aa");//此处填写快捷键响应代码          
    //                    break;
    //                case 101:     //按下的是Ctrl+B  
    //                              //此处填写快捷键响应代码  
    //                    break;
    //                case 102:     //按下的是Alt+D  
    //                              //此处填写快捷键响应代码  
    //                    break;
    //            }
    //            break;
    //    }
    //    base.WndProc(ref m);
    //}

    //private void txtKLine_KeyDown(object sender, KeyEventArgs e)
    //{
    //    StringBuilder keyValue = new StringBuilder();
    //    keyValue.Length = 0;
    //    keyValue.Append("");
    //    HotKey.KeyModifiers keyModifiers = HotKey.KeyModifiers.None;
    //    Keys k = Keys.None;
    //    if (e.Modifiers != 0)
    //    {
    //        if (e.Control)
    //        {
    //            keyValue.Append("Ctrl + ");
    //            keyModifiers = HotKey.KeyModifiers.Ctrl;
    //        }
    //        if (e.Alt)
    //        {
    //            keyValue.Append("Alt + ");
    //            keyModifiers = HotKey.KeyModifiers.Alt;
    //        }
    //        if (e.Shift)
    //        {
    //            keyValue.Append("Shift + ");
    //            keyModifiers = HotKey.KeyModifiers.Shift;
    //        }
    //    }
    //    if ((e.KeyValue >= 33 && e.KeyValue <= 40) ||
    //        (e.KeyValue >= 65 && e.KeyValue <= 90) ||   //a-z/A-Z
    //        (e.KeyValue >= 112 && e.KeyValue <= 123))   //F1-F12
    //    {
    //        keyValue.Append(e.KeyCode);
    //        k = e.KeyCode;
    //    }
    //    else if ((e.KeyValue >= 48 && e.KeyValue <= 57))    //0-9
    //    {
    //        keyValue.Append(e.KeyCode.ToString().Substring(1));
    //        k = e.KeyCode;
    //    }
    //    this.ActiveControl.Text = "";
    //    //设置当前活动控件的文本内容
    //    if (k != Keys.None)
    //    {
    //        HotKey.RegisterHotKey(Handle, 100, keyModifiers, k);
    //    }
    //    this.ActiveControl.Text = keyValue.ToString();
    //    e.SuppressKeyPress = false;
    //}
}