using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.SPCL
{
    public class Variable
    {
        #region 其他用到的公共变量
        public static int g_nUsbConnState = 0;//USB连接状态
        public static string strMrsLocation;//测量位置
        //public static string g_strSerialNumber;//产品序列号
        public static int ValSoftVersion;//软件版本 单机/网络
        public static int ValReportType;//报告类型  标准/病人
        public static int ValDeviceVersion;//设备版本：高配、标配
        public static EnumData.RunnigVersion g_enumVersion;
        public static EnumData.ReportType g_enumReportType;
        public static string g_strPath;//安装目录

        public static byte[] g_Key = new byte[12];
        public static byte[] g_PWVRunKey = new byte[2];//加密Key
        public static string Str_Client_Unit;//客户使用单位
        #endregion
       
        //public int g_AcquireP
        public static int Test_ABI_num = 0; //血压测试次数
        public static int Test_PWV_num = 0;//PWV测试次数
        public static int Test_AI_num = 0;//AI测试次数
        public static int Test_ECG_num = 0;//心电心音测试次数 

        public static List<double> LCpNormData = new List<double>();
        public static List<double> LFpNormData = new List<double>();
        public static List<double> RCpNormData = new List<double>();
        public static List<double> RFpNormData = new List<double>();

        public static List<double> ECGNormData = new List<double>();
        public static List<double> PCGNormData = new List<double>();

        public static List<double> AINormData = new List<double>();

        public static readonly int onceReadLength = 64;//一次采集数据长度
        public static readonly int Sample_Rate = 500;//采样率
        public static int QUE_BUFF_SIZE=128;//上位机缓存


        public static readonly int ScreenWidth = 1366;
        public static readonly int ScreenHeight = 768;
        public static string strSerialNumber="";//设备号

        public static string HKComPort;//华科模块串口号
        public static string BXComPort;//博谐模块串口号
        public static int Pressure;//关闭串口压力值
        public static int InintPressure;//预期压力

        //用于错误是收集数据
        public static string ErrorLCP;
        public static string ErrorLFP;
        public static string ErrorRCP;
        public static string ErrorRFP;
        public static List<double> ErrorECG;
        public static List<double> ErrorPCG;
        public static DateTime ErrorTime;

        public static string base64pdf;//报告的base64位数据
        //动脉弹性功能
        public struct MrsIndexValue
        {
            public int Hr;
            public double Ai;
            public double Sevr;
            public int CarSbp;
            public int CarDbp;
            public int CarPp;
            public int DataNum;
            public string DiagnosisResult;
            public string DiagnosisProposal;
            public static string Upload_Report_Name;
            public string DoctorDiagnosis;
            public string SignatureName;
            public string OperatingDoctor;
            public string DiagnosisTime;
           
        };
        public struct MrsBpValue//血压测量结果
        {
            public int Sbp;
            public int Dbp;
            public int Map;
        };
    }
    //数据缓存
    public class RxBuff
    {
        public static byte[,] Buff = new byte[128, 1920];
    }
    public class BpMrsResp
    {
        public static readonly int BpStart = 1;
        public static readonly int BpStop = 2;
    }
    public class ABIMrsResp
    {
        public static readonly int AbiStart = 1;
        public static readonly int AbiStop = 2;
    }
    public class PulseMrsResp
    {
        public static readonly int PulseStart = 1;
        public static readonly int PulseStop = 2;
    }

    public class MrsWaitForAck
    {
        public static readonly int MrsBP = 1;
        public static readonly int MrsPulse = 2;
        public static readonly int MrsPWV = 3;
    }
    public class PWVMrsResp
    {
        public static readonly int PwvStart = 1;
        public static readonly int PwvStop = 2;
        public static readonly int TOWBPStop = 3;
        public static readonly int VAClose = 4;
    }
    
    public class TestMode//测试模式 lobster
    {
        public static bool LBBP_Select;//左臂血压是否测试true：是，false：否；
        public static bool LABP_Select;//左踝血压
        public static bool RBBP_Select;//右臂血压
        public static bool RABP_Select;//右踝血压
        public static bool ECGPCG_Select;//心电心音
        public static bool AI_Select;
        public static byte CommandWordSend;
        public static byte ModelChange;
        public static bool Man_Select;//性别选择
        public static bool Woman_Select;
    }
    public class Record_Index
    {
        public static int AutoID;
        public static int IsContinue;//用来在测完血压后判段是否需要等待一段时间后再开启pwv测量
        public static string ID;//存储ID单机版是设备号+LoginID，网络版则相同
        public static string LoginID;//展示的ID
        public static string IsServerId;
        public static string Name;
        public static string sex;
        public static int Age;
        public static double Height;
        public static double Weight;
        public static string TestTime;

        public static int ABI_num;
        public static int BpHr;

        public static double Distance;

        public static int AI_num;
        public static double Ai;
        //桡动脉参数
        public static int Cap;
        public static int Sbp;
        public static int Dbp;
        public static int Pp;
        public static int Hr;
        public static int EdPct;
        public static int Spti;
        public static int Dpti;
        public static double Sevr;
        public static int Sbp2;
        public static double AIx;
        public static double Ed;
        public static string AIAssess;
        public static string AIDiagnonsisProproal;
        public static string AIDiagnonsisResult;
        //医师诊断
        public static string DoctorDiagnosis;
        //诊断日期
        public static string DiagnosisTime;
        //操作医师
        public static string OperatingDoctor;
        public static string Diagnosis_Result;
        public static string Diagnosis_Proposal;
        public static string IsRepertPrinted;
        public static string IsPDFReportPrinted;
        //心血管疾病危险因素
        public static string CardiovascularFactors;
        //患有的心血管疾病
        public static string CardiovascularDis;
        public static string ReportName;

    }
}
