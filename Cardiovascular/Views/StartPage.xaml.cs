using Cardio.DAL;
using Cardio.Model;
using FastReport;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Cardio.Views
{
    /// <summary>
    /// StartPage.xaml 的交互逻辑
    /// </summary>
    public partial class StartPage
    {
        private readonly UserViewModel userViewModel = new UserViewModel();
        public StartPage()
        {
            InitializeComponent();
            DataContext = userViewModel;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var task1 = Task.Run(() => {
                APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();
                if (!APPSettingUtil.LoadData()) HandyControl.Controls.Growl.Warning("系统配置加载失败！");
                AdminUserDAL.getInstance();
                UserInfoDAL.getInstance();
                DoctorInfoDAL.getInstance();
                SystemconfigDAL.GetInstance();
            });
            var task2 = Task.Run(() => {

                FastReport.Utils.Res.LoadLocale("./Resources/Template/Chinese.frl");
                var report = new Report();
                report.Load("./Resources/Template/WPFABIAI202.frx");  //载入报表文件
                report?.Prepare();
            });
            var task3 = Task.Run(() => {
                for (int i = 1; i <= 100; i += 1)
                {
                    Dispatcher.BeginInvoke(() => { CustomProgressBar.Value = i; });
                    Task.Delay(20);
                }
            });
            await Task.WhenAll(task1, task2, task3);
            userViewModel.CloseAction?.Invoke();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)
            {
                GC.Collect();
            }
        }
    }
}
