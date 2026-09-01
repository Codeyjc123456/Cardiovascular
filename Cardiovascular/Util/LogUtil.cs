using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Util
{
    public static class LogUtil
    {
        /// <summary>
        /// loginfo
        /// </summary>
        private static ILog loginfo = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Info(string title, string msg)
        {
            loginfo.Info(DateTime.Now + " " + title + " " + msg);
        }
        public static void Info(string msg)
        {
            loginfo.Info(",,,"+msg);
        }
        public static void Error(string title, string msg)
        {
            loginfo.Error(DateTime.Now + " " + title + " " + msg);
        }
        public static void Warn(string title, string msg)
        {
            loginfo.Warn(DateTime.Now + " " + title + " " + msg);
        }
        public static void Warn(string msg)
        {
            loginfo.Warn(",,," + msg);
        }
    }
}
