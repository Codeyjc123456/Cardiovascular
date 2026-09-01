using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    public class OnlineConfigDAL : BaseDAL<OnlineConfigEntity>
    {
        //定义单例对象
        private static OnlineConfigDAL onlineConfigDALInstance;
        //定义锁对象
        private static readonly object lockObj = new object();

        //public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        public List<string> filedNames = new List<string>();   //数据库字段名数组

        public OnlineConfigDAL()
        {
        }

        /// <summary>
        /// 定义静态方法获取唯一对象
        /// </summary>
        /// <param></param>
        /// <return></return>
        public static OnlineConfigDAL getInstance()
        {
            if (onlineConfigDALInstance == null)
            {
                lock (lockObj)
                {
                    if (onlineConfigDALInstance == null)
                    {
                        onlineConfigDALInstance = new OnlineConfigDAL();
                    }
                }
            }
            return onlineConfigDALInstance;
        }


    }
}
