using Cardio.BLL;
using Cardio.DAL;
using Cardio.Model;
using Cardio.SPCL;
using Cardio.Util;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cardio.Views.MeasurePage
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Page
    {
        Dialog d = null;
        private CancellationTokenSource? loginCancellation;
        UserInfoDAL userInfoDAL = null;
        LoginViewModel loginViewModel = new LoginViewModel();
        public static string userID = "";
        private Action tabAction;
        private readonly APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();

        public LoginPage()
        {
            InitializeComponent();
            Unloaded += (_, _) => loginCancellation?.Cancel();
            DataContext = loginViewModel;
            tabAction += TabCallBackExcute;
        }

        private void TabCallBackExcute()
        {
            
        }

        private async Task LoginAPI(CancellationToken token)
        {
            string url = APPSettingUtil.APP_ApiUrlLogin;
            var data = new Dictionary<string, object>
            {
                { "mbrKey", loginViewModel.UserID }
            };
            //url = "http://127.0.0.1:4523/m2/7869154-7618859-default/510152173";
            //url = "http://39.105.221.110:10013/health/rest/cardiovascularservice/getmemberbykey";
            var response = await ApiBLL.DoGetUserAsync(url, data, token);
            token.ThrowIfCancellationRequested();
            var response_json = (JObject)JsonConvert.DeserializeObject(response);
            UserInfoEntity userInfo = new UserInfoEntity();
            if (response_json != null)
            {
                JObject obj = JObject.Parse(response);

                userInfo.UserHeight = (double)obj["memberEntity"]["height"];
                userInfo.UserWeight = (double)obj["memberEntity"]["weight"];
                userInfo.UserName = obj["memberEntity"]["name"].ToString();
                userInfo.UserId = obj["memberEntity"]["id"].ToString();
                userInfo.UserCode = obj["memberEntity"]["code"].ToString();
                userInfo.UserSex = obj["memberEntity"]["sex"].ToString() == "01" ? "男" : "女";
                userInfo.UserBirthday = obj["memberEntity"]["birth"].ToString();
                if (userInfo.UserName.IsNullOrEmpty() || userInfo.UserHeight.ToString().IsNullOrEmpty() || userInfo.UserWeight.ToString().IsNullOrEmpty()
                    || userInfo.UserSex.IsNullOrEmpty() || userInfo.UserBirthday.IsNullOrEmpty())
                {
                    await Dialog.Show(new UpdateDialog(userInfo, tabAction)).GetResultAsync<string>();
                    token.ThrowIfCancellationRequested();
                    
                }
                userInfo.CreateTime = obj["memberEntity"]["createdTime"]?.ToString();
                userInfo.OperatingDoctor = obj["memberEntity"]["drId"].ToString();
                userInfo.OrgId = obj["memberEntity"]["orgId"].ToString();

                userInfo.UserAge = CalcuAge(userInfo.UserBirthday);
                TestMode.AI_Select = true;
                TestMode.LBBP_Select = true;
                TestMode.LABP_Select = true;
                TestMode.RBBP_Select = true;
                TestMode.RABP_Select = true;
                double[] Temp_Distance = new double[3];
                Temp_Distance[0] = 0.8129 * userInfo.UserHeight + 12.328;
                Temp_Distance[1] = 0.2195 * userInfo.UserHeight - 2.0734;
                Temp_Distance[2] = Temp_Distance[0] - Temp_Distance[1];
                userInfo.UserDistance = Math.Round(Temp_Distance[2], 1, MidpointRounding.AwayFromZero);
                this.NavigationService.Navigate(new MeasureReePage(userInfo));
            }
            else
            {
                HandyControl.Controls.Growl.Warning("查无此用户信息！");
            }
        }

        private async void LoginBtn(object sender, RoutedEventArgs e)
        {
            if (loginViewModel.UserID=="")
            {
                Growl.Warning("请填写账号！");
                return;
            }

            if (loginCancellation != null) return;
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            loginCancellation = cancellation;
            try
            {
                if (APPSettingUtil.APP_Network == "单机版")
                    await LoginLocal(cancellation.Token);
                else
                    await LoginAPI(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                if (IsLoaded) Growl.Warning("登录请求已取消或超时，请重试。");
            }
            catch (Exception ex)
            {
                LogUtil.Error("登录", ex.ToString());
                if (IsLoaded) Growl.Error("登录失败：" + ex.Message);
            }
            finally { loginCancellation = null; }
        }

        private async Task LoginLocal(CancellationToken token)
        {
            UserInfoEntity userInfo = new UserInfoEntity();
            string conditionStr = " UserId ='" + loginViewModel.UserID + "'";
            userInfo = await Task.Run(() => userInfoDAL.Find(conditionStr), token);
            token.ThrowIfCancellationRequested();
            if (userInfo != null)
            {
                this.NavigationService.Navigate(new MeasureReePage(userInfo));
            }
            else
            {
                userID = loginViewModel.UserID;
                UserInfoEntity u = new UserInfoEntity();
                u.UserId = loginViewModel.UserID;
                await Dialog.Show(new RegisterDialog(u, tabAction)).GetResultAsync<string>();
            }
        }
        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见 
            if (!isVisible)
            {
                loginCancellation?.Cancel();
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            userInfoDAL = UserInfoDAL.getInstance();
        }

        private void GetID(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;
            DateTime todayStart = today.Date;
            int Temp_Num = 0;
            string Temp_Str = DateTime.Now.Date.ToString("yyyyMMdd");//查询今天0点到现在时间的注册个数，定位Temp_Num; 
            string conditionStr = " ( createTime > '" + todayStart + "' and createTime < '" + DateTime.Now.ToString() + " '" + ')';
            List<UserInfoEntity> user = new List<UserInfoEntity>();
            user = userInfoDAL.Finds(conditionStr); 
            if (user != null)
            {
                Temp_Num = user.Count + 1;
            }
            loginViewModel.UserID = Temp_Str + Temp_Num.ToString("000");
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void LoginAPPID()
        {
            
            var (userInfo, message) = ApiBLL.GetUser(APPSettingUtil.APP_ApiUrlLogin, loginViewModel.UserID);
            LogUtil.Info(message);
            if (userInfo.UserId != "" && userInfo.UserId != null)
            {
                double[] Temp_Distance = new double[3];
                Temp_Distance[0] = 0.8129 * userInfo.UserWeight + 12.328;
                Temp_Distance[1] = 0.2195 * userInfo.UserHeight - 2.0734;
                Temp_Distance[2] = Temp_Distance[0] - Temp_Distance[1];
                userInfo.UserDistance = Math.Round(Temp_Distance[2], 1, MidpointRounding.AwayFromZero);
                this.NavigationService.Navigate(new MeasureReePage(userInfo));
            }
        }
        private async Task LoginAPIBX(CancellationToken token)
        {
            string url = APPSettingUtil.APP_ApiUrlLogin;
            var data = new Dictionary<string, object>
                {
                    { "userIdcard", loginViewModel.UserID }
                };
            url = "http://192.168.8.182:8800/api/v1.0/baseInfo/user/getUserByIdcard";
            var response = await ApiBLL.DoGetUserAsync(url, data, token);
            token.ThrowIfCancellationRequested();
            var response_json = (JObject)JsonConvert.DeserializeObject(response);
            UserInfoEntity userInfo = new UserInfoEntity();
            if ((string)response_json["code"] == "200")
            {
                // 方式1：使用 JObject 动态解析
                JObject obj = JObject.Parse(response);
                string userName = obj["data"]["userName"]?.ToString();
                userInfo?.UserHeight = (double)obj["data"]["userHeight"];
                userInfo?.UserWeight = (double)obj["data"]["userWeight"];
                userInfo.UserName = obj["data"]["userName"].ToString();
                userInfo.UserId = obj["data"]["userId"].ToString();
                userInfo.UserSex = obj["data"]["userSex"].ToString();
                userInfo.UserBirthday = obj["data"]["userBirthday"].ToString();
                userInfo.UserAge = CalcuAge(userInfo.UserBirthday);
                TestMode.AI_Select = true;
                TestMode.LBBP_Select = true;
                TestMode.LABP_Select = true;
                TestMode.RBBP_Select = true;
                TestMode.RABP_Select = true;
                double[] Temp_Distance = new double[3];
                Temp_Distance[0] = 0.8129 * userInfo.UserHeight + 12.328;
                Temp_Distance[1] = 0.2195 * userInfo.UserHeight - 2.0734;
                Temp_Distance[2] = Temp_Distance[0] - Temp_Distance[1];
                userInfo.UserDistance = Math.Round(Temp_Distance[2], 1, MidpointRounding.AwayFromZero);
                this.NavigationService.Navigate(new MeasureReePage(userInfo));
            }
            else
            {
                HandyControl.Controls.Growl.Warning("查无此用户信息！");
            }
        }
        public static int CalcuAge(string UserBirthday)
        {
            int age = 0;
            DateTime bir;
            if (DateTime.TryParse(UserBirthday, out bir))
            {
                age = DateTime.Now.Year - bir.Year;
                // 如果还没过今年的生日
                if (DateTime.Now.Month < bir.Month ||
                    (DateTime.Now.Month == bir.Month && DateTime.Now.Day < bir.Day))
                {
                    age--;
                }
            }
            return age;
        }
    }
}
