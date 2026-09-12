using Cardio.Model;
using Cardio.SPCL;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Cardio.Views.MeasurePage
{
    /// <summary>
    /// 健康问卷（登录完成后优先填写）：只采集心血管危险因素与既往心血管疾病，
    /// 不含医师判断/操作医师/医师建议（这些内容在 Diagnosis 页面填写）。
    /// 保存后写入 Variable.CardiovascularFactors / Variable.CardiovascularDis，
    /// 测量开始时带入 pulsedata，Diagnosis 打开即可自动回显。
    /// </summary>
    public partial class Questionnaire : Border
    {
        private readonly MeasureViewModel questionnaireViewModel = new ();

        public Questionnaire()
        {
            InitializeComponent();
            DataContext = questionnaireViewModel;
        }

        private void Border_Load(object sender, RoutedEventArgs e)
        {
            PrefillFromSaved();
        }

        /// <summary>
        /// 若已有保存过的问卷内容则回显（再次进入时保持上次选择）
        /// </summary>
        private void PrefillFromSaved()
        {
            string factors = Variable.CardiovascularFactors ?? "";
            questionnaireViewModel.Smoke = factors.Contains("吸烟");
            questionnaireViewModel.HighBP = factors.Contains("高血压");
            questionnaireViewModel.Tangniaobing = factors.Contains("糖尿病");
            questionnaireViewModel.Xuezhi = factors.Contains("血脂异常");

            // 勾选项只用于回显勾选，不写进“疾病说明”
            string checkedDis = Variable.CardiovascularDiseaseChecked ?? "";
            questionnaireViewModel.Guanxinbing = checkedDis.Contains("冠心病");
            questionnaireViewModel.Naozuzhong = checkedDis.Contains("脑卒中");
            questionnaireViewModel.Xinlishuaijie = checkedDis.Contains("心力衰竭");
            questionnaireViewModel.Xinjiaotong = checkedDis.Contains("心绞痛");
            questionnaireViewModel.Shenzangbing = checkedDis.Contains("肾脏病");
            questionnaireViewModel.Xinjigengsi = checkedDis.Contains("心肌梗死");

            // “疾病说明”只显示手写内容
            questionnaireViewModel.CardiovascularDIS = Variable.CardiovascularDis ?? "";
        }

        /// <summary>
        /// 保存：勾选项与“疾病说明”分别保存，互不混入
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Variable.CardiovascularFactors = BuildFactors();
            Variable.CardiovascularDiseaseChecked = BuildCheckedDiseases();
            Variable.CardiovascularDis = questionnaireViewModel.CardiovascularDIS ?? "";
            questionnaireViewModel.CloseAction?.Invoke();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // 取消：不保存，保留上次内容
            questionnaireViewModel.CloseAction?.Invoke();
        }

        private string BuildFactors()
        {
            var list = new List<string>();
            if (questionnaireViewModel.Smoke) list.Add("吸烟");
            if (questionnaireViewModel.HighBP) list.Add("高血压");
            if (questionnaireViewModel.Tangniaobing) list.Add("糖尿病");
            if (questionnaireViewModel.Xuezhi) list.Add("血脂异常");
            return string.Join("；", list);
        }

        private string BuildCheckedDiseases()
        {
            var list = new List<string>();
            if (questionnaireViewModel.Guanxinbing) list.Add("冠心病");
            if (questionnaireViewModel.Naozuzhong) list.Add("脑卒中");
            if (questionnaireViewModel.Xinlishuaijie) list.Add("心力衰竭");
            if (questionnaireViewModel.Xinjiaotong) list.Add("心绞痛");
            if (questionnaireViewModel.Shenzangbing) list.Add("肾脏病");
            if (questionnaireViewModel.Xinjigengsi) list.Add("心肌梗死");
            return string.Join("；", list);
        }
    }
}
