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
        UserInfoDAL userInfoDAL = null;
        LoginViewModel loginViewModel = new LoginViewModel();
        public static string userID = "";
        private Action tabAction;
        private readonly APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();

        public LoginPage()
        {
            InitializeComponent();
            DataContext = loginViewModel;
            tabAction += TabCallBackExcute;
        }

        private void TabCallBackExcute()
        {
            
        }

        private async void LoginAPI()
        {
            string url = APPSettingUtil.APP_ApiUrlLogin;
            var data = new Dictionary<string, object>
            {
                { "mbrKey", loginViewModel.UserID }
            };
            //url = "http://127.0.0.1:4523/m2/7869154-7618859-default/510152173";
            //url = "http://39.105.221.110:10013/health/rest/cardiovascularservice/getmemberbykey";
            var response = ApiBLL.DoGetUser(url, data);
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
                string raw = obj["memberEntity"]["birth"]?.ToString();
                userInfo.UserBirthday = DateTime.Parse(raw).ToString("yyyy-MM-dd");
                if (userInfo.UserName.IsNullOrEmpty() || userInfo.UserHeight.ToString().IsNullOrEmpty() || userInfo.UserWeight.ToString().IsNullOrEmpty()
                    || userInfo.UserSex.IsNullOrEmpty() || userInfo.UserBirthday.IsNullOrEmpty())
                {
                    await Dialog.Show(new UpdateDialog(userInfo, tabAction)).GetResultAsync<string>();
                    
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

        private void LoginBtn(object sender, RoutedEventArgs e)
        {
            if (loginViewModel.UserID=="")
            {
                Growl.Warning("请填写账号！");
                return;
            }

            if (APPSettingUtil.APP_Network == "单机版")
            {
                LoginLocal();
            }
            else
            {
                LoginAPI();//标准API接口获取用户信息
                //LoginAPPID();//圣乐版本获取用户信息
                //LoginAPIBX();//博谐内部系统
            }
        }
        private async void LoginLocal()
        {
            UserInfoEntity userInfo = new UserInfoEntity();
            string conditionStr = " UserId ='" + loginViewModel.UserID + "'";
            userInfo = userInfoDAL.Find(conditionStr);
            if (userInfo != null)
            {
                // 本地已有该用户：信息不完整时先弹补全界面；点“返回/取消”就留在登录页，不进入测量页
                if (IsUserInfoIncomplete(userInfo))
                {
                    string dialogResult = await Dialog.Show(new UpdateDialog(userInfo, tabAction)).GetResultAsync<string>();
                    if (dialogResult != "ok")
                        return;
                    if (IsUserInfoIncomplete(userInfo))
                    {
                        Growl.Warning("用户信息未填写完整，无法开始测量！");
                        return;
                    }
                    userInfoDAL.Update(userInfo);   // 补全后的信息写回本地库
                }
                this.NavigationService.Navigate(new MeasureReePage(userInfo));
            }
            else
            {
                userID = loginViewModel.UserID;
                UserInfoEntity u = new UserInfoEntity();
                u.UserId = loginViewModel.UserID;
                await Dialog.Show(new RegisterDialog(u, tabAction)).GetResultAsync<string>();
                // “返回”时 RegisterDialog 会清空 UserId，这里统一按信息是否完整来判断是否允许进入测量页
                if (IsUserInfoIncomplete(u))
                {
                    Growl.Warning("用户信息未填写完整，无法开始测量！");
                    return;
                }
                this.NavigationService.Navigate(new MeasureReePage(u));
            }
        }
        /// <summary>
        /// 判断用户信息是否不完整（缺账号/姓名/性别/生日/身高/体重任意一项）
        /// </summary>
        private static bool IsUserInfoIncomplete(UserInfoEntity userInfo)
        {
            return userInfo.UserId.IsNullOrEmpty() || userInfo.UserName.IsNullOrEmpty()
                || userInfo.UserSex.IsNullOrEmpty() || userInfo.UserBirthday.IsNullOrEmpty()
                || userInfo.UserHeight <= 0 || userInfo.UserWeight <= 0;
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
                GC.Collect();
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            userInfoDAL = UserInfoDAL.getInstance();
        }

        private void GetID(object sender, RoutedEventArgs e)
        {
            // 生成当天的新账号：yyyyMMdd + 三位流水号。
            // 原来用 DateTime.Today/DateTime.Now.ToString() 拼 SQL 去比较 createTime，
            // 这两个字符串受操作系统的区域设置影响，各台机器格式不一致时字符串比较就会失效
            // （自己电脑上能取到号、上位机上取不到就是这个原因）。
            // 改为直接取当天已有账号里最大的流水号，不依赖任何日期格式。
            string todayPrefix = DateTime.Now.ToString("yyyyMMdd");
            int Temp_Num = 0;
            List<UserInfoEntity> user = userInfoDAL.Finds("1 = 1");
            if (user != null)
            {
                foreach (UserInfoEntity item in user)
                {
                    string userId = item.UserId ?? "";
                    if (userId.Length == todayPrefix.Length + 3 && userId.StartsWith(todayPrefix)
                        && int.TryParse(userId.Substring(todayPrefix.Length), out int seq))
                    {
                        Temp_Num = Math.Max(Temp_Num, seq);
                    }
                }
            }
            // 再逐个往后找没被占用的号，避免与手工录入的同号账号冲突
            string newUserId;
            do
            {
                Temp_Num++;
                newUserId = todayPrefix + Temp_Num.ToString("000");
            }
            while (userInfoDAL.FindCount(" UserId ='" + newUserId + "'") > 0);
            loginViewModel.UserID = newUserId;
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
        private async void LoginAPIBX()
        {
            string url = APPSettingUtil.APP_ApiUrlLogin;
            var data = new Dictionary<string, object>
                {
                    { "userIdcard", loginViewModel.UserID }
                };
            url = "http://192.168.8.182:8800/api/v1.0/baseInfo/user/getUserByIdcard";
            var response = ApiBLL.DoGetUser(url, data);
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
