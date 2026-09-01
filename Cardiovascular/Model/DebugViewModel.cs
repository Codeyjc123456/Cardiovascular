using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Model
{
    public class DebugViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<TipsStruct> tipsPPGList = new ObservableCollection<TipsStruct>();
        public ObservableCollection<TipsStruct> TipsPPGList
        {
            get { return tipsPPGList; }
            set
            {
                tipsPPGList = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("TipsPPGList"));
            }
        }
        #region 计算指标
        private string hr = "--";
        public string HR
        {
            get { return hr; }
            set
            {
                hr = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("HR"));
            }
        }

        private string period = "--";
        public string Period
        {
            get { return period; }
            set
            {
                period = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Period"));
            }
        }

        private string number = "--";
        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Number"));
            }
        }
        private string rate = "--";
        public string Rate
        {
            get { return rate; }
            set
            {
                rate = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Rate"));
            }
        }

        private string getAirStatusBtn_Img = "pack://application:,,,/Resources/Image/system/获取环境状态.png";
        public string GetAirStatusBtn_Img
        {
            get { return getAirStatusBtn_Img; }
            set
            {
                getAirStatusBtn_Img = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("GetAirStatusBtn_Img"));
            }
        }
        #endregion
        #region  设置字段
        private string bpCOM = "";
        public string BPCOM
        {
            get { return bpCOM; }
            set
            {
                bpCOM = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BPCOM"));
            }
        }
        private string correctLBSbp = "";
        public string CorrectLBSbp
        {
            get { return correctLBSbp; }
            set
            {
                correctLBSbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CorrectLBSbp"));
            }
        }


        private string correctLBDbp = "";
        public string CorrectLBDbp
        {
            get { return correctLBDbp; }
            set
            {
                correctLBDbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CorrectLBDbp"));
            }
        }
        private string correctRBSbp;
        public string CorrectRBSbp
        {
            get { return correctRBSbp; }
            set
            {
                correctRBSbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CorrectRBSbp"));
            }
        }


        private string correctRBDbp = "";
        public string CorrectRBDbp
        {
            get { return correctRBDbp; }
            set
            {
                correctRBDbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CorrectRBDbp"));
            }
        }
        private string correctLASbp;
        public string CorrectLASbp
        {
            get { return correctLASbp; }
            set
            {
                correctLASbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CorrectLASbp"));
            }
        }


        private string correctLADbp = "";
        public string CorrectLADbp
        {
            get { return correctLADbp; }
            set
            {
                correctLADbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CorrectLADbp"));
            }
        }
        private string correctRASbp;
        public string CorrectRASbp
        {
            get { return correctRASbp; }
            set
            {
                correctRASbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CorrectRASbp"));
            }
        }


        private string correctRADbp = "";
        public string CorrectRADbp
        {
            get { return correctRADbp; }
            set
            {
                correctRADbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CorrectRADbp"));
            }
        }

        private string pressure = "0";
        public string Pressure
        {
            get { return pressure; }
            set
            {
                pressure = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Pressure"));
            }
        }

        private string sbp = "--";
        public string SBP
        {
            get { return sbp; }
            set
            {
                sbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SBP"));
            }
        }
        private string dbp = "--";
        public string DBP
        {
            get { return dbp; }
            set
            {
                dbp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DBP"));
            }
        }

        private string map = "--";
        public string MAP
        {
            get { return map; }
            set
            {
                map = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MAP"));
            }
        }

        private string bpHr = "--";
        public string BPHr
        {
            get { return bpHr; }
            set
            {
                bpHr = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BPHr"));
            }
        }

        private string realPressure = "--";
        public string RealPressure
        {
            get { return realPressure; }
            set
            {
                realPressure = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RealPressure"));
            }
        }
        private string controlTempStatusO2 = "--";
        public string ControlTempStatusO2
        {
            get { return controlTempStatusO2; }
            set
            {
                controlTempStatusO2 = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ControlTempStatusO2"));
            }
        }
        private string returnCode = "--";
        public string ReturnCode
        {
            get { return returnCode; }
            set
            {
                returnCode = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ReturnCode"));
            }
        }/// <summary>
        /// 血压测量控制
        /// </summary>
        private string bpCB = "--";
        public string BPCB
        {
            get { return bpCB; }
            set
            {
                bpCB = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BPCB"));
            }
        }
        /// <summary>
        /// 脉搏测量控制
        /// </summary>
        private string pwvCB = "--";
        public string PWVCB
        {
            get { return pwvCB; }
            set
            {
                pwvCB = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PWVCB"));
            }
        }
        /// <summary>
        /// 阀门控制
        /// </summary>
        private string bpcontrolCB = "--";
        public string BPcontrolCB
        {
            get { return bpcontrolCB; }
            set
            {
                bpcontrolCB = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BPcontrolCB"));
            }
        }
        

        private string getDeviceStatusBtn_Img = "pack://application:,,,/Resources/system/获取设备状态.png";
        public string GetDeviceStatusBtn_Img
        {
            get { return getDeviceStatusBtn_Img; }
            set
            {
                getDeviceStatusBtn_Img = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("GetDeviceStatusBtn_Img"));
            }
        }

        private string measureConBtn_Img = "pack://application:,,,/Resources/Image/system/开始测量.png";
        public string MeasureConBtn_Img
        {
            get { return measureConBtn_Img; }
            set
            {
                measureConBtn_Img = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MeasureConBtn_Img"));
            }
        }
        #endregion
    }
}
