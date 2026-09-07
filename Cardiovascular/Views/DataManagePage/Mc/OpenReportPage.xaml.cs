using Cadio.BLL;
using Cardio.BLL;
using Cardio.DAL;
using Cardio.Model;
using Cardio.Util;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.Win32;
using System.IO;
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
        /// <summary>
        /// 数据绑定
        /// </summary>
        #region
        #endregion
        private Report pReport = null;   //实例化一个Report报表; //新建一个私有变量
        ReportViewModel model = new ReportViewModel();
        string reportFile = "./Resources/Template/WPFABIAI202.frx";
        private readonly APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();
        List<string> fileUrls = new List<string>();

        public OpenReportPage(PulseDataLocalEntity testData, Action<string> tAction)
        {
            InitializeComponent();
            //传入参数
            model.testResult = testData;
            DataContext = model;
        }

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
            });
            thread.Start();
        }
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

        private async void InsertNet()
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    Upload.IsEnabled = false;
                }));

                string url = APPSettingUtil.APP_ApiUrlData;
                //url = "http://127.0.0.1:4523/m2/7869154-7618859-default/510691540";
                //url = "http://39.105.221.110:10013/health/rest/cardiovascularservice/uploadcardiovascular";
                var data = new Dictionary<string, object>
                {
                    { "userId", model.testResult.userId },
                    { "userCode", model.testResult.userCode },
                    { "userName", model.testResult.userName },
                    { "userSex", model.testResult.userSex },
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
                    { "ed", model.testResult.Ed/100 },
                    { "spti", model.testResult.Spti },
                    { "dpti", model.testResult.Dpti },
                    { "sevr", model.testResult.Sevr },
                    { "Result", model.testResult.AIDiagnosisResult },
                    { "data", model.testResult.RpRawData },
                    { "proposal", model.testResult.AIDiagnosisProposal },
                    { "reportName", model.testResult.Report_Name },
                };

                //var data = new Dictionary<string, object>
                //{
                //    { "userId", "18955154603-mbr" },
                //    { "userCode", "18955154603" },
                //    { "userName", "项龙飞" },
                //    { "userSex", "01" },
                //    { "userBirthday", "1988-01-01 00:00:00" },
                //    { "userHeight", "165" },
                //    { "userWeight", "57" },
                //    { "doctorId", "1567153547218-0001-0070-1615" },
                //    { "orgId", "0001" },
                //    { "checkTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
                //    { "hr", "60" },
                //    { "sbp", "140" },
                //    { "dbp", "80" },
                //    { "pp", "40" },
                //    { "cap", "100"},
                //    { "ai", "80" },
                //    { "ed", "0.30" },
                //    { "spti", "110" },
                //    { "dpti", "90" },
                //    { "sevr", "120" },
                //    { "Result", "未见异常" },
                //    { "data", "原始波形采样字符串" },
                //    { "proposal", "建议控制饮食，规律作息，定期监测血压" },
                //    { "reportName", "2026 年心血管检测报告" },
                //};

                await ApiBLL.DoPostUpload(url, data);
                Dispatcher.BeginInvoke(new Action(() => {
                    Upload.IsEnabled = true;
                }));
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    Upload.IsEnabled = true;
                }));
                HandyControl.Controls.Growl.Success("数据上传失败！");
                LogUtil.Error("上传数据", ex.Message);
            }

        }

        private void ThreadAPIStart()
        {
            Thread thread = new(() =>
            {
                InsertNet();
            });
            thread.Start();
        }

        private void BackBtn(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private new void ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
