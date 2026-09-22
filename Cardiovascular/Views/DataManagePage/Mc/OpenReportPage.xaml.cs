using Cadio.BLL;
using Cardio.BLL;
using Cardio.DAL;
using Cardio.Model;
using Cardio.SPCL;
using Cardio.Util;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.Win32;
using SqlSugar;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace Cardio.Views.DataManagePage.Mc
{
    /// <summary>
    /// OpenReportPage.xaml 的交互逻辑
    /// </summary>
    public partial class OpenReportPage : Page
    {
        ReportViewModel model = new ReportViewModel();
        string reportFile = "./Resources/Template/WPFABIAI202.frx";
        private readonly APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();
        // 是否由测量页（MeasureReePage）的“打印报告”打开：
        // 是——返回时直接回到注册用户界面，且按参数设置判断是否自动上传；
        // 否（数据管理页打开）——按原逻辑返回上一页，且永远不自动上传
        private readonly bool fromMeasurePage;
        // 报告渲染（含填充数据、Prepare）是否完成，用于上传时等待报告生成完毕
        private volatile bool reportRendered;

        public OpenReportPage(PulseDataLocalEntity testData, Action<string> tAction, bool fromMeasurePage = false)
        {
            InitializeComponent();
            this.fromMeasurePage = fromMeasurePage;
            //传入参数
            model.testResult = testData;
            DataContext = model;
        }

        #region 触发方法
        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            Upload.Visibility = APPSettingUtil.APP_Network == "网络版" ? Visibility.Visible : Visibility.Collapsed;
            Grid.SetColumn(previewControl1, 0);
            Thread thread = new(() =>
            {
                model.IsRunning = "Visible";
                // 开始渲染报告
                model.InitReport(reportFile);
                model.LoadChartData();
                Dispatcher.BeginInvoke(() =>
                {
                    model.SetReport(zg_aichart);
                });
                FastReport.Export.Image.ImageExport ex;
                model.ExprtReport();
                reportRendered = true;
            });
            thread.Start();
            // 测量页打开的报告：参数设置为“自动上传”时，初始化即上传本次报告
            if (fromMeasurePage && APPSettingUtil.APP_Network == "网络版" && APPSettingUtil.APP_AutoUpload == "自动")
            {
                ThreadAPIStart();
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)//不可见主动回收资源
            {
                //TextDialog.isMeasure = false;
                this.Content = null;
                GC.Collect();
                DirectoryInfo di = new("./Resources/Report");
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.Delete(); // 删除文件
                    }
                    catch { }
                }
            }
        }

        private new void ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region 按钮方法
        private void PrintBtn(object sender, RoutedEventArgs e)
        {
            if (model.pReport == null)
                return;
            ImagePrinter.PrintImagesWithDialog(model.fileUrls);
        }

        private void SaveBtn(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveImageDialog = new()
            {
                Title = "保存文件",
                Filter = "(*.pdf)|*.pdf",
                DefaultExt = ".pdf"
            };
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                Thread thread = new Thread(() =>
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        SaveBtnO.IsEnabled = false;
                    }));
                    var export = new PDFSimpleExport();
                    model.pReport?.Export(export, saveImageDialog.FileName);
                    HandyControl.Controls.Growl.Success("保存成功！");
                    Dispatcher.BeginInvoke(new Action(() => {
                        SaveBtnO.IsEnabled = true;
                    }));
                    LogUtil.Info("导出报告成功！文件名称：" + saveImageDialog.FileName);
                });
                thread.Start();
            }
        }

        private void UploadBtn(object sender, RoutedEventArgs e)
        {
            ThreadAPIStart();
        }

        private void BackBtn(object sender, RoutedEventArgs e)
        {
            if (!fromMeasurePage)
            {
                this.NavigationService.GoBack();
                return;
            }
            // 测量流程的打印报告是最后一步：跳过测量页，直接返回到注册用户界面（LoginPage）。
            // 这里先移除测量页这条历史记录再走 GoBack，而不是 Navigate 一个新的 LoginPage：
            // Navigate 会把本页压入历史，而本页在不可见时会清空 Content，返回时就成了空白页
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.RemoveBackEntry();
            }
            this.NavigationService.GoBack();
        }
        #endregion

        #region 上传方法

        private void ThreadAPIStart()
        {
            Task.Run(()=> { InsertNet(); });
        }

        private async void InsertNet()
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    Upload.IsEnabled = false;
                }));



                await Upload_ZJ();
                //await Upload_BX();


                Dispatcher.Invoke(new Action(() => {
                    Upload.IsEnabled = true;
                }));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() => {
                    Upload.IsEnabled = true;
                }));
                HandyControl.Controls.Growl.Success("数据上传失败！");
            }
        }

        private async Task Upload_BX()  
        {
            // 报告转Base64：临时PDF → Base64 → 删除临时文件（无感，不弹保存框）
            string base64Pdf = BuildReportBase64();

            var pulseData = new Dictionary<string, object>();
            //pulseData.Add("userID", "101");
            //pulseData.Add("userName", "测试男");
            //pulseData.Add("userBirthday",  "1985-09-13");
            //pulseData.Add("userAge", 41);
            //pulseData.Add("userSex", "男");
            //pulseData.Add("userHeight", 193);
            //pulseData.Add("userWeight", 78);
            pulseData.Add("userID", model.testResult.userId ?? "");
            pulseData.Add("userName", model.testResult.userName ?? "");
            pulseData.Add("userBirthday", model.testResult.userBirthday ?? "");
            pulseData.Add("userAge", model.testResult.userAge);
            pulseData.Add("userSex", model.testResult.userSex ?? "");
            pulseData.Add("userHeight", model.testResult.userHeight);
            pulseData.Add("userWeight", model.testResult.userWeight);
            //// 添加ABI相关
            pulseData.Add("test_pul_sbp", model.testResult.Sbp);
            pulseData.Add("test_pul_dbp", model.testResult.Dbp);
            pulseData.Add("test_pul_pp", model.testResult.Pp);
            pulseData.Add("test_pul_cap", model.testResult.Sbp2);
            pulseData.Add("test_pul_hr", model.testResult.Hr);
            pulseData.Add("test_pul_ed", model.testResult.Ed);
            pulseData.Add("test_pul_spti", model.testResult.Spti);
            pulseData.Add("test_pul_dpti", model.testResult.Dpti);
            pulseData.Add("test_pul_sevr", model.testResult.Sevr);
            pulseData.Add("test_pul_ai", model.testResult.AIx);
            //诊断结果
            pulseData.Add("diagnosis_result", model.testResult.AIDiagnosisResult);
            pulseData.Add("diagnosis_proposal", model.testResult.DoctorDiagnosis);
            // 添加心脏指数相关
            // 添加测试时间、设备编号、检查结果、报告Base64
            pulseData.Add("check_up_time", model.testResult.TestDateTime ?? "");

            pulseData.Add("base64PdfString", base64Pdf);
            pulseData.Add("deviceId",APPSettingUtil.APP_DeviceId);
            pulseData.Add("id",model.testResult.Id.ToString());

            string url = APPSettingUtil.APP_ApiUrlData;
            string appid = APPSettingUtil.APP_AppId;
            string timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string nonce = Guid.NewGuid().ToString("N");
            string secret = APPSettingUtil.APP_AppSecret;
            string deviceid = APPSettingUtil.APP_DeviceId;
            BX_UploadSetting bx = new BX_UploadSetting
            {
                url = url,
                appid = appid,
                nonce = nonce,
                timestamp = timestamp,
                secret = secret,
                deviceid = deviceid
            };
            await ApiBLL.DoPostUpload_BX(bx, pulseData);
        }

        private async Task Upload_ZJ()
        {
            var data = new Dictionary<string, object>
            {
                { "userId", model.testResult.userId },
                { "userCode", model.testResult.userCode },
                { "userName", model.testResult.userName },
                { "userSex", model.testResult.userSex.Contains("男") ? "01" : "00" },
                { "userBirthday", model.testResult.userBirthday },
                { "userHeight", model.testResult.userHeight },
                { "userWeight", model.testResult.userWeight },
                { "doctorId", model.testResult.OperationgDoctor },
                { "orgId", model.testResult.orgId },
                { "checkTime", model.testResult.TestDateTime },
                { "hr", model.testResult.Hr },
                { "sbp", model.testResult.Sbp },
                { "dbp", model.testResult.Dbp },
                { "pp", model.testResult.Pp },
                { "cap", model.testResult.Sbp2},
                { "ai", model.testResult.AIx },
                { "ed", model.testResult.EdPct /100 },
                { "spti", model.testResult.Spti },
                { "dpti", model.testResult.Dpti },
                { "sevr", model.testResult.Sevr },
                { "Result", model.testResult.AIDiagnosisResult },
                { "data", model.testResult.RpRawData },
                { "proposal", model.testResult.DoctorDiagnosis },
            };

            string url = APPSettingUtil.APP_ApiUrlData;
            await ApiBLL.DoPostUpload_ZJ(url, data);
        }

        /// <summary>
        /// 把当前报告导出为临时 PDF 并转成 Base64 字符串（无感：不弹保存框，转完立即删除临时文件）
        /// </summary>
        private string BuildReportBase64()
        {
            // 报告由 Window_OnLoaded 里的后台线程渲染，自动上传时可能还没渲染完，
            // 这里等渲染完成再导出，避免传上去的是没有数据的空报告
            for (int i = 0; i < 300 && !reportRendered; i++)
            {
                Thread.Sleep(100);
            }
            if (model.pReport == null)
                return "";

            string tempFile = Path.Combine(Path.GetTempPath(), "report_" + Guid.NewGuid().ToString("N") + ".pdf");
            try
            {
                model.pReport.Export(new PDFSimpleExport(), tempFile);
                if (!System.IO.File.Exists(tempFile))
                    return "";
                return ApiBLL.ConvertPdfToBase64(tempFile);
            }
            catch (Exception ex)
            {
                LogUtil.Error("报告转Base64", ex.Message);
                return "";
            }
            finally
            {
                try
                {
                    if (System.IO.File.Exists(tempFile))
                        System.IO.File.Delete(tempFile);
                }
                catch { }
            }
        }
        #endregion

    }
}
