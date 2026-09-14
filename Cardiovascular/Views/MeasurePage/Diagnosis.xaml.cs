using Cardio.DAL;
using Cardio.Model;
using Cardio.SPCL;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

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
        // 健康问卷来源：登录进入测量页时填写的用户实体（int 字段 1=有，0=无）；
        // 数据管理查看历史报告时没有该实体，传 null
        private readonly UserInfoEntity userInfo;
        public Diagnosis(PulseDataLocalEntity testData, UserInfoEntity userInfo = null)
        {
            InitializeComponent();
            pulsedata = testData;
            this.userInfo = userInfo;
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
                    CbDoctor.Items.Add(doctorInfo.DoctorName);
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
            // 医师诊断文本框初始展示 AI 生成的建议（医师可编辑后保存；已有医师诊断则不覆盖）
            if (string.IsNullOrEmpty(pulsedata.DoctorDiagnosis))
            {
                doctorViewModel.Doctordiagnosis = pulsedata.AIDiagnosisProposal ?? "";
            }
            // 注意：Intshow() 内部的 CardiovascularFactorsShow() 会按旧逻辑重算/清空 pulsedata.CardiovascularFactors，
            // 所以问卷回显统一放在 Intshow() 之后，由实体（int 字段 1=有/0=无）直接带入
            Intshow();
            if (userInfo != null)
            {
                // 测量流程：直接用登录时填写的 UserInfoEntity 带入问卷内容
                PrefillFromUserInfo(userInfo);
            }
            else
            {
                // 数据管理查看历史报告：没有用户实体，退回按记录中保存的问卷串回显
                PrefillQuestionnaire(pulsedata.CardiovascularFactors, pulsedata.CardiovascularDis, pulsedata.CardiovascularDis);
            }
        }
        /// <summary>
        /// 从 UserInfoEntity 的 int 字段（1=有，0=无）带入健康问卷内容
        /// </summary>
        private void PrefillFromUserInfo(UserInfoEntity u)
        {
            if (u.RiskSmoking == 1) { doctorViewModel.Smoke = true; doctorViewModel.ImgSelect = CheckImgPath; }
            if (u.RiskHypertension == 1) { doctorViewModel.HighBP = true; doctorViewModel.ImgHighBP = CheckImgPath; }
            if (u.RiskDiabetes == 1) { doctorViewModel.Tangniaobing = true; doctorViewModel.ImgTang = CheckImgPath; }
            if (u.RiskDyslipidemia == 1) { doctorViewModel.Xuezhi = true; doctorViewModel.ImgXuezhi = CheckImgPath; }

            if (u.CoronaryDiease == 1) { doctorViewModel.Guanxinbing = true; doctorViewModel.ImgGuan = CheckImgPath; }
            if (u.Stroke == 1) { doctorViewModel.Naozuzhong = true; doctorViewModel.ImgNaozu = CheckImgPath; }
            if (u.HeartFailure == 1) { doctorViewModel.Xinlishuaijie = true; doctorViewModel.ImgXinli = CheckImgPath; }
            if (u.Angina == 1) { doctorViewModel.Xinjiaotong = true; doctorViewModel.ImgXinjiaotong = CheckImgPath; }
            if (u.KidneyDiease == 1) { doctorViewModel.Shenzangbing = true; doctorViewModel.ImgShenzang = CheckImgPath; }
            if (u.MvocardialInfarction == 1) { doctorViewModel.Xinjigengsi = true; doctorViewModel.ImgXinji = CheckImgPath; }

            // “疾病说明”文本框显示问卷中填写的内容
            doctorViewModel.CardiovascularDIS = u.OtherDisease ?? "";

            // 恢复被 CardiovascularFactorsShow() 重算掉的记录字段（医师若取消保存，报告中仍保留问卷内容）
            List<string> factors = new List<string>();
            if (u.RiskSmoking == 1) factors.Add("吸烟");
            if (u.RiskHypertension == 1) factors.Add("高血压");
            if (u.RiskDiabetes == 1) factors.Add("糖尿病");
            if (u.RiskDyslipidemia == 1) factors.Add("血脂异常");
            pulsedata.CardiovascularFactors = string.Join("；", factors);
            pulsedata.CardiovascularDis = u.OtherDisease ?? "";
        }
        /// <summary>
        /// 回显问卷内容：factors=危险因素；diseaseChecked=既往疾病勾选项（只用于勾选）；
        /// diseaseText=“疾病说明”文本框内容（只显示手写说明，不含勾选项）
        /// </summary>
        private const string CheckImgPath = "pack://application:,,,/Resources/Image/Measure/Check.png";
        private void PrefillQuestionnaire(string factors, string diseaseChecked, string diseaseText)
        {
            factors ??= "";
            diseaseChecked ??= "";
            diseaseText ??= "";
            if (factors.Contains("吸烟")) { doctorViewModel.Smoke = true; doctorViewModel.ImgSelect = CheckImgPath; }
            if (factors.Contains("高血压")) { doctorViewModel.HighBP = true; doctorViewModel.ImgHighBP = CheckImgPath; }
            if (factors.Contains("糖尿病")) { doctorViewModel.Tangniaobing = true; doctorViewModel.ImgTang = CheckImgPath; }
            if (factors.Contains("血脂异常")) { doctorViewModel.Xuezhi = true; doctorViewModel.ImgXuezhi = CheckImgPath; }

            if (diseaseChecked.Contains("冠心病")) { doctorViewModel.Guanxinbing = true; doctorViewModel.ImgGuan = CheckImgPath; }
            if (diseaseChecked.Contains("脑卒中")) { doctorViewModel.Naozuzhong = true; doctorViewModel.ImgNaozu = CheckImgPath; }
            if (diseaseChecked.Contains("心力衰竭")) { doctorViewModel.Xinlishuaijie = true; doctorViewModel.ImgXinli = CheckImgPath; }
            if (diseaseChecked.Contains("心绞痛")) { doctorViewModel.Xinjiaotong = true; doctorViewModel.ImgXinjiaotong = CheckImgPath; }
            if (diseaseChecked.Contains("肾脏病")) { doctorViewModel.Shenzangbing = true; doctorViewModel.ImgShenzang = CheckImgPath; }
            if (diseaseChecked.Contains("心肌梗死")) { doctorViewModel.Xinjigengsi = true; doctorViewModel.ImgXinji = CheckImgPath; }

            // 文本框只显示“疾病说明”的手写内容
            doctorViewModel.CardiovascularDIS = diseaseText;
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
            bool isSucess;
            if (pulsedata.Id > 0)
            {
                isSucess = dataDAL.Update(pulsedata);   //已有记录：更新
            }
            else
            {
                isSucess = dataDAL.Insert(pulsedata) > 0;   //首次保存：插入
            }
            if (isSucess)
            {
                HandyControl.Controls.Growl.Success("诊断信息保存成功！", "SuccessMsg");
                doctorViewModel.CloseAction?.Invoke();   // 保存成功自动关闭
            }
            else
            {
                HandyControl.Controls.Growl.Warning("诊断信息保存失败！", "SuccessMsg");
            }
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
