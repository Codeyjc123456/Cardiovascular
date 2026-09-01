using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Model
{
    public class ParameterViewModel : BaseViewModel
    {
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

        private string _APP_CompaneTitle = "";
        public string APP_CompaneTitle
        {
            get => _APP_CompaneTitle;
            set => SetProperty(ref _APP_CompaneTitle, value);
        }


    }
}
