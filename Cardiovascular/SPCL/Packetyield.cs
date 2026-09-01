using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.SPCL
{
    public class Packetyield
    {
        public static List<int> PacketYield = new List<int>();
        public static int countNum = 0;
        public static int Xvalue = 0;
        public static string workStatus;//用来将调试界面的工作状态保存，用来设置界面点击关闭按钮时判断


        public static string temp;//保存登录界面的温度
    }
    public static class CheckedHealth//健康状态
    {
        public static bool cb_0 { get; set; }//健康
        public static bool cb_1 { get; set; }//胰腺炎
        public static bool cb_2 { get; set; }//糖尿病
        public static bool cb_3 { get; set; }//肥胖
        public static bool cb_4 { get; set; }//血脂异常
        public static bool cb_5 { get; set; }//高血压
        public static bool cb_6 { get; set; }//其他
    }
    public static class CheckedActivity
    {
        public static bool cb_0 { get; set; }//卧床
        public static bool cb_1 { get; set; }//静坐为主
        public static bool cb_2 { get; set; }//轻体力活动
        public static bool cb_3 { get; set; }//中度体力活动
        public static bool cb_4 { get; set; }//重体力活动
        public static bool cb_5 { get; set; }//卧床，几乎无法
        public static bool cb_6 { get; set; }//卧床，可以做
        public static bool cb_7 { get; set; }//可以洗浴
        public static bool cb_8 { get; set; }//每天可以在走廊

        public static bool  cb_N { get; set; }//普通患者
        public static bool cb_Y { get; set; }//住院患者
    }
}
