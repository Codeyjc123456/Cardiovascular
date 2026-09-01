using Cardio.CustomRule;
using Cardio.DAL;
using Cardio.Util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Cardio.Model
{
    public class APPSettingsViewModel : BaseViewModel
    {

        private readonly SystemconfigDAL configDAL = SystemconfigDAL.GetInstance();
        //定义单例对象
        private static APPSettingsViewModel Instance;
        private static readonly object LockObj = new object();

        private int _RemoveAndAdd = 0;
        public int RemoveAndAdd
        {
            get => _RemoveAndAdd;
            set => SetProperty(ref _RemoveAndAdd, value);
        }

        private string _Message;
        public string Message
        {
            get => _Message;
            set => SetProperty(ref _Message, value);
        }

        

      
        /// <summary>
        /// 定义静态方法获取唯一对象
        /// </summary>
        /// <param></param>
        /// <return></return>
        public static APPSettingsViewModel getInstance()
        {
            if (Instance == null)
            {
                lock (LockObj) if (Instance == null) Instance = new APPSettingsViewModel();
            }
            return Instance;
        }

        public int ShortWaitTime = 3;//非阻态时间 s


        #region 参数设置
        private string _APP_dataType = "";
        public string APP_dataType
        {
            get => _APP_dataType;
            set => SetProperty(ref _APP_dataType, value);
        }

        private string _APP_checkNo = "";
        public string APP_checkNo
        {
            get => _APP_checkNo;
            set => SetProperty(ref _APP_checkNo, value);
        }

        private string _APP_checkDoctor = "";
        public string APP_checkDoctor
        {
            get => _APP_checkDoctor;
            set => SetProperty(ref _APP_checkDoctor, value);
        }

        private string _APP_checkDepartment = "";
        public string APP_checkDepartment
        {
            get => _APP_checkDepartment;
            set => SetProperty(ref _APP_checkDepartment, value);
        }

        private string _APP_checkTime = "";
        public string APP_checkTime
        {
            get => _APP_checkTime;
            set => SetProperty(ref _APP_checkTime, value);
        }

        private string _APP_deviceModel = "";
        public string APP_deviceModel
        {
            get => _APP_deviceModel;
            set => SetProperty(ref _APP_deviceModel, value);
        }

        private string _APP_checkResult = "";
        public string APP_checkResult
        {
            get => _APP_checkResult;
            set => SetProperty(ref _APP_checkResult, value);
        }

        private string _APP_checkResultText = "";
        public string APP_checkResultText
        {
            get => _APP_checkResultText;
            set => SetProperty(ref _APP_checkResultText, value);
        }

        private string _APP_manufacturer = "";
        public string APP_manufacturer
        {
            get => _APP_manufacturer;
            set => SetProperty(ref _APP_manufacturer, value);
        }

        private string _APP_OwnerSet = "合肥中科博谐科技有限公司";
        public string APP_OwnerSet
        {
            get => _APP_OwnerSet;
            set => SetProperty(ref _APP_OwnerSet, value);
        }

        private string _APP_PWD = "";
        public string APP_PWD
        {
            get => _APP_PWD;
            set => SetProperty(ref _APP_PWD, value);
        }

        #endregion

       
        private string _APP_Printer = "";
        public string APP_Printer
        {
            get => _APP_Printer;
            set => SetProperty(ref _APP_Printer, value);
        }

        private string _APP_PrinterDialog = "";
        public string APP_PrinterDialog
        {
            get => _APP_PrinterDialog;
            set => SetProperty(ref _APP_PrinterDialog, value);
        }

        private string _APP_ApiUrlLogin = "";
        public string APP_ApiUrlLogin
        {
            get => _APP_ApiUrlLogin;
            set => SetProperty(ref _APP_ApiUrlLogin, value);
        }

        private string _APP_AutoUpload = "自动";
        public string APP_AutoUpload
        {
            get => _APP_AutoUpload;
            set => SetProperty(ref _APP_AutoUpload, value);
        }

        private string _APP_ApiUrlData = "";
        public string APP_ApiUrlData
        {
            get => _APP_ApiUrlData;
            set => SetProperty(ref _APP_ApiUrlData, value);
        }

        private string _APP_Network = "单机版";
        public string APP_Network
        {
            get => _APP_Network;
            set => SetProperty(ref _APP_Network, value);
        }

        private string _APP_DoctorName = "hfs";
        public string APP_DoctorName
        {
            get => _APP_DoctorName;
            set => SetProperty(ref _APP_DoctorName, value);
        }

        private string _APP_DoctorPWD = "123";
        public string APP_DoctorPWD
        {
            get => _APP_DoctorPWD;
            set => SetProperty(ref _APP_DoctorPWD, value);
        }

        private string _APP_CompaneTitle = "";
        public string APP_CompaneTitle
        {
            get => _APP_CompaneTitle;
            set => SetProperty(ref _APP_CompaneTitle, value);
        }

        private string _APP_Version = "";
        public string APP_Version
        {
            get => _APP_Version;
            set => SetProperty(ref _APP_Version, value);
        }

        private string _APP_Type = "";
        public string APP_Type
        {
            get => _APP_Type;
            set => SetProperty(ref _APP_Type, value);
        }

        private string _APP_debugPwd = "";
        public string APP_debugPwd
        {
            get => _APP_debugPwd;
            set => SetProperty(ref _APP_debugPwd, value);
        }

        private string _APP_BPPort = "";
        public string APP_BPPort
        {
            get => _APP_BPPort;
            set => SetProperty(ref _APP_BPPort, value);
        }

        private string _IsHasBP = "关闭";
        public string IsHasBP
        {
            get => _IsHasBP;
            set => SetProperty(ref _IsHasBP, value);
        }
        private string _APP_COMRate;
        public string APP_COMRate
        {
            get => _APP_COMRate;
            set=>SetProperty(ref _APP_COMRate, value);
        }
        private string _APP_CorSbp;
        public string APP_CorSbp
        {
            get => _APP_CorSbp;
            set => SetProperty(ref _APP_CorSbp, value);
        }
        private string _APP_CorDbp;
        public string APP_CorDbp
        {
            get => _APP_CorDbp;
            set => SetProperty(ref _APP_CorDbp, value);
        }
        private string _APP_CorHr;
        public string APP_CorHr
        {
            get => _APP_CorHr;
            set => SetProperty(ref _APP_CorHr, value);
        }
        private string _APP_CorMap;
        public string APP_CorMap
        {
            get => _APP_CorMap;
            set => SetProperty(ref _APP_CorMap, value);
        }
        public bool LoadData()
        {
            SystemconfigEntity systemConfig = configDAL?.FindByID(1);
            if (systemConfig != null)
            {
                APP_DoctorName = systemConfig.APP_DoctorName;
                APP_DoctorPWD = systemConfig.APP_DoctorPWD;

                APP_dataType = systemConfig.APP_dataType;
                APP_checkNo = systemConfig.APP_checkNo;
                APP_checkDoctor = systemConfig.APP_checkDoctor;
                APP_checkDepartment = systemConfig.APP_checkDepartment;
                APP_checkTime = systemConfig.APP_checkTime;
                APP_deviceModel = systemConfig.APP_deviceModel;
                APP_checkResult = systemConfig.APP_checkResult;
                APP_checkResultText = systemConfig.APP_checkResultText;
                APP_manufacturer = systemConfig.APP_manufacturer;

                APP_PWD = systemConfig.APP_PWD;
                APP_Network = systemConfig.APP_Network;
                APP_ApiUrlLogin = systemConfig.APP_ApiUrlLogin;
                APP_ApiUrlData = systemConfig.APP_ApiUrlData;

                APP_Version = systemConfig.APP_Version;
                APP_Type = systemConfig.APP_Type;
                APP_OwnerSet = systemConfig.APP_OwnerSet;

                //调试密码
                APP_debugPwd = systemConfig.APP_debugPwd;

                APP_BPPort = systemConfig.APP_BPPort;
                APP_CompaneTitle = systemConfig.APP_CompaneTitle;

                APP_Printer = systemConfig.APP_Printer;
                APP_PrinterDialog = systemConfig.APP_PrinterDialog;
                APP_COMRate=systemConfig.APP_COMRate;

                APP_CorSbp = systemConfig.APP_CorSbp;
                APP_CorDbp = systemConfig.APP_CorDbp;
                APP_CorMap = systemConfig.APP_CorMap;
                APP_CorHr = systemConfig.APP_CorHr;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SaveData()
        {
            SystemconfigEntity systemConfig = configDAL?.FindByID(1);
            if (systemConfig != null)
            {
                systemConfig.APP_DoctorName = APP_DoctorName;
                systemConfig.APP_DoctorPWD = APP_DoctorPWD;

                systemConfig.APP_dataType = APP_dataType;
                systemConfig.APP_checkNo = APP_checkNo;
                systemConfig.APP_checkDoctor = APP_checkDoctor;
                systemConfig.APP_checkDepartment = APP_checkDepartment;
                systemConfig.APP_checkTime = APP_checkTime;
                systemConfig.APP_deviceModel = APP_deviceModel;
                systemConfig.APP_checkResult = APP_checkResult;
                systemConfig.APP_checkResultText = APP_checkResultText;
                systemConfig.APP_manufacturer = APP_manufacturer;

                systemConfig.APP_PWD = APP_PWD;
                systemConfig.APP_Network = APP_Network;
                systemConfig.APP_ApiUrlLogin = APP_ApiUrlLogin;
                systemConfig.APP_ApiUrlData = APP_ApiUrlData;

                systemConfig.APP_Version = APP_Version;
                systemConfig.APP_Type = APP_Type;
                systemConfig.APP_OwnerSet = APP_OwnerSet;

                //调试密码
                systemConfig.APP_debugPwd = APP_debugPwd;

                systemConfig.APP_BPPort = APP_BPPort;
                systemConfig.APP_CompaneTitle = APP_CompaneTitle;

                systemConfig.APP_Printer = APP_Printer;
                systemConfig.APP_PrinterDialog = APP_PrinterDialog;
                systemConfig.APP_COMRate = APP_COMRate;

                systemConfig.APP_CorHr = APP_CorHr;
                systemConfig.APP_CorSbp = APP_CorSbp;
                systemConfig.APP_CorDbp = APP_CorDbp;
                systemConfig.APP_CorMap = APP_CorMap;
                return configDAL?.Update(systemConfig) ?? false;
            }
            return false;
        }
    }
}
