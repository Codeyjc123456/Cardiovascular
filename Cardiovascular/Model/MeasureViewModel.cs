using Cardio.CustomRule;
using Cardio.DAL;
using HandyControl.Tools.Extension;
using ScottPlot;
using ScottPlot.WPF;
using System.Collections.ObjectModel;

namespace Cardio.Model
{
    public class MeasureViewModel : BaseViewModel, IDialogResultable<string>, IValidationExceptionHandler
    {

        private ObservableCollection<TipsStruct> tipsPPGList = new ObservableCollection<TipsStruct>();
        public ObservableCollection<TipsStruct> TipsPPGList
        {
            get => tipsPPGList;
            set => SetProperty(ref tipsPPGList, value);
        }
        #region 个人信息
        public PulseDataLocalEntity testdata = new();
        public List<double> AIData = [];
        public void InitDisplay()
        {
            UserID = testdata.userId;
            UserName = testdata.userName;
            UserHeight = testdata.userHeight.ToString();
            UserWeight = testdata.userWeight.ToString();
            userAge = testdata.userAge.ToString();
            UserSex = testdata.userSex;
            Sbp = testdata.Sbp.ToString();
            Dbp = testdata.Dbp.ToString();
            Map = testdata.Map.ToString();
            Hr = testdata.Hr.ToString();
            Spti = testdata.Spti.ToString();
            Dpti = testdata.Dpti.ToString();
            Cap = testdata.Sbp2.ToString();
            Sevr = testdata.Sevr.ToString();
            Pp = testdata.Pp.ToString();
            AIx = testdata.AIx.ToString();
            EdPct = testdata.EdPct.ToString();
        }
        private string userID = "--";
        public string UserID
        {
            get => userID;
            set => SetProperty(ref userID, value);
        }

        private string userName = "--";
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        private string userSex = "--";
        public string UserSex
        {
            get => userSex;
            set => SetProperty(ref userSex, value);
        }

        private string birthDay = "--";
        public string BirthDay
        {
            get => birthDay;
            set => SetProperty(ref birthDay, value);
        }

        private string userHeiht = "--";
        public string UserHeight
        {
            get => userHeiht;
            set => SetProperty(ref userHeiht, value);
        }
        private string userWeiht = "--";
        public string UserWeight
        {
            get => userWeiht;
            set => SetProperty(ref userWeiht, value);
        }
        private string userAge = "--";
        public string UserAge
        {
            get => userAge;
            set => SetProperty(ref userAge, value);
        }
        #endregion

        #region 图片
        private string ready = "pack://application:,,,/Resources/Image/Measure/测量准备.png";
        public string Ready
        {
            get => ready;
            set => SetProperty(ref ready, value);
        }
        private string measureBtn_Img = "pack://application:,,,/Resources/Image/Measure/开始测量.jpg";
        public string MeasureBtn_Img
        {
            get => measureBtn_Img;
            set => SetProperty(ref measureBtn_Img, value);
        }



        private string hrup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string HrUp
        {
            get => hrup;
            set => SetProperty(ref hrup, value);
        }

        private string edup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string EdUp
        {
            get => edup;
            set => SetProperty(ref edup, value);
        }

        private string sptiup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string SptiUp
        {
            get => sptiup;
            set => SetProperty(ref sptiup, value);
        }

        private string dptiup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string DptiUp
        {
            get => dptiup;
            set => SetProperty(ref dptiup, value);
        }



        private string sbpup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string SBpUp
        {
            get => sbpup;
            set => SetProperty(ref sbpup, value);
        }

        private string dbpup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string DBpUp
        {
            get => dbpup;
            set => SetProperty(ref dbpup, value);
        }

        private string ppup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string PpUp
        {
            get => ppup;
            set => SetProperty(ref ppup, value);
        }

        private string capup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string CapUp
        {
            get => capup;
            set => SetProperty(ref capup, value);
        }

        private string aixup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string AIxUp
        {
            get => aixup;
            set => SetProperty(ref aixup, value);
        }
        
        private string sevrup = "pack://application:,,,/Resources/Image/Measure/升高.png";
        public string SevrUp
        {
            get => sevrup;
            set => SetProperty(ref sevrup, value);
        }


        private string aiTest_Img = "pack://application:,,,/Resources/Image/Measure/心血管测试.png";
        public string AITest_Img
        {
            get => aiTest_Img;
            set => SetProperty(ref aiTest_Img, value);
        }



        #endregion

        #region 测量记录
        private ObservableCollection<DataGridModel> dataList = new ObservableCollection<DataGridModel>();
        public ObservableCollection<DataGridModel> DataList
        {
            get => dataList;
            set => SetProperty(ref dataList, value);
        }
        #endregion

        #region 测量指标
        private string testStatus = "";
        public string TestStatus
        {
            get => testStatus;
            set => SetProperty(ref testStatus, value);
        }
        private string hr = "--";
        public string Hr
        {
            get => hr;
            set => SetProperty(ref hr, value);
        }

        private string edpct = "--";
        public string EdPct
        {
            get => edpct;
            set => SetProperty(ref edpct, value);
        }

        private string spti = "--";
        public string Spti
        {
            get => spti;
            set => SetProperty(ref spti, value);
        }

        private string dpti = "--";
        public string Dpti
        {
            get => dpti;
            set => SetProperty(ref dpti, value);
        }

        private string pp = "--";
        public string Pp
        {
            get => pp;
            set => SetProperty(ref pp, value);
        }

        private string cap = "--";
        public string Cap
        {
            get => cap;
            set => SetProperty(ref cap, value);
        }

        private string sevr = "--";
        public string Sevr
        {
            get => sevr;
            set => SetProperty(ref sevr, value);
        }

        private string aix = "--";
        public string AIx
        {
            get => aix;
            set => SetProperty(ref aix, value);
        }

        private string sbp = "--";
        public string Sbp
        {
            get => sbp;
            set => SetProperty(ref sbp, value);
        }

        private string dbp = "--";
        public string Dbp
        {
            get => dbp;
            set => SetProperty(ref dbp, value);
        }
        private string map = "--";
        public string Map
        {
            get => map;
            set => SetProperty(ref map, value);
        }
        private string aiProposalDiag = "";
        public string AIProposalDiag
        {
            get => aiProposalDiag;
            set => SetProperty(ref aiProposalDiag, value);
        }
        private string aiDiagnosisProposal = "";
        public string AIDiagnosisProposal
        {
            get => aiDiagnosisProposal;
            set => SetProperty(ref aiDiagnosisProposal, value);
        }
        #endregion



        private string isRunning = "Hidden";
        public string IsRunning
        {
            get => isRunning;
            set => SetProperty(ref isRunning, value);
        }

        private string isRunningLoad = "Visible";
        public string IsRunningLoad
        {
            get => isRunningLoad;
            set => SetProperty(ref isRunningLoad, value);
        }

        private string network = "";
        public string Network
        {
            get => network;
            set => SetProperty(ref network, value);
        }

        private string version = "";
        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }

        private string tips = "";
        public string Tips
        {
            get => tips;
            set => SetProperty(ref tips, value);
        }

        #region 医师诊断基础信息


        public Action CloseAction { get; set; }

        private string diagnosisTime;
        public string DiagnosisTime
        {
            get => diagnosisTime;
            set => SetProperty(ref diagnosisTime, value);
        }

        private string cardiovasculardis = "";
        public string CardiovascularDIS
        {
            get => cardiovasculardis;
            set => SetProperty(ref cardiovasculardis, value);
        }

        private string doctordiagnosis = "";
        public string Doctordiagnosis
        {
            get => doctordiagnosis;
            set => SetProperty(ref doctordiagnosis, value);
        }

        private string doctorname = "";
        public string DoctorName
        {
            get => doctorname;
            set => SetProperty(ref doctorname, value);
        }

        private bool smoke = false;
        public bool Smoke
        {
            get => smoke;
            set => SetProperty(ref smoke, value);
        }

        private bool highbp = false;
        public bool HighBP
        {
            get => highbp;
            set => SetProperty(ref highbp, value);
        }

        private bool tangniaobing = false;
        public bool Tangniaobing
        {
            get => tangniaobing;
            set => SetProperty(ref tangniaobing, value);
        }

        private bool xuezhi = false;
        public bool Xuezhi
        {
            get => xuezhi;
            set => SetProperty(ref xuezhi, value);
        }

        private bool guanxinbing = false;
        public bool Guanxinbing
        {
            get => guanxinbing;
            set => SetProperty(ref guanxinbing, value);
        }

        private bool naozuzhong = false;
        public bool Naozuzhong
        {
            get => naozuzhong;
            set => SetProperty(ref naozuzhong, value);
        }

        private bool xinlishuaijie = false;
        public bool Xinlishuaijie
        {
            get => xinlishuaijie;
            set => SetProperty(ref xinlishuaijie, value);
        }

        private bool xinjiaotong = false;
        public bool Xinjiaotong
        {
            get => xinjiaotong;
            set => SetProperty(ref xinjiaotong, value);
        }

        private bool shenzangbing = false;
        public bool Shenzangbing
        {
            get => shenzangbing;
            set => SetProperty(ref shenzangbing, value);
        }

        private bool xinjigengsi = false;
        public bool Xinjigengsi
        {
            get => xinjigengsi;
            set => SetProperty(ref xinjigengsi, value);
        }

        #endregion
        #region 医师诊断图片
        private string imgselect = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgSelect
        {
            get => imgselect;
            set => SetProperty(ref imgselect, value);
        }

        private string imghighbp = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgHighBP
        {
            get => imghighbp;
            set => SetProperty(ref imghighbp, value);
        }

        private string imgtang = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgTang
        {
            get => imgtang;
            set => SetProperty(ref imgtang, value);
        }

        private string imgxuezhi = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgXuezhi
        {
            get => imgxuezhi;
            set => SetProperty(ref imgxuezhi, value);
        }

        private string imgguan = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgGuan
        {
            get => imgguan;
            set => SetProperty(ref imgguan, value);
        }

        private string imgnaozu = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgNaozu
        {
            get => imgnaozu;
            set => SetProperty(ref imgnaozu, value);
        }

        private string imgxinli = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgXinli
        {
            get => imgxinli;
            set => SetProperty(ref imgxinli, value);
        }

        private string imgxinjiaotong = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgXinjiaotong
        {
            get => imgxinjiaotong;
            set => SetProperty(ref imgxinjiaotong, value);
        }

        private string imgshenzang = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgShenzang
        {
            get => imgshenzang;
            set => SetProperty(ref imgshenzang, value);
        }

        private string imgxinji = "pack://application:,,,/Resources/Image/Measure/Border.png";
        public string ImgXinji
        {
            get => imgxinji;
            set => SetProperty(ref imgxinji, value);
        }
        public int RemoveAndAdd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Message { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Result { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        public void InitializeZedGraph(WpfPlot zg_ai)
        {
            var AICurve = zg_ai.Plot.Add.Signal(AIData);
            AICurve.LegendText = "";
            AICurve.Color = ScottPlot.Colors.Red;
            AICurve.Axes.YAxis = zg_ai.Plot.Axes.Left;
            zg_ai.Plot.Axes.Left.Min = 0;
            zg_ai.Plot.Axes.Left.Max = 4400;
            zg_ai.Plot.Axes.SetLimitsX(0, 6000);
            zg_ai.Plot.Axes.Bottom.IsVisible = false;
            zg_ai.Plot.Axes.Left.IsVisible = false;
            zg_ai.Plot.Axes.Right.IsVisible = false;
            zg_ai.Plot.Axes.Top.IsVisible = false;
            zg_ai.Plot.HideGrid();
            zg_ai.Plot.ShowLegend(Alignment.UpperCenter, Orientation.Horizontal);
            zg_ai.Refresh();
        }
    }
}