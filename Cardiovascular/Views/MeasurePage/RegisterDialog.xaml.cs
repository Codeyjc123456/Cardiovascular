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
    public partial class RegisterDialog
    {
        private readonly UserInfoDAL userInfoDAL = UserInfoDAL.getInstance();
        private readonly UserInfoEntity userinfo;
        private readonly UserViewModel userViewModel = new UserViewModel();
        private readonly Action tabAction;

        public RegisterDialog(UserInfoEntity u, Action t)
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
                HandyControl.Controls.Growl.Info("信息校验失败，请修改！", "Rigister");
                return;
            }

            if (userViewModel.UserName == "" || userViewModel.UserID == "" || userViewModel.BirthDay == "")
            {
                HandyControl.Controls.Growl.Info("请将信息填写完整！", "Rigister");
                return;
            }
           
           
            userinfo.UserId = userViewModel.UserID;
            userinfo.UserName = userViewModel.UserName;
            userinfo.UserSex = userViewModel.UserSex;
            DateTime t = DateTime.Parse(userViewModel.BirthDay);
            userinfo.UserBirthday = t.ToString("yyyy-MM-dd");
            userinfo.UserHeight = userViewModel.UserHeight;
            userinfo.UserWeight = userViewModel.UserWeight;
            userinfo.CreateTime = DateTime.Now.ToString();
            double[] Temp_Distance = new double[3];
            Temp_Distance[0] = (double)(0.8129 * userinfo.UserHeight + 12.328);
            Temp_Distance[1] = (double)(0.2195 * userinfo.UserHeight - 2.0734);
            Temp_Distance[2] = Temp_Distance[0] - Temp_Distance[1];
            userinfo.UserDistance = (float)Math.Round((decimal)Temp_Distance[2], 1, MidpointRounding.AwayFromZero);
            DateTime bir;
            if (DateTime.TryParse(userinfo.UserBirthday, out bir))
            {
                userinfo.UserAge = DateTime.Now.Year - bir.Year;
                if (DateTime.Now.Month < bir.Month)
                {
                    userinfo.UserAge -= 1;
                }
            }
            if (userinfo.UserAge < 7 || userinfo.UserAge > 100)
            {
                HandyControl.Controls.Growl.Warning("出生日期填写有误，请重新填写！", "Rigister");
                return;
            }

            if (userinfo.Id > 0)
            {
                bool isSucess = userInfoDAL.Update(userinfo);  // 插入新用户到数据库中，返回是否成功
                if (isSucess)
                {
                    HandyControl.Controls.Growl.Success("用户更新成功！", "Rigister");
                    userViewModel.CloseAction?.Invoke();
                    tabAction.Invoke();
                }
                else
                {
                    HandyControl.Controls.Growl.Warning("用户更新失败！", "Rigister");
                }
            }
            else
            {
                if (userLoginNameUsed(userViewModel.UserID))
                {
                    HandyControl.Controls.Growl.Warning("用户名已被使用，请重新填写！", "Rigister");
                    return;
                }

                if (userInfoDAL.Insert(userinfo) > 0)
                {
                    HandyControl.Controls.Growl.Success("用户注册成功！", "Rigister");
                    userViewModel.CloseAction?.Invoke();
                    tabAction.Invoke();
                }
                else
                {
                    HandyControl.Controls.Growl.Warning("用户注册失败！", "Rigister");
                }
            } 
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            userViewModel.UserID = userinfo.UserId ?? "";
            if (userinfo.Id > 0)
            {
                userViewModel.UserID = userinfo.UserId ?? "";
                userViewModel.UserName = userinfo.UserName ?? "";
                userViewModel.BirthDay = userinfo.UserBirthday ?? "";
                userViewModel.UserSex = userinfo.UserSex ?? "";
                userViewModel.UserHeight = Convert.ToInt32(userinfo.UserHeight);
                userViewModel.UserWeight = Convert.ToDouble(userinfo.UserWeight);
                Dispatcher.Invoke(() => {
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
