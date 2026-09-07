
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandyControl.Data;
using Cardio.Model;
using MiniExcelLibs;
using HandyControl.Controls;
using Cardio.Views.MeasurePage;
using HandyControl.Tools.Extension;
using Cardio.DAL;
using Cardio.Util;

namespace Cardio.Views.DataManagePage.Mc
{
    /// <summary>
    /// DataListPage.xaml 的交互逻辑
    /// </summary>
    public partial class DataListPage : Page
    {

        private Action<string> ReportAction;
        Dialog d = null;
        PulseDataLocalDAL dataDAL = null;
        int size = 10;
        List<PulseDataLocalEntity> datalist = null;
        DataListViewModel<PulseDataLocalEntity> model = new DataListViewModel<PulseDataLocalEntity>();
        private Action<PulseDataLocalEntity> taAction;
        int recordTotal = 0;

        public DataListPage()
        {
            InitializeComponent();
            DataContext = model;
            taAction += TabCallBackExcute;
        }

        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            dataDAL = PulseDataLocalDAL.getInstance();
            Search();
        }

        public void ReportCallBackExcute(string t)
        {

        }

        private new void ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void Search()
        {
            Thread thread = new Thread(() =>
            {
                model.IsRunning = "Visible";
                GetDataList();
                model.IsRunning = "Hidden";
            });
            thread.Start();
        }

        private void GetDataList()
        {
            try
            {
                string conditionStr = "(userID like '%" + model.SearchText + "%' or userName like '%" + model.SearchText + "%')";
                string startTime = "";
                string endTime = "";
                Dispatcher.BeginInvoke(new Action(() => {
                    startTime = StartTime.Text;
                    endTime = EndTime.Text;
                }));

                DateTime date1 = new DateTime();
                DateTime date2 = new DateTime();
                if (startTime != "")
                {
                    date1 = DateTime.Parse(startTime);
                    startTime = date1.ToString("yyyy-MM-dd");
                }
                if (endTime != "")
                {
                    date2 = DateTime.Parse(endTime);
                    endTime = date2.ToString("yyyy-MM-dd");
                }

                if (startTime == "" && endTime != "")
                {
                    conditionStr += " and TestDateTime <= '" + endTime + " '";
                }
                else if (startTime != "" && endTime == "")
                {
                    conditionStr += " and TestDateTime >= '" + startTime + "'";
                }
                else if (startTime != "" && endTime != "")
                {
                    conditionStr += " and TestDateTime >= '" + startTime + "' and TestDateTime <= '" + endTime + " '";
                }
                
                datalist = dataDAL.Finds(model.PageIndex, size, ref recordTotal, conditionStr, "id desc");//查全部
                if (datalist != null && datalist.Count > 0)  //如果搜索到
                {
                    model.DataList = datalist;
                    model.RecordTotal = recordTotal;
                }
                else //如果未搜索到
                {
                    model.RecordTotal = 0;
                    model.DataList = datalist;
                    Growl.Info("无任何记录！");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("datalist", ex.Message);
            }   
        }

        // 页码改变 
        private void PageNextUpdated(object sender, RoutedEventArgs e)
        {
            if (recordTotal > size * model.PageIndex)
            {
                model.PageIndex++;
                Search();
            }
            else
            {
                Growl.Warning("已经是最后一页！");
            }
        }

        private void PageLastUpdated(object sender, RoutedEventArgs e)
        {
            if (model.PageIndex > 1)
            {
                model.PageIndex--;
                Search();
            }
            else
            {
               Growl.Warning("已经是第一页！");
            }
        }

        private void OpenReport(object sender, RoutedEventArgs e)
        {
            var id = (sender as System.Windows.Controls.Button).Tag.ToString();
            model.IsRunning = "Visible";
            PulseDataLocalEntity u = dataDAL.FindByID(Convert.ToInt32(id));
            if (u != null)
            {
                this.NavigationService.Navigate(new OpenReportPage(u, ReportAction));
            }
            else
            {
                Growl.Warning("未找到相关记录！");
            }
        }

        private async void DiagnosisOpen(PulseDataLocalEntity testdata)
        {
            await Dialog.Show(new Diagnosis(testdata)).GetResultAsync<string>();
            if (testdata != null)
            {
                this.NavigationService.Navigate(new OpenReportPage(testdata, ReportAction));
            }
        }

        private void TabCallBackExcute(PulseDataLocalEntity user)
        {
            var id = Tag.ToString();
        }

        private void SerachBtn(object sender, RoutedEventArgs e)
        {
            model.PageIndex = 1;
            SearchTestData();
        }

        private void ExportBtn(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.Filter = "Excel(*.xlsx)|*.xlsx";
           
            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string name = save.FileName;
                ExcelInserNet(name);
            }
        }

        private void SearchTestData()
        {
            try
            {
                string conditionStr = "";
                if (model.SearchText == "")
                {
                    conditionStr = "1=1";
                }
                else
                {
                    conditionStr = "(userID like '%" + model.SearchText + "%' or userName like '%" + model.SearchText + "%')";
                }

                string startTime = "";
                string endTime = "";
                
                Dispatcher.BeginInvoke(() => {
                    startTime = StartTime.Text;
                    endTime = EndTime.Text;
                });

                DateTime date = new DateTime();
                if (startTime != "")
                {
                    date = DateTime.Parse(startTime);
                    startTime = date.ToString("yyyy-MM-dd");
                }
                if (endTime != "")
                {
                    date = DateTime.Parse(endTime);
                    endTime = date.ToString("yyyy-MM-dd");
                }

                if (startTime == "" && endTime != "")
                {
                    conditionStr += " and TestDateTime <= '" + endTime + " '";
                }
                else if (startTime != "" && endTime == "")
                {
                    conditionStr += " and TestDateTime >= '" + startTime + "'";
                }
                else if (startTime != "" && endTime != "")
                {
                    conditionStr += " and TestDateTime >= '" + startTime + "' and TestDateTime <= '" + endTime + " '";
                }

                datalist = dataDAL.Finds(conditionStr);//查全部

                Console.WriteLine("总数：" + datalist.Count);
                if (datalist != null && datalist.Count > 0)  //如果搜索到
                {
                    model.DataList = datalist;
                    model.RecordTotal = datalist.Count;
                }
                else //如果未搜索到
                {
                    model.RecordTotal = 0;
                    model.DataList = datalist;
                    Growl.Info("无任何记录！");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("datalist", ex.Message);
                Growl.Info("请检查数据搜索信息输入是否正确!" + ex.Message);
            }
        }

        private void ExcelInserNet(string name)
        {
            SearchTestData();
            if (datalist.Count == 0)
            {
                Growl.Info("无任何记录！");
                return;
            }

            List<Dictionary<string, object>> values = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>
                {
                    {"账号",datalist[0].userId },
                    {"姓名",datalist[0].userName },
                    {"性别",datalist[0].userSex },
                    {"年龄",datalist[0].userAge },
                    {"身高",datalist[0].userHeight },
                    {"体重",datalist[0].userWeight },
                    {"HR",datalist[0].Hr },
                    {"ED",datalist[0].Ed },
                    {"SPTI",datalist[0].Spti },
                    {"DPTI",datalist[0].Dpti },
                    {"SEVR",datalist[0].Sevr },
                    {"SBP",datalist[0].Sbp },
                    {"DBP",datalist[0].Dbp },
                    {"PP",datalist[0].Pp },
                    {"SBP2",datalist[0].Sbp2 },
                    {"AI",datalist[0].AIx },
                    {"诊断结果",datalist[0].AIDiagnosisResult },
                    {"指导建议",datalist[0].AIDiagnosisProposal },
                    {"测量时间",datalist[0].TestDateTime }
                }
            };
            for (int i = 1; i < datalist.Count; i++)
            {
                Dictionary<string, object> values1 = new Dictionary<string, object>{
                    {"账号",datalist[i].userId },
                    {"姓名",datalist[i].userName },
                    {"性别",datalist[i].userSex },
                    {"年龄",datalist[i].userAge },
                    {"身高",datalist[i].userHeight },
                    {"体重",datalist[i].userWeight },
                    {"HR",datalist[i].Hr },
                    {"ED",datalist[i].Ed },
                    {"SPTI",datalist[i].Spti },
                    {"DPTI",datalist[i].Dpti },
                    {"SEVR",datalist[i].Sevr },
                    {"SBP",datalist[i].Sbp },
                    {"DBP",datalist[i].Dbp },
                    {"PP",datalist[i].Pp },
                    {"SBP2",datalist[i].Sbp2 },
                    {"AI",datalist[i].AIx },
                    {"诊断结果",datalist[i].AIDiagnosisResult },
                    {"指导建议",datalist[i].AIDiagnosisProposal },
                    {"测量时间",datalist[i].TestDateTime },
                };
                values.Add(values1);
            }

            try
            {
                //导出的文件名
                MiniExcel.SaveAs(name, values);
                Growl.Success("数据导出完成，共" + datalist.Count + "条！");
            }
            catch (Exception e)
            {
                LogUtil.Info("数据导出失败：" + e.Message);
            }
        }

        private void OpenDelete(object sender, RoutedEventArgs e)
        {
            var id = (sender as System.Windows.Controls.Button).Tag.ToString();
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show("是否确定继续操作？", "删除提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // 继续操作的逻辑
                bool isSuccess = dataDAL.Delete(Convert.ToInt32(id));
                if (isSuccess)
                {
                    Growl.Success("删除用户成功！");
                    model.PageIndex = 1;
                    Search();
                }
                else
                {
                  Growl.Warning("删除用户失败！");
                }
            }
            else
            {
               Growl.Info("已经取消删除操作！");
            }
        }

        private void BackBtn(object sender, RoutedEventArgs e)
        {
             NavigationService.GoBack();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)//不可见主动回收资源
            {
                model.IsRunning = "Hidden";
                GC.Collect();
            }
        }

    }
}
