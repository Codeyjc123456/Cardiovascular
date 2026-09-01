using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Model
{
    public class FactoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        
        private string version;
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Version"));
            }
        }

        private string isVersion;
        public string IsVersion
        {
            get { return isVersion; }
            set
            {
                isVersion = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsVersion"));
            }
        }

        private string app_PWD;
        public string APP_PWD
        {
            get { return app_PWD; }
            set
            {
                app_PWD = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("APP_PWD"));
            }
        }

        private string isAdjust;
        public string IsAdjust
        {
            get { return isAdjust; }
            set
            {
                isAdjust = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsAdjust"));
            }
        }


       


        private string isHasBP = "已连接";
        public string IsHasBP
        {
            get { return isHasBP; }
            set
            {
                isHasBP = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsHasBP"));
            }
        }

        private string isHasECG = "已连接";
        public string IsHasECG
        {
            get { return isHasECG; }
            set
            {
                isHasECG = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsHasECG"));
            }
        }

        private string bPCOM = "COM3";
        public string BPCOM
        {
            get { return bPCOM; }
            set
            {
                bPCOM = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BPCOM"));
            }
        }

        private string eCGCOM = "COM3";
        public string ECGCOM
        {
            get { return eCGCOM; }
            set
            {
                eCGCOM = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ECGCOM"));
            }
        }
    }
}
