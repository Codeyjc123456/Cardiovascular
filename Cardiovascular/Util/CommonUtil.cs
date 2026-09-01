using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Util
{
    public static class CommonUtil
    {
        /// <summary>
        /// 将 数字 变为两位 字符串
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertTwoNumber(int number)
        {
            string s = number.ToString();
            if (number < 10)
            {
                s = "0" + s;
            }
            return s;
        }

        public static int ConvertPositionNumber(int number)
        {
            return number>=0?number:0;
        }

        public static double ConvertPositionNumber(double number)
        {
            return number >= 0 ? number : 0;
        }
    }
}
