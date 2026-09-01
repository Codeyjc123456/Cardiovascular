using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    public class DoctorInfoDAL:BaseDAL<DoctorInfoEntity>
    {
        //定义单例对象
        private static DoctorInfoDAL doctorInfoDALInstance;
        //定义锁对象
        private static readonly object lockObj = new object();
        //继承BaseDAL
        //public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        public List<string> filedNames = new List<string>();   //数据库字段名数组

        /// <summary>
        /// 定义静态方法获取唯一对象
        /// </summary>
        /// <param></param>
        /// <return></return>
        public static DoctorInfoDAL getInstance()
        {
            if (doctorInfoDALInstance == null)
            {
                lock (lockObj)
                {
                    if (doctorInfoDALInstance == null)
                    {
                        doctorInfoDALInstance = new DoctorInfoDAL();
                    }
                }
            }
            return doctorInfoDALInstance;
        }

        public DoctorInfoDAL()   //构造函数，传入数据库连接信息/表名，添加字段名
        {
           
        }

        
    }
}
