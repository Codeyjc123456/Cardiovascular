using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Util
{
    public static class ScreenTaskBar
    {
        private const int SwHide = 0; //隐藏窗口
        private const int SwRestore = 9;//还原窗口

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        /// <summary>
        /// 显示任务栏
        /// </summary>
        public static void Show()
        {
            ShowWindow(FindWindow("Shell_TrayWnd", null), SwRestore);
        }
        /// <summary>
        /// 隐藏任务栏
        /// </summary>
        public static void Hide()
        {
            ShowWindow(FindWindow("Shell_TrayWnd", null), SwHide);
        }
    }
}
