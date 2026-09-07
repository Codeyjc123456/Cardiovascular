using Cardio.DAL;
using Cardio.Util;
using FastReport;
using HandyControl.Controls;
using Mysqlx.Prepare;
using ScottPlot;
using ScottPlot.WPF;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using SDColor = System.Drawing.Color;

namespace Cardio.Model
{
    public class ReportViewModel : BaseViewModel
    {
        public PulseDataLocalEntity testResult = new();// 测试的结果实体
        public Report pReport = null;   //实例化一个Report报表; //新建一个私有变量
        public List<double> AIData = new();
        public readonly string chineseSimpleFrl = "./Resources/Template/Chinese.frl";
        APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();
        public readonly List<string> fileUrls = new();
        private string image1 = "";
        public string Image1
        {
            get => image1;
            set => SetProperty(ref image1, value);
        }


        private string isRunning = "Hidden";
        public string IsRunning
        {
            get => isRunning;
            set => SetProperty(ref isRunning, value);
        }


        public void LoadChartData()
        {
            string aidatalist = testResult.RpRawData;
            try
            {
                string[] aiList = aidatalist.Split(';');
                for (int i = 0; i < aiList.Length; i++)
                    AIData.Add(Convert.ToDouble(aiList[i]));
            }
            catch (Exception e)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    Growl.Info("桡动脉采集数据不理想，未能成功绘制波形，待重新采集！" + e.Message)));
            }
        }
        public void InitReport(string reportFile)
        {
            FastReport.Utils.Res.LoadLocale(chineseSimpleFrl);
            pReport = new Report();
            pReport.Load(reportFile);  //载入报表文件
        }
        public void SetReport(WpfPlot zg_ai)
        {
            InitializeZedGraph(zg_ai);
            SetTitlePic();
            SetChartPic(zg_ai, "PictureAI");
        }
        public void ExprtReport()
        {
            FastReport.Export.Image.ImageExport ex;
            SetReportData();
            pReport?.Prepare();
            try
            {
                using (ex = new FastReport.Export.Image.ImageExport())
                {
                    ex.HasMultipleFiles = true;
                    ex.ImageFormat = FastReport.Export.Image.ImageExportFormat.Png;
                    ex.Resolution = 192;
                    ex.Export(pReport, AppDomain.CurrentDomain.BaseDirectory + "/Resources/Report/" + Guid.NewGuid().ToString("N") + ".png");
                    foreach (string file in ex.GeneratedFiles)
                        fileUrls.Add(file);

                }
            }
            catch (Exception a)
            {
                LogUtil.Error("OpenReport", a.Message);
                throw;
            }
        }//将对象转换为 DataTable（每个属性对应一列，单行）
        public DataTable ObjectToDataTable(object obj)
        {
            DataTable dt = new("TestData");
            if (obj == null) return dt;
            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dt.Columns.Add(prop.Name, type);
            }
            DataRow row = dt.NewRow();
            foreach (var prop in props)
            {
                var value = prop.GetValue(obj);
                row[prop.Name] = value ?? DBNull.Value;
            }
            dt.Rows.Add(row);
            return dt;
        }
        public void SetReportData()
        {
            SetTxt("txt_userId", testResult.userId);
            SetTxt("txt_userName", testResult.userName);
            SetTxt("txt_userSex", testResult.userSex);
            SetTxt("txt_userAge", testResult.userAge + " 岁");
            SetTxt("txt_userHeight", testResult.userHeight.ToString() + " cm");
            SetTxt("txt_userWeight", testResult.userWeight.ToString() + " kg");
            SetTxt("TestDate", testResult.TestDateTime);
            // 指标异常标注：超出正常范围显示 ↑/↓ 并变红（范围与测量界面一致）
            SetTxtWithMark("txt_HR",   testResult.Hr,   60, 100,   "f0");
            SetTxtWithMark("txt_ED",   testResult.Ed,   30, 45,    "f2");
            SetTxtWithMark("txt_SPTI", testResult.Spti, 1800, 2500, "f0");
            SetTxtWithMark("txt_DPTI", testResult.Dpti, 2300, 3500, "f0");
            SetTxtWithMark("txt_SEVR", testResult.Sevr, 1.0, 4,    "f2");
            SetTxtWithMark("txt_SBP",  testResult.Sbp,  90, 130,  "f0");
            SetTxt("txt_title", APPSettingUtil.APP_OwnerSet);
            SetTxt("txt_SBP1", testResult.Sbp.ToString() + "mmHg");
            SetTxtWithMark("txt_DBP",  testResult.Dbp,  60, 90,   "f0");
            SetTxt("txt_DBP1", testResult.Dbp.ToString() + "mmHg");
            SetTxtWithMark("txt_PP",   testResult.Pp,   30, 45,   "f0");
            SetTxtWithMark("txt_SBP2", testResult.Sbp2, 85, 110,  "f0");
            SetTxtWithMark("txt_AI",   testResult.AIx,  0, 0.75,  "f2");
            SetTxt("txt_AIDiagnosisResult", testResult.AIDiagnosisResult.ToString());
            SetTxt("txt_AIDiagnosisProposal", testResult.AIDiagnosisProposal.ToString());
            SetTxt("txt_title", APPSettingUtil.APP_CompaneTitle);
        }
        public void SetTitlePic()
        {
            PictureObject title = pReport?.FindObject("Pic_title") as PictureObject;
            if (title != null)
            {
                try
                {
                    string imagePath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Resources",
                        "Template",
                        "title.jpg"
                    );
                    if (File.Exists(imagePath))
                    {
                        // 方法1：直接通过文件路径加载（FastReport 支持）
                        title.ImageLocation = imagePath;  // 直接指定图片路径
                        // 方法2：通过 Image 对象加载（更灵活）
                        // using System.Drawing;
                        // Image img = Image.FromFile(imagePath);
                        // title.Image = img;  // 直接赋值 Image 对象
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Info($"加载图片失败：{ex.Message}");
                    return;
                }
            }
            else
            {
                LogUtil.Info("未找到报表中的 Pic_title 对象");
            }
        }/// <summary>
        public void SetChartPic(WpfPlot zg_chart, string pic)
        {
            string tempPath = Path.GetTempFileName();
            zg_chart.Plot.SavePng(tempPath, (int)zg_chart.Width, (int)zg_chart.Height);
            // 从文件读取到内存
            byte[] fileBytes = File.ReadAllBytes(tempPath);
            var memoryStream = new MemoryStream(fileBytes);
            System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);
            PictureObject? picture = pReport?.FindObject(pic) as PictureObject;
            if (picture != null) picture.Image = image;
        }
        private void SetTxt(string key, string value)
        {
            TextObject txt = pReport?.FindObject(key) as TextObject;
            if (txt == null)
                return;
            txt.Text = value;
        }

        /// <summary>
        /// 设置指标文本：超出正常范围时追加 ↑/↓ 箭头并标红
        /// </summary>
        private void SetTxtWithMark(string key, double value, double min, double max, string format)
        {
            TextObject txt = pReport?.FindObject(key) as TextObject;
            if (txt == null)
                return;
            bool high = value > max;
            bool low = value < min;
            string arrow = high ? "↑" : (low ? "↓" : "");
            txt.Text = value.ToString(format) + (arrow.Length > 0 ? " " + arrow : "");
            txt.TextColor = (high || low) ? SDColor.Red : SDColor.Black;
        }
        private void SetPic(string key, string pic)
        {
            PictureObject picture = pReport?.FindObject(key) as PictureObject;
            if (picture != null)
            {
                picture.Image = System.Drawing.Image.FromFile(pic);
            }
        }
        public void InitializeZedGraph(WpfPlot zg_ai)
        {
            #region 心血管
            zg_ai.Plot.Clear();
            var AICurve = zg_ai.Plot.Add.Signal(AIData);
            AICurve.Color = ScottPlot.Colors.Red;
            AICurve.Axes.YAxis = zg_ai.Plot.Axes.Left;
            zg_ai.Plot.Axes.Left.Min = 0;
            zg_ai.Plot.Axes.Left.Max = 4400;
            zg_ai.Plot.Axes.SetLimitsX(0, 6000);
            zg_ai.Plot.Axes.Bottom.IsVisible = false;
            zg_ai.Plot.Axes.Left.IsVisible = false;
            zg_ai.Plot.Axes.Right.IsVisible = false;
            zg_ai.Plot.Axes.Top.IsVisible = false;
            zg_ai.Plot.HideGrid();
            zg_ai.Plot.ShowLegend(Alignment.UpperCenter, Orientation.Horizontal);
            zg_ai.Refresh();
            #endregion
            #region 特征点

            #endregion
        }
    }
}
