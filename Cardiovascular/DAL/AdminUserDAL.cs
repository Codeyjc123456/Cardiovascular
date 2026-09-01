using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    public class AdminUserDAL : BaseDAL<AdminUserEntity>
    {
        public static AdminUserDAL instance;
        //定义锁对象
        private static readonly object lockObj = new object();
        public static AdminUserDAL getInstance()
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                        instance = new AdminUserDAL();
                }
            }
            return instance;
        }
        public AdminUserDAL()
        {

        }
    }
}
