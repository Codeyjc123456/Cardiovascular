using Cardio.Algorithm;
using Cardio.DAL;
using Cardio.Model;
using Cardio.SPCL;
using Cardio.Util;
using Cardio.Views.DoctorPage;
using FastReport;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Cardio.Views
{
    /// <summary>
    /// AdminLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class AdminLoginPage
    {  
        DoctorInfoDAL doctorDAL = null;
        AdminUserDAL adminUserDAL = null;

        LoginViewModel loginView = new LoginViewModel();

        private readonly APPSettingsViewModel viewModel = APPSettingsViewModel.getInstance();
        public AdminLoginPage()
        {
            InitializeComponent();
            DataContext = loginView;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            doctorDAL = DoctorInfoDAL.getInstance();
            adminUserDAL = AdminUserDAL.getInstance();
            LoadDefaultDoctor();
        }

        private void LoadDefaultDoctor()
        {
            // 启动时直接进入登录页，不能依赖 StartPage 预先读取配置。
            bool loaded = viewModel.LoadData();
            string doctorName = loaded ? viewModel.APP_DoctorName ?? "" : "";
            string doctorPassword = loaded ? viewModel.APP_DoctorPWD ?? "" : "";

            GlobalVariable.DoctorName = doctorName;
            GlobalVariable.DoctorPwd = doctorPassword;
            loginView.UserID = doctorName;
            UserPWD.Password = doctorPassword;

            if (!loaded)
                Growl.Warning("系统配置加载失败，未能读取默认医师信息，请手动输入账号和密码。");
        }
        private void LoginBtn(object sender, RoutedEventArgs e)
        {
            if (loginView.RemoveAndAdd > 0)
            {
                Growl.Warning("信息校验失败，请修改");
                return;
            }
            if (loginView.UserID == "")
            {
                Growl.Info("请输入用户名！");
                return;
            }
            if (UserPWD.Password == "")
            {
                Growl.Info("请输入密码");
                return;
            }
            string conditionStr = " AdminName ='" + loginView.UserID.Trim() + "'";
            List<AdminUserEntity> listadmin = adminUserDAL.Finds(conditionStr);
            if (listadmin != null && listadmin.Count != 0)
            {
                foreach (AdminUserEntity adminUser in listadmin)
                {
                    if (adminUser.AdminPassword == UserPWD.Password)
                    {
                        AdminManager();
                    }
                    else
                    {
                        Growl.Info("管理员密码输入不正确！");
                    }  
                }
            }
            else//如果在管理员表查不到，再次查询医师表
            {
                string conditionStrDoctor = "DoctorName ='" + loginView.UserID.Trim() + "'";
                List<DoctorInfoEntity> doctorlist = doctorDAL.Finds(conditionStrDoctor);
                if (doctorlist != null && doctorlist.Count != 0)
                {
                    foreach (DoctorInfoEntity doctor in doctorlist)
                    {
                      
                        if (doctor.DoctorPwd == UserPWD.Password)
                        {
                            GlobalVariable.DoctorName = doctor.DoctorName;
                            this.NavigationService.Navigate(new MainPage());
                            return;
                        }
                    }
                    Growl.Info("医师密码输入不正确！");
                }
                else
                {
                    Growl.Info("当前医师不存在！");
                }
            }
        }
        private async void AdminManager()
        {
            await Dialog.Show(new DoctorListDialog()).GetResultAsync<string>();
            LoadDefaultDoctor();
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            ScreenTaskBar.Show();
            var myWindow = HandyControl.Controls.Window.GetWindow(this);
            myWindow.Close();
            Application.Current.Shutdown();
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UserPWD.Password.Length > 18)
            {
                PWDInfo.Text = "密码超过最大长度限制!";
                return;
            }
            else
            {
                PWDInfo.Text = "";
                return;
            }
        }
    }
}
