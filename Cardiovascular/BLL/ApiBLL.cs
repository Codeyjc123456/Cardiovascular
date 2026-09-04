using Cardio.DAL;
using Cardio.Model;
using Cardio.SPCL;
using Cardio.Util;
using HandyControl.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cardio.BLL
{
    public static class ApiBLL
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async static Task<UserInfoEntity> LoginAPI(Dictionary<string, object> data)
        {
             
            string url = APPSettingsViewModel.getInstance().APP_ApiUrlLogin;
            try
            {
                UserInfoEntity userInfo = new UserInfoEntity();
                var response = await HttpUtil.DoPost(url, data);
                var response_json = (JObject)JsonConvert.DeserializeObject(response);
                if ((string)response_json["code"] == "200")
                {
                    var data_json = (JObject)JsonConvert.DeserializeObject(response_json["data"].ToString());
                    userInfo.UserHeight = Convert.ToDouble(data_json["UserHeight"]);
                    userInfo.UserWeight = Convert.ToDouble(data_json["UserWeight"]);
                    userInfo.UserName = data_json["UserName"].ToString();
                    userInfo.UserId = data_json["UserID"].ToString();
                    userInfo.UserSex = data_json["UserSex"].ToString();
                    userInfo.UserAge = Convert.ToInt32(data_json["UserAge"].ToString());
                    return userInfo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Info("", ex.Message);
                return null;
            }
        }

        public async static Task<int> CommitDataAPI(PulseDataLocalEntity testData,string filePath)
        {
            string url = APPSettingsViewModel.getInstance().APP_ApiUrlData; ;
            try
            {
                Dictionary<string, object> data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(testData));
                var response = await HttpUtil.DoPostDataAndFile(url, data,filePath);
                var response_json = (JObject)JsonConvert.DeserializeObject(response);
                if ((string)response_json["code"] == "200")
                {
                    return 200;
                }
                else
                {
                    return 300;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Info("", ex.Message);
                return 400;
            }
        }
        public static int CommitDataAPI(string url, Object testData, string filePath)
        {
            try
            {
                string JsonData = JsonConvert.SerializeObject(testData);
                string base64PDF = ConvertPdfToBase64(filePath);
                JsonData = JsonData.Replace("}", ",\"base64Pdf\":\"" + base64PDF + "\"}");
                var response = DoPostBase64(url, JsonData);
                var response_json = (JObject)JsonConvert.DeserializeObject(response);
                if ((string)response_json["code"] == "200")
                {
                    return 200;
                }
                else
                {
                    return 300;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Info("", ex.Message);
                return 400;
            }
        }
        public static string ConvertPdfToBase64(string pdfFilePath)
        {
            byte[] pdfBytes;
            using (FileStream pdfFile = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read))
            {
                pdfBytes = new byte[pdfFile.Length];
                pdfFile.Read(pdfBytes, 0, pdfBytes.Length);
            }
            string base64Pdf = Convert.ToBase64String(pdfBytes);
            return base64Pdf;
        }
        public static string DoPostBase64(string url, string data)
        {
            try
            {
                var request = new RestRequest(url, Method.Post);
                using (HttpClient client = new HttpClient())
                {
                    var requestContent = new StringContent(data, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, requestContent).Result;
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                return "{'code':'400','data':'}" + e.Message + "'}";
            }

        }
        public static string DoPostUser(string url, string data)
        {
            try
            {
                var request = new RestRequest(url, Method.Post);
                using (HttpClient client = new HttpClient())
                {
                    var requestContent = new StringContent(data, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, requestContent).Result;
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                return "{'code':'400','data':'}" + e.Message + "'}";
            }

        }
        public static string DoGetUser(string url, Dictionary<string, object> parameters = null)
        {
            try
            {
                // 构建带参数的完整URL
                if (parameters != null && parameters.Count > 0)
                {
                    var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
                    foreach (var param in parameters)
                    {
                        query[param.Key] = param.Value?.ToString();
                    }
                    url = $"{url}?{query}";
                }

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                return "{'code':'400','data':'}" + e.Message + "'}";
            }
        }
        public static  (UserInfoEntity, string) GetUser(string url, string dataStic)
        {
            UserInfoEntity userInfo = new UserInfoEntity();
            string callback = "";
            try
            {
                // VB6中的参数初始化
                string secret = "s7N9dxjNhANNyuzN";
                string loginID = string.Empty;
                // 计算时间戳（Unix时间戳，UTC+8）
                var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var now = DateTime.UtcNow.AddHours(8); // 转换为UTC+8
                var timestamp = (long)(now - baseDate).TotalSeconds;
                // 计算签名
                var signSource = secret + timestamp.ToString();
                var sign = CalculateMD5Hash(signSource);
                sign = sign.ToUpper();
                // 构建请求参数
                var postData = $"page=1&limit=10&keyword={WebUtility.UrlEncode(dataStic)}";
                var postDataBytes = Encoding.UTF8.GetBytes(postData);
                // 创建HTTP请求（替代WinHttp.WinHttpRequest）
                using (var client = new WebClient())
                {
                    // 设置请求头
                    client.Headers.Add("timestamp", timestamp.ToString());
                    client.Headers.Add("sign", sign);
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    // 发送POST请求
                    var responseBytes = client.UploadData(url, "POST", postDataBytes);
                    var responseContent = Encoding.UTF8.GetString(responseBytes);
                    JObject jsonobject = JObject.Parse(responseContent);
                    var json = JsonConvert.DeserializeObject(responseContent);
                    // 提取code和msg（关键！）
                    int code = jsonobject["code"]?.Value<int>() ?? -1;
                    string msg = jsonobject["msg"]?.ToString() ?? "未知错误";
                    switch (code)
                    {
                        case 200:
                            // 解析JSON（替代VB6的JSON.parse）
                            var jsonObject = JObject.Parse(responseContent);
                            var userList = jsonObject["data"]["list"];
                            // 遍历用户列表
                            foreach (var user in userList)
                            {
                                userInfo.UserName = user["userName"]?.ToString() ?? string.Empty;
                                userInfo.UserId = user["userSn"]?.ToString() ?? string.Empty;
                                userInfo.UserPhone = user["userPhone"]?.ToString() ?? string.Empty;
                                userInfo.UserSex = user["userSex"]?.ToString() ?? string.Empty;
                                // 处理生日和年龄
                                var birthday = user["userBirthday"]?.ToString() ?? string.Empty;
                                userInfo.UserBirthday = birthday.Replace("/", "-");

                                if (DateTime.TryParse(userInfo.UserBirthday, out var birthDate))
                                {
                                    userInfo.UserAge = DateTime.Now.Year - birthDate.Year;
                                    // 修正年龄计算（未到生日的情况）
                                    if (DateTime.Now < birthDate.AddYears(userInfo.UserAge))
                                        userInfo.UserAge--;
                                }
                                // 处理身高体重
                                double.TryParse(user["userHeight"]?.ToString(), out var height);
                                userInfo.UserHeight = height;

                                double.TryParse(user["userWeight"]?.ToString(), out var weight);
                                userInfo.UserWeight = weight;
                            }
                            callback = $"获取用户{userInfo.UserName}成功！";
                            break;
                        case 400:
                            callback = "请求参数错误或缺失！";
                            break;
                        case 500:
                            callback = "内部服务器出错！";
                            break;
                        case 3001:
                            callback = "错误的请求方式！";
                            break;
                        case 3002:
                            callback = "签名错误,请同步设备时间！";
                            break;
                        case 10201:
                            callback = "报告单已经上传过！";
                            break;
                        case 10202:
                            callback = "报告单上传失败！";
                            break;
                        case 10203:
                            callback = "用户未找到！";
                            break;
                        case 10204:
                            callback = "用户体重或身高信息不完整！";
                            break;
                        default:
                            callback = $"未知响应代码: {msg}";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // WPF中显示错误提示
                HandyControl.Controls.MessageBox.Show($"请求用户信息失败：{ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                callback = $"未知响应代码,错误信息：{ex.Message}";
            }

            return (userInfo, callback);
        }
        private static readonly HttpClient _httpClient = new HttpClient();
        public static void DataUploadHelper()
        {
            // 初始化HTTP客户端
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        public static string UploadDataPost(string url, PulseDataLocalEntity data)
        {
            int typeAsOrCa;
            string callback = "";
            try
            {
                // 1. 初始化基础参数
                var secret = "s7N9dxjNhANNyuzN";
                // 计算时间戳（和VB6完全对齐：UTC+8，1970-01-01 00:00:00）
                var baseDate = new DateTime(1970, 1, 1, 0, 0, 0); // 本地时区（UTC+8）
                var now = DateTime.Now;
                var timestamp = (long)(now - baseDate).TotalSeconds;
                // 2. 构建请求参数字典
                var pulseData = new Dictionary<string, string>();
                // 添加用户基础信息
                pulseData.Add("userSn", data.userId ?? "");
                pulseData.Add("userName", data.userName ?? "");
                pulseData.Add("userSex", data.userSex ?? "");
                pulseData.Add("userAge", data.userAge.ToString());
                pulseData.Add("userHeight", data.userHeight.ToString());
                pulseData.Add("userWeight", data.userWeight.ToString());
                // 添加ABI相关
                pulseData.Add("Sbp", data.Sbp.ToString());
                pulseData.Add("Dbp", data.Dbp.ToString());
                pulseData.Add("Pp", data.Pp.ToString());
                pulseData.Add("Map", data.Map.ToString());
                pulseData.Add("Hr", data.Hr.ToString());
                pulseData.Add("BpHr", data.BpHr.ToString());
                pulseData.Add("Distance", data.Distance.ToString());
                pulseData.Add("Ed", data.Ed.ToString());
                pulseData.Add("Spti", data.Spti.ToString());
                pulseData.Add("Dpti", data.Dpti.ToString());
                pulseData.Add("Sevr", data.Sevr.ToString());
                pulseData.Add("AIx", data.AIx.ToString());
                pulseData.Add("AIAssess", data.AIAssess.ToString());
                pulseData.Add("AIDiagnosisReult", data.AIDiagnosisResult.ToString());
                pulseData.Add("AIDiagnosisProposal", data.AIDiagnosisProposal.ToString());
                pulseData.Add("lbPwv", data.Sbp2.ToString());
                typeAsOrCa = 3;
                // 添加心脏指数相关
                // 添加测试时间、类型、检查结果、报告Base64
                pulseData.Add("TestDateTime", data.TestDateTime ?? "");
                pulseData.Add("type", typeAsOrCa.ToString());
                pulseData.Add("checkResult", "1");
                //Variable.base64pdf = "1";
                pulseData.Add("reportBase64", Variable.base64pdf);
                // 3. 拼接请求参数（x-www-form-urlencoded格式）
                var contentBuilder = new StringBuilder();
                foreach (var key in pulseData.Keys)
                {
                    if (contentBuilder.Length > 0)
                        contentBuilder.Append("&");
                    // URL编码（避免特殊字符问题）
                    contentBuilder.Append($"{System.Web.HttpUtility.UrlEncode(key)}={System.Web.HttpUtility.UrlEncode(pulseData[key])}");
                }
                var postContent = contentBuilder.ToString();
                // 4. 计算签名（和VB6逻辑一致：secret + timestamp）
                var signSource = secret + timestamp.ToString();
                var sign = CalculateMD5Hash(signSource); // 复用之前的MD5方法
                sign = sign.ToUpper();
                // ===== 修复核心：替换Send方法 =====
                //// 方式1：用FormUrlEncodedContent构建请求（推荐，自动处理编码）
                var formData = new MultipartFormDataContent();
                //为每个字段添加StringContent
                foreach (var item in pulseData)
                {
                    //对reportBase64使用更合适的编码方式
                    if (item.Key == "reportBase64")
                        // 对于特别长的 Base64 字符串，可以考虑分块或压缩
                        formData.Add(new StringContent(item.Value), item.Key);
                    else
                        formData.Add(new StringContent(item.Value), item.Key);
                }
                // 添加timestamp和sign请求头
                formData.Headers.Add("timestamp", timestamp.ToString());
                formData.Headers.Add("sign", sign);
                // 发送同步POST请求（PostAsync + .Result 替代Send）
                HttpResponseMessage response = _httpClient.PostAsync(url, formData).Result;
                // ==================================
                // 4. 处理响应（逻辑不变）
                var responseContent = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HandyControl.Controls.MessageBox.Show($"响应内容：{responseContent}", "调试信息");
                    // 解析响应（复用通用响应类）
                    var jsonObject = JObject.Parse(responseContent);
                    int code = (int)jsonObject["code"];
                    switch (code)
                    {
                        case 200:
                            callback = "数据上传成功！";
                            break;
                        case 400:
                            callback = "请求参数错误或缺失！";
                            break;
                        case 500:
                            callback = "内部服务器出错！";
                            break;
                        case 3001:
                            callback = "错误的请求方式！";
                            break;
                        case 3002:
                            callback = "签名错误！";
                            break;
                        case 10201:
                            callback = "报告单已经上传过！";
                            break;
                        case 10202:
                            callback = "报告单上传失败！";
                            break;
                        case 10203:
                            callback = "用户未找到！";
                            break;
                        case 10204:
                            callback = "用户体重或身高信息不完整！";
                            break;
                        case 10205:
                            callback = "报告单base64缺失！";
                            break;
                    }
                    return callback;
                }
                else
                    HandyControl.Controls.MessageBox.Show($"请求失败，状态码：{response.StatusCode}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show($"上传数据异常：{ex.Message}\n{ex.StackTrace}", "异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return callback;
        }
        // MD5计算方法
        private static string CalculateMD5Hash(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        /// <returns>Base64编码字符串（失败返回空）</returns>
        public static async Task<string> ConvertPdfToBase64Async(string pdfFilePath)
        {
            try
            {
                if (!File.Exists(pdfFilePath))
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        HandyControl.Controls.MessageBox.Show($"PDF文件不存在：{pdfFilePath}", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return string.Empty;
                }
                var fileExtension = Path.GetExtension(pdfFilePath).ToLower();
                if (fileExtension != ".pdf")
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        HandyControl.Controls.MessageBox.Show($"文件不是PDF格式：{pdfFilePath}", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return string.Empty;
                }
                byte[] pdfBytes;
                using (var fs = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    pdfBytes = new byte[fs.Length];
                    // 异步读取文件流
                    await fs.ReadAsync(pdfBytes, 0, pdfBytes.Length);
                }
                string base64Str = Convert.ToBase64String(pdfBytes);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Growl.Info($"PDF转换Base64成功！\n文件大小：{pdfBytes.Length / 1024:F2} KB");
                });
                return base64Str;
            }
            catch (IOException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    HandyControl.Controls.MessageBox.Show($"读取PDF文件失败：{ex.Message}\n可能文件被占用",
                          "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                return string.Empty;
            }
        }

        public static async Task DoPostUpload(string url,Dictionary<string, object> data)
        {
            try
            {
                var response = await HttpUtil.DoPost(url, data);
                var response_json = (JObject)JsonConvert.DeserializeObject(response);
                if ((string)response_json["ERROR_TYPE"] =="0")
                {
                    HandyControl.Controls.Growl.Success("数据上传成功！");
                }
                else
                {
                    HandyControl.Controls.Growl.Error("数据上传失败！");
                }
            }
            catch (Exception ex)
            {
                HandyControl.Controls.Growl.Error("数据上传失败！");
                LogUtil.Error("上传数据", ex.Message);
            }
        }
    }
}
