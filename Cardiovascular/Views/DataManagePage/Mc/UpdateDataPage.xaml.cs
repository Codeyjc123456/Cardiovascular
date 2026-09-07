
using Cardio.DAL;
using Cardio.Model;
using System.Windows;
using System.Windows.Controls;

namespace Cadio.Views.DataManagePage.Mc
{
    public partial class UpdateDataPage : Page
    {
        MeasureViewModel model = new MeasureViewModel();
        private readonly PulseDataLocalDAL pulseDataDAL = PulseDataLocalDAL.getInstance();
        public UpdateDataPage(PulseDataLocalEntity entity)
        {
            InitializeComponent();
            DataContext = model;
            model.testdata = entity;
        }
        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            model.InitDisplay();
        }
        private void BackBtn(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            if (model.RemoveAndAdd != 0)
            {
                HandyControl.Controls.Growl.Warning("信息校验失败，请检查后修改!");
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                model.testdata.userName = model.UserName;
                model.testdata.userHeight = Convert.ToInt32(model.UserHeight);
                model.testdata.userWeight = Convert.ToInt32(model.UserWeight);
                model.testdata.userAge = Convert.ToInt32(model.UserAge);
                model.testdata.userSex = model.UserSex;
                model.testdata.Hr = Convert.ToInt32(model.Hr);
                model.testdata.Ed = Convert.ToDouble(model.EdPct);
                model.testdata.Spti = Convert.ToInt32(model.Spti);
                model.testdata.Dpti = Convert.ToInt32(model.Dpti);
                model.testdata.Sevr = Convert.ToDouble(model.Sevr);
                model.testdata.Sbp = Convert.ToInt32(model.Sbp);
                model.testdata.Dbp = Convert.ToInt32(model.Dbp);
                model.testdata.Pp = Convert.ToInt32(model.Pp);
                model.testdata.Sbp2 = Convert.ToInt32(model.Cap);
                model.testdata.AIx = Convert.ToDouble(model.AIx);
               
                model.testdata.AIDiagnosisResult = model.AIProposalDiag;//心血管诊断结果
                model.testdata.AIDiagnosisProposal = model.AIDiagnosisProposal;//心血管诊断建议
            }));
            bool isSucess = pulseDataDAL.Update(model.testdata, "TestDate='" + model.testdata.TestDateTime + "'");  // 插入新用户到数据库中，返回是否成功
            if (isSucess)
                HandyControl.Controls.Growl.Success("数据更新成功！");
            else
                HandyControl.Controls.Growl.Warning("数据更新失败！");
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)
            {
                DataContext = null;
                GC.Collect();
            }
        }
    }
}
