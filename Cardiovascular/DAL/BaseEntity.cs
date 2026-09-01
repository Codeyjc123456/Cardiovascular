using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 实体类，每个数据库表的实体类继承这个类
 * 完成时间：2016.08.03 15：00
 * 完成人：汪锡  ahuwx@mail.ustc.edu.cn
 */

namespace Cardio.DAL
{
    public class BaseEntity
    {
        public int Id { get; set; }
    }
}
