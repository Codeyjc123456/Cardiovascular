using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 自定义一个新的数据类型，继承与List，可实现固定长度
 * 完成时间：2016.08.05  09：54
 * 完成人：汪锡 ahuwx@mail.ustc.edu.cn
 */

namespace Cardio.Algorithm
{
    /// <summary>
    /// 固定长度的List,继承于List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixLenList <T>:List<T>
    {
        private int FixLen=0;
        public FixLenList(int length)
        {
            FixLen = length;
        }

        /// <summary>
        /// 添加一个元素
        /// </summary>
        /// <param name="item">待添加的元素</param>
        /// <returns></returns>
        public FixLenList<T> AddOne(T item)
        {
            this.Add(item);

            while (this.Count >FixLen )
            {
                this.RemoveAt(0);
            }

            return this;
        }
    }
}
