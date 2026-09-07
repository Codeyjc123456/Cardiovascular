using Cadio.CustomRule;
using System.Globalization;
using Cardio.CustomRule;
using Cardio.DAL;
using Cardio.Model;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Windows;

namespace Cardio.Views.MeasurePage
{
    /// <summary>
    /// RegisterDialog.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateDialog
    {
        private readonly UserInfoDAL userInfoDAL = UserInfoDAL.getInstance();
        private readonly UserInfoEntity userinfo;
        private readonly UserViewModel userViewModel = new UserViewModel();
        private readonly Action tabAction;

        public UpdateDialog(UserInfoEntity u, Action t)
        {
            InitializeComponent();
            userinfo = u;
            tabAction = t;
            DataContext = userViewModel;
           
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            userinfo.UserId = "";
            userViewModel.CloseAction?.Invoke();
            tabAction.Invoke();
        }
        
        private bool userLoginNameUsed(string userLoginName)
        {

            string conditionStr = " userID ='" + userLoginName + "'";
            if (userInfoDAL.FindCount(conditionStr) > 0) return true;
            return false;
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            if (userViewModel.RemoveAndAdd != 0)
            {
                HandyControl.Controls.Growl.Info("信息校验失败，请修改！", "Update");
                return;
            }

            if (userViewModel.UserName == "" || userViewModel.UserID == "" || userViewModel.BirthDay == "")
            {
                HandyControl.Controls.Growl.Info("请将信息填写完整！", "Update");
                return;
            }
           
           
            DateTime today = DateTime.Today;
            var birthDateValidation = BirthDateRule.ValidateBirthDate(
                userViewModel.BirthDay, CultureInfo.CurrentCulture, today, out DateTime birthDate);
            if (!birthDateValidation.IsValid)
            {
                HandyControl.Controls.Growl.Warning(birthDateValidation.ErrorContent.ToString(), "Update");
                return;
            }

            userinfo.UserId = userViewModel.UserID;
            userinfo.UserName = userViewModel.UserName;
            userinfo.UserSex = userViewModel.UserSex;

            userinfo.UserBirthday = birthDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            userinfo.UserHeight = userViewModel.UserHeight;
            userinfo.UserWeight = userViewModel.UserWeight;
            double[] Temp_Distance = new double[3];
            Temp_Distance[0] = (double)(0.8129 * userinfo.UserHeight + 12.328);
            Temp_Distance[1] = (double)(0.2195 * userinfo.UserHeight - 2.0734);
            Temp_Distance[2] = Temp_Distance[0] - Temp_Distance[1];
            userinfo.UserDistance = (float)Math.Round((decimal)Temp_Distance[2], 1, MidpointRounding.AwayFromZero);
            userinfo.UserAge = BirthDateRule.CalculateAge(birthDate, today);

            // 不写本地数据库：信息已回填到 userinfo 实体（与调用方是同一引用），
            // 关闭对话框跳回 LoginPage，由 LoginPage 使用补全后的 userinfo
            userViewModel.CloseAction?.Invoke();
            tabAction.Invoke();
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            userViewModel.UserID = userinfo.UserId ?? "";
            // 已有值就预填，不依赖 Id（API 临时用户 Id 为 0 也能回显）
            if (!string.IsNullOrEmpty(userinfo.UserName))
                userViewModel.UserName = userinfo.UserName;
            if (!string.IsNullOrEmpty(userinfo.UserBirthday))
                userViewModel.BirthDay = userinfo.UserBirthday;
            if (!string.IsNullOrEmpty(userinfo.UserSex))
                userViewModel.UserSex = userinfo.UserSex;
            if (userinfo.UserHeight > 0)
                userViewModel.UserHeight = (int)userinfo.UserHeight;
            if (userinfo.UserWeight > 0)
                userViewModel.UserWeight = userinfo.UserWeight;

            if (userinfo.Id > 0)
            {
                Dispatcher.BeginInvoke(() => {
                    UserIDInfo.IsReadOnly = true;
                });
            }
            UserNameBtn.Focus();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible) GC.Collect();
        }
    }
}
