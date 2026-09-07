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
        private bool loading;
        public StartPage()
        {
            InitializeComponent();
            DataContext = userViewModel;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (loading) return;
            loading = true;
            try
            {
                var task1 = Task.Run(() => {
                    APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();
                    bool loaded = APPSettingUtil.LoadData();
                    AdminUserDAL.getInstance();
                    UserInfoDAL.getInstance();
                    DoctorInfoDAL.getInstance();
                    SystemconfigDAL.GetInstance();
                    return loaded;
                });
                var task2 = Task.Run(() => {

                    FastReport.Utils.Res.LoadLocale("./Resources/Template/Chinese.frl");
                    using var report = new Report();
                    report.Load("./Resources/Template/WPFABIAI202.frx");  //载入报表文件
                    report?.Prepare();
                });
                var task3 = ShowProgressAsync();
                await Task.WhenAll(task1, task2, task3);
                if (!await task1) HandyControl.Controls.Growl.Warning("系统配置加载失败！");
                userViewModel.CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                Cardio.Util.LogUtil.Error("启动初始化", ex.ToString());
                HandyControl.Controls.Growl.Error("初始化失败：" + ex.Message);
                loading = false;
            }
        }

        private async Task ShowProgressAsync()
        {
            for (int i = 1; i <= 100 && IsLoaded; i++)
            {
                CustomProgressBar.Value = i;
                await Task.Delay(20);
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)
            {
                // 页面隐藏不强制触发全量垃圾回收。
            }
        }
    }
}
