using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    public class SystemconfigDAL : BaseDAL<SystemconfigEntity>
    {
        private static SystemconfigDAL Instance;
        //定义锁对象
        private static readonly object LockObj = new object();
        public static SystemconfigDAL GetInstance()
        {
            if (Instance == null) lock (LockObj) Instance = new SystemconfigDAL();
            return Instance;
        }
        public SystemconfigDAL()  //构造函数，传入数据库连接信息/表名，添加字段名
        {
        }
    }
}
