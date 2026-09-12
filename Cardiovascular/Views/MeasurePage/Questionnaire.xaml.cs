using Cardio.DAL;
using Cardio.Model;
using System.Windows;
using System.Windows.Controls;

namespace Cardio.Views.MeasurePage
{
    /// <summary>
    /// 健康问卷（登录完成后优先填写）：只采集心血管危险因素与既往心血管疾病，
    /// 不含医师判断/操作医师/医师建议（这些内容在 Diagnosis 页面填写）。
    /// 勾选项直接存进 UserInfoEntity 的 int 字段（1=有，0=无），
    /// “疾病说明”存 OtherDisease，便于直接从数据库/实体带入。
    /// </summary>
    public partial class Questionnaire : Border
    {
        private readonly MeasureViewModel questionnaireViewModel = new ();
        private readonly UserInfoEntity userInfo;

        public Questionnaire(UserInfoEntity userInfo)
        {
            InitializeComponent();
            this.userInfo = userInfo;
            DataContext = questionnaireViewModel;
        }

        private void Border_Load(object sender, RoutedEventArgs e)
        {
            PrefillFromEntity();
        }

        /// <summary>
        /// 从 UserInfoEntity 回显（1=有，0=无）
        /// </summary>
        private void PrefillFromEntity()
        {
            questionnaireViewModel.Smoke = userInfo.RiskSmoking == 1;
            questionnaireViewModel.HighBP = userInfo.RiskHypertension == 1;
            questionnaireViewModel.Tangniaobing = userInfo.RiskDiabetes == 1;
            questionnaireViewModel.Xuezhi = userInfo.RiskDyslipidemia == 1;

            questionnaireViewModel.Guanxinbing = userInfo.CoronaryDiease == 1;
            questionnaireViewModel.Naozuzhong = userInfo.Stroke == 1;
            questionnaireViewModel.Xinlishuaijie = userInfo.HeartFailure == 1;
            questionnaireViewModel.Xinjiaotong = userInfo.Angina == 1;
            questionnaireViewModel.Shenzangbing = userInfo.KidneyDiease == 1;
            questionnaireViewModel.Xinjigengsi = userInfo.MvocardialInfarction == 1;

            questionnaireViewModel.CardiovascularDIS = userInfo.OtherDisease ?? "";
        }

        /// <summary>
        /// 保存：把勾选写回 UserInfoEntity（1/0），并标记问卷已填写
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            userInfo.RiskSmoking = questionnaireViewModel.Smoke ? 1 : 0;
            userInfo.RiskHypertension = questionnaireViewModel.HighBP ? 1 : 0;
            userInfo.RiskDiabetes = questionnaireViewModel.Tangniaobing ? 1 : 0;
            userInfo.RiskDyslipidemia = questionnaireViewModel.Xuezhi ? 1 : 0;

            userInfo.CoronaryDiease = questionnaireViewModel.Guanxinbing ? 1 : 0;
            userInfo.Stroke = questionnaireViewModel.Naozuzhong ? 1 : 0;
            userInfo.HeartFailure = questionnaireViewModel.Xinlishuaijie ? 1 : 0;
            userInfo.Angina = questionnaireViewModel.Xinjiaotong ? 1 : 0;
            userInfo.KidneyDiease = questionnaireViewModel.Shenzangbing ? 1 : 0;
            userInfo.MvocardialInfarction = questionnaireViewModel.Xinjigengsi ? 1 : 0;

            userInfo.OtherDisease = questionnaireViewModel.CardiovascularDIS ?? "";
            userInfo.IsSurveryed = 1;

            questionnaireViewModel.CloseAction?.Invoke();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // 取消：不写入实体
            questionnaireViewModel.CloseAction?.Invoke();
        }
    }
}
