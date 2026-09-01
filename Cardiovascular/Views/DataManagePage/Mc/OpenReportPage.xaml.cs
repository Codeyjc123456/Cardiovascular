using Cadio.BLL;
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
                Dispatcher.Invoke(() =>
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
                    Dispatcher.Invoke(new Action(() => {
                        SaveBtnO.IsEnabled = false;
                    }));
                    var export = new PDFSimpleExport();
                    model.pReport?.Export(export, saveImageDialog.FileName);
                    HandyControl.Controls.Growl.Success("保存成功！");
                    Dispatcher.Invoke(new Action(() => {
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
                Dispatcher.Invoke(new Action(() => {
                    Upload.IsEnabled = false;
                }));
                //PDFExport export = new PDFExport();
                //string filePath = System.IO.Directory.GetCurrentDirectory() + "/"+ testResult.Report_Name;
                //.Export(pReport,filePath);
                //var code = await ApiBLL.CommitDataAPI(testResult, filePath);
                //if (code == 200)
                {
                    HandyControl.Controls.Growl.Success("数据上传成功！");
                }
                //else
                {
                    HandyControl.Controls.Growl.Success("数据上传失败！");
                }
                //File.Delete(filePath);
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
