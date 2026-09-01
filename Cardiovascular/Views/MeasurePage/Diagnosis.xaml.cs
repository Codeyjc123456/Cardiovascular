using Cardio.Model;
using System.Windows;
using System.Windows.Controls;
using Cardio.DAL;

namespace Cardio.Views.MeasurePage
{
    /// <summary>
    /// Diagnosis.xaml 的交互逻辑
    /// </summary>
    public partial class Diagnosis : Border
    {
        /// <summary>
        /// 数据绑定
        /// </summary>
        #region
        #endregion
        DoctorInfoDAL doctorInfoDAL = null;//定义医师操作类
        DoctorInfoEntity doctorInfo = new ();
        MeasureViewModel doctorViewModel = new ();
        PulseDataLocalDAL dataDAL = null;
        PulseDataLocalEntity pulsedata = new ();
        List<DoctorInfoEntity> doctorlist = new();
        public Diagnosis(PulseDataLocalEntity testData)
        {
            InitializeComponent();
            pulsedata = testData;
            DataContext = doctorViewModel;
        }
        private void AddDoctor(object sender, RoutedEventArgs e)
        {
            doctorInfo.DoctorName = CbDoctor.Text;
            if (!CbDoctor.Items.Contains(doctorInfo.DoctorName))
            {
                if (doctorInfoDAL.Insert(doctorInfo) > 0)
                {
                    HandyControl.Controls.Growl.Success("医师添加成功！", "SuccessMsg");
                    {
                        GC.Collect();
                    }
                }
            }
            else
            {
                HandyControl.Controls.Growl.Warning("该医师已经存在，请勿重复添加！", "SuccessMsg");
                {
                    GC.Collect();
                }
            }
        }
        /// <summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_Load(object sender, RoutedEventArgs e)
        {
            dataDAL = PulseDataLocalDAL.getInstance();
            doctorInfoDAL = DoctorInfoDAL.getInstance();
            doctorlist = doctorInfoDAL.Finds("1 = 1");
            List<string> doclist = new List<string>();
            foreach (DoctorInfoEntity doctor in doctorlist)
            {
                //doclist.Add(doctor.DoctorName);
                CbDoctor.Items.Add(doctor.DoctorName);  //将医师表中的医师姓名赋值到文本框中
            }
            CbDoctor.SelectedIndex = 0;
            doctorViewModel.DiagnosisTime = DateTime.Now.ToString("yyyy-MM-dd");
            doctorViewModel.CardiovascularDIS = pulsedata.CardiovascularDis;
            Intshow();

        }
        private void Intshow()
        {   //Contain
            if (pulsedata.DiagnosisTime != null)
            {
                doctorViewModel.DiagnosisTime = pulsedata.DiagnosisTime;
            }
            if (pulsedata.OperationgDoctor != null)
            {
                CbDoctor.Text = pulsedata.OperationgDoctor;
            }
            CardiovascularFactorsShow();

        }

        private void CardiovascularFactorsShow()
        {

            if (pulsedata.CardiovascularFactors == "吸烟；高血压；糖尿病；血脂异常")
            {
            doctorViewModel.Smoke = true;
            doctorViewModel.HighBP = true;
            doctorViewModel.Tangniaobing = true;
            doctorViewModel.Xuezhi = true;
            //Xuezhiyichang.Visibility = Visibility.Visible;
            doctorViewModel.ImgSelect = "pack://application:,,,/Resources/Image/Measure/Check.png";
            doctorViewModel.ImgHighBP = "pack://application:,,,/Resources/Image/Measure/Check.png";
            doctorViewModel.ImgTang = "pack://application:,,,/Resources/Image/Measure/Check.png";
            doctorViewModel.ImgXuezhi = "pack://application:,,,/Resources/Image/Measure/Check.png";

        }
            else if (pulsedata.CardiovascularFactors == "吸烟；高血压；糖尿病")
            {
            ;
            doctorViewModel.Smoke = true;
                doctorViewModel.HighBP = true; 
                doctorViewModel.Tangniaobing = true;
            doctorViewModel.ImgSelect = "pack://application:,,,/Resources/Image/Measure/Check.png";
            doctorViewModel.ImgHighBP = "pack://application:,,,/Resources/Image/Measure/Check.png";
            doctorViewModel.ImgTang = "pack://application:,,,/Resources/Image/Measure/Check.png";
            }
            else if (pulsedata.CardiovascularFactors =="吸烟；高血压；血脂异常")
            {
            doctorViewModel.Smoke = true;
            doctorViewModel.HighBP = true;
            doctorViewModel.Xuezhi = true;
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.Tangniaobing == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；糖尿病；血脂异常";
            }
            else if (doctorViewModel.HighBP == true && doctorViewModel.Tangniaobing == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "高血压；糖尿病；血脂异常";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.HighBP == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；高血压";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.Tangniaobing == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；糖尿病";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；血脂异常";
            }
            else if (doctorViewModel.HighBP == true && doctorViewModel.Tangniaobing == true)
            {
                pulsedata.CardiovascularFactors = "高血压；糖尿病";
            }
            else if (doctorViewModel.HighBP == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "高血压；血脂异常";
            }
            else if (doctorViewModel.Tangniaobing == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "糖尿病；血脂异常";
            }
            else if (doctorViewModel.Smoke == true)
            {
                pulsedata.CardiovascularFactors = "吸烟";
            }
            else if (doctorViewModel.HighBP == true)
            {
                pulsedata.CardiovascularFactors = "高血压";
            }
            else if (doctorViewModel.Tangniaobing == true)
            {
                pulsedata.CardiovascularFactors = "糖尿病";
            }
            else if (doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "血脂异常";
            }
            else
            {
                pulsedata.CardiovascularFactors = "";
            }

            if (pulsedata.CardiovascularFactors == "血脂异常")
            {
                doctorViewModel.Xuezhi = true;
                //Xuezhiyichang.Visibility = Visibility.Visible;
                doctorViewModel.ImgXuezhi = "pack://application:,,,/Resources/Image/Measure/Check.png";
            }
        }

        //
        private void SaveDiagnosis(object sender, RoutedEventArgs e)
        {
            SeekReason();
            Disease();
            if (doctorViewModel.CardiovascularDIS != "")
            {
                pulsedata.CardiovascularDis = doctorViewModel.CardiovascularDIS;
            }
            pulsedata.DoctorDiagnosis = doctorViewModel.Doctordiagnosis;
            pulsedata.OperationgDoctor = doctorViewModel.DoctorName;
            pulsedata.DiagnosisTime = doctorViewModel.DiagnosisTime;
            pulsedata.OperationgDoctor = CbDoctor.Text;
            bool isSucess = dataDAL.Update(pulsedata);   //医师诊断更新
            doctorViewModel.CloseAction?.Invoke();
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            doctorViewModel.CloseAction?.Invoke();
        }
        /// <summary>
        /// 心血管疾病危险因素
        /// </summary>
        private void SeekReason()
        {
            if (doctorViewModel.Smoke  && doctorViewModel.HighBP  && doctorViewModel.Tangniaobing  && doctorViewModel.Xuezhi)
            {
                pulsedata.CardiovascularFactors = "吸烟；高血压；糖尿病；血脂异常";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.HighBP == true && doctorViewModel.Tangniaobing == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；高血压；糖尿病";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.HighBP == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；高血压；血脂异常";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.Tangniaobing == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；糖尿病；血脂异常";
            }
            else if (doctorViewModel.HighBP == true && doctorViewModel.Tangniaobing == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "高血压；糖尿病；血脂异常";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.HighBP == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；高血压";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.Tangniaobing == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；糖尿病";
            }
            else if (doctorViewModel.Smoke == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "吸烟；血脂异常";
            }
            else if (doctorViewModel.HighBP == true && doctorViewModel.Tangniaobing == true)
            {
                pulsedata.CardiovascularFactors = "高血压；糖尿病";
            }
            else if (doctorViewModel.HighBP == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "高血压；血脂异常";
            }
            else if (doctorViewModel.Tangniaobing == true && doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "糖尿病；血脂异常";
            }
            else if (doctorViewModel.Smoke == true)
            {
                pulsedata.CardiovascularFactors = "吸烟";
            }
            else if (doctorViewModel.HighBP == true)
            {
                pulsedata.CardiovascularFactors = "高血压";
            }
            else if (doctorViewModel.Tangniaobing == true)
            {
                pulsedata.CardiovascularFactors = "糖尿病";
            }
            else if (doctorViewModel.Xuezhi == true)
            {
                pulsedata.CardiovascularFactors = "血脂异常";
            }
            else
            {
                pulsedata.CardiovascularFactors = "";
            }
        }
        /// <summary>
        /// 患有的心血管疾病
        /// </summary>
        private void Disease()
        {
            if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心力衰竭；心绞痛；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心力衰竭；心绞痛；肾脏病";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心力衰竭；心绞痛；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心力衰竭；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心绞痛；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心力衰竭；心绞痛；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心力衰竭；心绞痛";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心力衰竭；肾脏病";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心绞痛；肾脏病";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心力衰竭；心绞痛；肾脏病";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心力衰竭；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心绞痛；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心力衰竭；心绞痛；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心力衰竭；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心绞痛；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "心力衰竭；心绞痛；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心绞痛；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心力衰竭；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心力衰竭；心绞痛；心肌梗死";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心力衰竭；心绞痛；肾脏病";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心力衰竭";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjiaotong == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心绞痛";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心力衰竭；心绞痛";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；肾脏病";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心力衰竭；肾脏病";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心绞痛；肾脏病";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中；心肌梗死";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心力衰竭；心肌梗死";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；肾脏病；心肌梗死";//
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心绞痛；心肌梗死";
            }
            else if (doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "心绞痛；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Xinlishuaijie == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "心力衰竭；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "心力衰竭；心绞痛；心肌梗死";
            }
            else if (doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "心力衰竭；心绞痛；肾脏病";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心绞痛；心肌梗死";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心绞痛；肾脏病";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心力衰竭；心肌梗死";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心力衰竭；肾脏病";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心力衰竭；心绞痛";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Naozuzhong == true)
            {
                pulsedata.CardiovascularDis = "冠心病；脑卒中";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinlishuaijie == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心力衰竭";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinjiaotong == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心绞痛";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病；肾脏病";
            }
            else if (doctorViewModel.Guanxinbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "冠心病；心肌梗死";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinlishuaijie == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心力衰竭";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjiaotong == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心绞痛";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；肾脏病";
            }
            else if (doctorViewModel.Naozuzhong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "脑卒中；心肌梗死";
            }
            else if (doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjiaotong == true)
            {
                pulsedata.CardiovascularDis = "心力衰竭；心绞痛";
            }
            else if (doctorViewModel.Xinlishuaijie == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "心力衰竭；肾脏病";
            }
            else if (doctorViewModel.Xinlishuaijie == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "心力衰竭；心肌梗死";
            }
            else if (doctorViewModel.Xinjiaotong == true && doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "心绞痛；肾脏病";
            }
            else if (doctorViewModel.Xinjiaotong == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "心绞痛；心肌梗死";
            }
            else if (doctorViewModel.Shenzangbing == true && doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "肾脏病；心肌梗死";
            }
            else if (doctorViewModel.Guanxinbing == true)
            {
                pulsedata.CardiovascularDis = "冠心病";
            }
            else if (doctorViewModel.Naozuzhong == true)
            {
                pulsedata.CardiovascularDis = "脑卒中";
            }
            else if (doctorViewModel.Xinlishuaijie == true)
            {
                pulsedata.CardiovascularDis = "心力衰竭";
            }
            else if (doctorViewModel.Xinjiaotong == true)
            {
                pulsedata.CardiovascularDis = "心绞痛";
            }
            else if (doctorViewModel.Shenzangbing == true)
            {
                pulsedata.CardiovascularDis = "肾脏病";
            }
            else if (doctorViewModel.Xinjigengsi == true)
            {
                pulsedata.CardiovascularDis = "心肌梗死";
            }
            else
            {
                pulsedata.CardiovascularDis = "";
            }
        }
        private void Smoke(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Smoke == false)
            {
                doctorViewModel.Smoke = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgSelect = "pack://application:,,,/Resources/Image/Measure/Check.png";

                }));
                return;
            }
            else
            {
                doctorViewModel.Smoke = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgSelect = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;

            }

        }
        private void HighBp(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.HighBP == false)
            {
                doctorViewModel.HighBP = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgHighBP = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.HighBP = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgHighBP = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }
        private void Tangniaobing_Click(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Tangniaobing == false)
            {
                doctorViewModel.Tangniaobing = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgTang = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.Tangniaobing = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgTang = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }
        private void Xuezhiyichang_Click(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Xuezhi == false)
            {
                doctorViewModel.Xuezhi = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgXuezhi = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.Xuezhi = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgXuezhi = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }

        private void Guanxinbing(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Guanxinbing == false)
            {
                doctorViewModel.Guanxinbing = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgGuan = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.Guanxinbing = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgGuan = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }

        private void Naozuzhong(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Naozuzhong == false)
            {
                doctorViewModel.Naozuzhong = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgNaozu = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.Naozuzhong = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgNaozu = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }

        private void Xinlishuaijie(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Xinlishuaijie == false)
            {
                doctorViewModel.Xinlishuaijie = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgXinli = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.Xinlishuaijie = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgXinli = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }

        private void Xinjiaotong(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Xinjiaotong == false)
            {
                doctorViewModel.Xinjiaotong = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgXinjiaotong = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.Xinjiaotong = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgXinjiaotong = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }

        private void Shenzangbing(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Shenzangbing == false)
            {
                doctorViewModel.Shenzangbing = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgShenzang = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.Shenzangbing = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgShenzang = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }

        private void Xinjigengsi(object sender, RoutedEventArgs e)
        {
            if (doctorViewModel.Xinjigengsi == false)
            {
                doctorViewModel.Xinjigengsi = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgXinji = "pack://application:,,,/Resources/Image/Measure/Check.png";
                }));
                return;
            }
            else
            {
                doctorViewModel.Xinjigengsi = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    doctorViewModel.ImgXinji = "pack://application:,,,/Resources/Image/Measure/Border.png";
                }));
                return;
            }

        }

    }
}
