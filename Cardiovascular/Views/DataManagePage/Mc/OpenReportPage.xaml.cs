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

        private bool reportBusy;
        private static readonly SemaphoreSlim reportGate = new(1, 1);
        private bool reportReady;
        private bool pageClosed;

        private async void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (reportBusy || reportReady || pageClosed) return;
            Upload.Visibility = APPSettingUtil.APP_Network == "网络版" ? Visibility.Visible : Visibility.Collapsed;
            Grid.SetColumn(previewControl1, 0);
            reportBusy = true;
            model.IsRunning = "Visible";
            SaveBtnO.IsEnabled = Upload.IsEnabled = false;
            try
            {
                await reportGate.WaitAsync();
                try
                {
                    if (pageClosed) return;
                    await Task.Run(() =>
                    {
                        model.InitReport(reportFile);
                        model.LoadChartData();
                    });
                    if (pageClosed) return;
                    model.SetReport(zg_aichart);
                    await Task.Run(model.ExprtReport);
                    reportReady = model.fileUrls.Count > 0;
                    if (reportReady && !pageClosed) model.Image1 = model.fileUrls[0];
                }
                finally { reportGate.Release(); }
            }
            catch (Exception ex)
            {
                LogUtil.Error("生成报表", ex.ToString());
                if (!pageClosed) HandyControl.Controls.Growl.Error("生成报表失败：" + ex.Message);
            }
            finally
            {
                reportBusy = false;
                model.IsRunning = "Hidden";
                SaveBtnO.IsEnabled = Upload.IsEnabled = reportReady && !pageClosed;
                if (pageClosed) ReleaseReport();
            }
        }
        private void PrintBtn(object sender, RoutedEventArgs e)
        {
            if (!reportReady || reportBusy || pageClosed)
                return;
            ImagePrinter.PrintImagesWithDialog(model.fileUrls);
        }

        private async void SaveBtn(object sender, RoutedEventArgs e)
        {
            if (!reportReady || reportBusy || pageClosed) return;
            using var dialog = new System.Windows.Forms.SaveFileDialog
            {
                Title = "保存文件",
                Filter = "(*.pdf)|*.pdf",
                DefaultExt = ".pdf"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string fileName = dialog.FileName;
            reportBusy = true;
            SaveBtnO.IsEnabled = false;
            try
            {
                await reportGate.WaitAsync();
                try
                {
                    await Task.Run(() =>
                    {
                        using var export = new PDFSimpleExport();
                        model.pReport.Export(export, fileName);
                    });
                }
                finally { reportGate.Release(); }
                if (!pageClosed) HandyControl.Controls.Growl.Success("保存成功！");
                LogUtil.Info("导出报告成功！文件名称：" + fileName);
            }
            catch (Exception ex)
            {
                LogUtil.Error("导出报表", ex.ToString());
                if (!pageClosed) HandyControl.Controls.Growl.Error("保存失败：" + ex.Message);
            }
            finally
            {
                reportBusy = false;
                SaveBtnO.IsEnabled = !pageClosed;
                if (pageClosed) ReleaseReport();
            }
        }

        private void UploadBtn(object sender, RoutedEventArgs e)
        {
            ThreadAPIStart();
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) return;
            pageClosed = true;
            this.Content = null;
            if (!reportBusy) ReleaseReport();
        }

        private void ReleaseReport()
        {
            model.pReport?.Dispose();
            model.pReport = null;
            // 只清理本报告的文件，等待生成/导出任务结束后再释放。
            foreach (string file in model.fileUrls)
            {
                try { System.IO.File.Delete(file); }
                catch (IOException ex) { LogUtil.Error("清理报告", ex.Message); }
                catch (UnauthorizedAccessException ex) { LogUtil.Error("清理报告", ex.Message); }
            }
            model.fileUrls.Clear();
        }

        private async Task InsertNetAsync()
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
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
                Dispatcher.Invoke(new Action(() => {
                    Upload.IsEnabled = true;
                }));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() => {
                    Upload.IsEnabled = true;
                }));
                HandyControl.Controls.Growl.Error("数据上传失败！");
                LogUtil.Error("上传数据", ex.Message);
            }

        }

        private async void ThreadAPIStart()
        {
            if (!Upload.IsEnabled || pageClosed) return;
            await InsertNetAsync();
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
