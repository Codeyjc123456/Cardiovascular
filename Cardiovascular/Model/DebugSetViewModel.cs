using Cardio.Model;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardioVascular.Model
{
    public class DebugSetViewModel : BaseViewModel
    {
        #region
        private ObservableCollection<TipsStruct> _TipsPPGList = new ObservableCollection<TipsStruct>();
        public ObservableCollection<TipsStruct> TipsPPGList
        {
            get => _TipsPPGList;
            set => SetProperty(ref _TipsPPGList, value);
        }

        //预期压力
        private string _Pressure = "80";
        public string Pressure
        {
            get => _Pressure;
            set => SetProperty(ref _Pressure, value);
        }

        //SPB
        private string _CorrectLBSbp = "";
        public string CorrectLBSbp
        {
            get => _CorrectLBSbp;
            set => SetProperty(ref _CorrectLBSbp, value);
        }

        //DBP
        private string _CorrectLBDbp = "";
        public string CorrectLBDbp
        {
            get => _CorrectLBDbp;
            set => SetProperty(ref _CorrectLBDbp, value);
        }
        #endregion

        #region
        private string _SBP = "--";
        public string SBP
        {
            get => _SBP;
            set => SetProperty(ref _SBP, value);
        }

        private string _DBP = "--";
        public string DBP
        {
            get => _DBP;
            set => SetProperty(ref _DBP, value);
        }

        private string _MAP = "--";
        public string MAP
        {
            get => _MAP;
            set => SetProperty(ref _MAP, value);
        }

        private string _BPHr = "--";
        public string BPHr
        {
            get => _BPHr;
            set => SetProperty(ref _BPHr, value);
        }

        private string _RealPressure = "--";
        public string RealPressure
        {
            get => _RealPressure;
            set => SetProperty(ref _RealPressure, value);
        }

        private string _ReturnCode = "--";
        public string ReturnCode
        {
            get => _ReturnCode;
            set => SetProperty(ref _ReturnCode, value);
        }
        #endregion

        #region
        private string _HR = "--";
        public string HR
        {
            get => _HR;
            set => SetProperty(ref _HR, value);
        }

        private string _ED = "--";
        public string ED
        {
            get => _ED;
            set => SetProperty(ref _ED, value);
        }

        private string _SPTI = "--";
        public string SPTI
        {
            get => _SPTI;
            set => SetProperty(ref _SPTI, value);
        }

        private string _Dpti = "--";
        public string Dpti
        {
            get => _Dpti;
            set => SetProperty(ref _Dpti, value);
        }

        private string _Sever = "--";
        public string Sever
        {
            get => _Sever;
            set => SetProperty(ref _Sever, value);
        }


        #endregion
        #region 桡动脉波形调试指标
        private string debughr = "--";
        public string DebugHR
        {
            get => debughr;
            set => SetProperty(ref debughr, value);
        }

        private string period = "--";
        public string Period
        {
            get => period;
            set => SetProperty(ref period, value);

        }

        private string number = "--";
        public string Number
        {
            get => number;
            set => SetProperty(ref number, value);

        }
        private string rate = "--";
        public string Rate
        {
            get => rate;
            set => SetProperty(ref rate, value);

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
        #endregion
    }
}
