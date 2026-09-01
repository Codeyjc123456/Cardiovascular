using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.BLL
{
    public class StringMergeSplit
    {
        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        /// <summary>
        /// 将分号分隔的string（测量数据）转换成List<string>数据
        /// </summary>
        /// <param name=list"></param>
        public List<string> SplitString(string list)
        {
            List<string> testData = new List<string>();
            try
            {
                string[] str = list.Split(';');
                foreach (var i in str)
                {
                    testData.Add(i);
                }
            }
            catch (Exception ex)
            {
                loginfo.Error(ex.Message);
            }
            return testData;
        }

        /// <summary>
        /// 将list合并为字符串，中间以分号分割
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string MergeString(List<string> list)
        {
            string result = "";

            foreach (string element in list)
            {
                result = result + element + ";";
            }
            try
            {
                result = result.Remove(result.LastIndexOf(";"), 1);
            }
            catch (Exception ex)
            {
                loginfo.Error(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 将list合并为字符串，中间以分号分割
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string MergeString(List<DateTime> list)
        {
            string result = "";

            foreach (DateTime element in list)
            {
                result = result + element.ToString() + ";";
            }
            try
            {
                result = result.Remove(result.LastIndexOf(";"), 1);
            }
            catch (Exception ex)
            {
                loginfo.Error(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 将list合并为字符串，中间以分号分割
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string MergeString(List<double> list)
        {
            string result = "";
            try
            {
                foreach (double element in list)
                {
                    result = result + element.ToString("f3") + ";";
                }
                result = result.Remove(result.LastIndexOf(";"), 1);
            }
            catch (Exception ex)
            {
                loginfo.Error(ex.Message);
            }
            return result;
        }
        public string MergeString(List<int> list)
        {
            string result = "";

            
            try
            {
                foreach (double element in list)
                {
                    result = result + element + ";";
                }
                result = result.Remove(result.LastIndexOf(";"), 1);
            }
            catch (Exception ex)
            {
                loginfo.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 将字符型数组合并为字符串，中间以分号分割
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string MergeString(string[] list)
        {
            string result = "";

            foreach (string element in list)
            {
                result = result + element + ";";
            }
            try
            {
                result = result.Remove(result.LastIndexOf(";"), 1);
            }
            catch(Exception ex)
            {
                loginfo.Error(ex.Message);
            }
            return result;
        }

    }
}
