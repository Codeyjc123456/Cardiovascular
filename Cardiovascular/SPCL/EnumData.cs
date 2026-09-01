using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.SPCL
{
    public class EnumData
    {
        public enum AgeLevel
        {
            cnr = 1,//普通成人
            lr = 2,//老年人
            yf = 3,//孕产妇
        }

        public enum HealthLevel
        {
            zc = 1,//正常
            xyyc = 2,//血压异常
            sjgxy = 3,//三级高血压
            xzzsyc = 4,//心脏指数异常
            aiyc = 5,//AI异常
        }

        public enum HealthAlert
        {
            //修改此处以后，注意修改下方类中的函数。
            xlsc = 1,//心律失常
            gxy = 2,//高血压
            dxy = 3,//低血压
            gxb = 4,//冠心病
            xbmjb = 5,//心瓣膜疾病
            px = 6,//低血容量、贫血
            gxyxxzb = 7,//高血压性心脏病
            dmyh = 8,//动脉硬化
            xzyc = 9,//血脂异常
            tlb = 10,//糖尿病
            xjqx = 11,//心肌缺血
            nzz = 12,//脑卒中
            xzsj = 13,//心脏衰竭
            jk = 14,//甲亢
            fscdzz = 15,//房室传导阻滞
            xdgs = 16,//心动过速
            xlgh = 17,//心率过缓
            msxhbl = 18, //末梢循环不良
            xlsj = 19,//心力衰竭
            xjb = 20,//心肌病
            fxb = 21,//肺心病
            xngs = 22,//心脑梗塞
            zf = 23,//中风
        }

        public class IllsChName
        {
            /// <summary>
            /// 获取病的中文名
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static string GetIllChName(HealthAlert value)
            {
                switch (value)
                {
                    case HealthAlert.dmyh:
                        return "动脉硬化";
                    case HealthAlert.dxy:
                        return "低血压";
                    case HealthAlert.fscdzz:
                        return "房室传导阻滞";
                    case HealthAlert.fxb:
                        return "肺心病";
                    case HealthAlert.gxb:
                        return "冠心病";
                    case HealthAlert.gxy:
                        return "高血压";
                    case HealthAlert.gxyxxzb:
                        return "高血压性心脏病";
                    case HealthAlert.jk:
                        return "甲亢";
                    case HealthAlert.msxhbl:
                        return "末梢循环不良";
                    case HealthAlert.nzz:
                        return "脑卒中";
                    case HealthAlert.px:
                        return "贫血";
                    case HealthAlert.tlb:
                        return "糖尿病";
                    case HealthAlert.xbmjb:
                        return "心瓣膜疾病";
                    case HealthAlert.xdgs:
                        return "心动过速";
                    case HealthAlert.xjb:
                        return "心肌病";
                    case HealthAlert.xjqx:
                        return "心肌缺血";
                    case HealthAlert.xlgh:
                        return "心率过缓";
                    case HealthAlert.xlsc:
                        return "心律失常";
                    case HealthAlert.xlsj:
                        return "心力衰竭";
                    case HealthAlert.xngs:
                        return "心脑梗塞";
                    case HealthAlert.xzsj:
                        return "心脏衰竭";
                    case HealthAlert.xzyc:
                        return "血脂异常";
                    case HealthAlert.zf:
                        return "中风";
                    default:
                        return string.Empty;
                }
            }

            public static string GetIllChName(string value)
            {
                return GetIllChName((HealthAlert)Enum.Parse(typeof(HealthAlert), value));
            }
        }
        public enum HealthDataTarget
        {
            HR = 1,
            ED = 2,
            SPTI = 3,
            DPTI = 4,
            SEVR = 5,
            SBP = 6,
            DBP = 7,
            PP = 8,
            Cap = 9,
            AI = 10,
            WAVE = 11,
        }
        public enum AlertType
        {
            Confrim = 0,
            Alert = 1,
        }
        public enum TestInterval
        {
            Days = 15,
        }
        public enum TestInterval2
        {
            Days = 15,
        }
        /// <summary>
        /// LOBSTER
        /// </summary>
        public enum MeasureMode
        {
            ExpertMode = 0,
            ExperienceMode = 1,
        }
        public enum RunnigVersion
        {
            LocalVersion = 0,//本地登录
            ServerVersion = 1,
            AutoVersion = 2,
            UnknownVersion = 2,
        }
        public enum ReportType
        {
            Standard = 0,
            Patient = 1,
            UnknownType = 2,
        }

        //public enum BPX_LIMB
        //{
        //    BPX_LEFT_BRA = 0x1,//  '左上臂-0x01
        //    BPX_LEFT_ANK = 0x2,//        '左脚踝-0x02
        //    BPX_RIGHT_BRA = 0x3,//       '右上臂-0x03
        //    BPX_RIGHT_ANK = 0x4,//       '右脚踝-0x04
        //    BPX_FOUR_LIMB = 0x5,//       '四肢-0x05
        //    BPX_TOW_LIMB = 0x6,//        '左侧-0x06
        //}


    }
}
