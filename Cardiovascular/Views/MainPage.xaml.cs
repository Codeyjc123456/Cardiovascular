using Cardio.Algorithm;
using Cardio.Model;
using Cardio.SPCL;
using Cardio.Views.MeasurePage;
using Cardio.Views.UserManagePage;
using System.Windows;
using System.Windows.Controls;

namespace Cardio.Views
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();
        MeasureViewModel measureViewModel = null;
       
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)
            {
                GC.Collect();
            }
        }

        private void OpenSystemBtn(object sender, RoutedEventArgs e)
        { 
            this.NavigationService.Navigate(new SystemWinPage());
        }

        private void OpenUserManageBtn(object sender, RoutedEventArgs e)
        {
            
            this.NavigationService.Navigate(new UserListPage());
        }

        private void OpenDataManageBtn(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new DataManageWinPage());
        }

        private void OpenMeasureBtn(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new LoginPage());
        }

        private void HelpBtn(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var ImgPath = "pack://application:,,,/Resources/Image/Help/m_helpAccount.png";
                new HandyControl.Controls.ImageBrowser(new Uri(ImgPath)).Show();
            }));
        }

        private void QuitSoft(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show("是否确定继续返回？", "操作提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.NavigationService.GoBack();
            }
            else
            {

            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            measureViewModel = new MeasureViewModel();
            DataContext = measureViewModel;
            this.IsVisibleChanged += MainPage_IsVisibleChanged;
            APPSettingUtil.LoadData();
            measureViewModel.Network = APPSettingUtil.APP_Network;
            measureViewModel.Version = APPSettingUtil.APP_Version;
            GlobalVariable.Sample_Rate = Convert.ToInt32(APPSettingUtil.APP_COMRate);
            //GlobalVariable.Sample_Rate = 1000;
        }
    }
}
