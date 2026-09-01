using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Cardio.BLL
{
    public class SystemDateAndTime
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SystemTime sysTime);


        //定义时间结构体
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            //public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }

        /// <summary>
        /// 设置系统时间（需要管理员权限）
        /// </summary>
        /// <param name="newdatetime"></param>
        /// <returns></returns>
        public static bool SetSysTime(DateTime newdatetime)
        {
            SystemTime st = new SystemTime();
            GetLocalTime(ref st);

            st.wYear = (ushort)newdatetime.Year;
            st.wMonth = (ushort)newdatetime.Month;
            st.wDay = (ushort)newdatetime.Day;
            st.wHour = (ushort)newdatetime.Hour;
            st.wMinute = (ushort)newdatetime.Minute;
            st.wSecond = (ushort)newdatetime.Second;
            st.wMiliseconds = (ushort)newdatetime.Millisecond;
            return SetLocalTime(ref st);
        }
    }
}
