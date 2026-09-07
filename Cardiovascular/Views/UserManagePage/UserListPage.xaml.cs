using Cardio.DAL;
using Cardio.Util;
using Cardio.Views.MeasurePage;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cardio.Views.UserManagePage
{
    /// <summary>
    /// UserListPage.xaml 的交互逻辑
    /// </summary>
    public partial class UserListPage : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// 数据绑定
        /// </summary>
        #region
        private int pageIndex = 1;
        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                pageIndex = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PageIndex"));
            }
        }

        private int recordTotal = 0;
        public int RecordTotal
        {
            get { return recordTotal; }
            set
            {
                recordTotal = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RecordTotal"));
            }
        }

        private List<UserInfoEntity> dataList;
        public List<UserInfoEntity> DataList
        {
            get { return dataList; }
            set
            {
                dataList = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DataList"));
            }
        }

        private string searchText = "";
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SearchText"));
            }
        }
        private string isRunning = "Hidden";
        public string IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsRunning"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private UserInfoDAL userInfoDAL = null;//定义一个用户信息操作类
        private int size = 10;
        private List<UserInfoEntity> userlist = null;
        private readonly Action tabAction;

        public UserListPage()
        {
            InitializeComponent();
            Unloaded += (_, _) => search.Invalidate();
            DataContext = this;
            tabAction += TabCallBackExcute;
        }

        private void TabCallBackExcute()
        {
            Search();
        }

        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            userInfoDAL = UserInfoDAL.getInstance();
            Search();
        }

        private new void ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private readonly LatestSearch search = new();

        private async void Search()
        {
            int page = PageIndex;
            string text = SearchText;
            var dal = userInfoDAL;
            if (dal == null) return;
            await search.RunAsync(() =>
            {
                int total = 0;
                string condition = string.IsNullOrEmpty(text) ? "" :
                    " userID like '%" + text + "%' or userName like '%" + text + "%'";
                var rows = dal.Finds(page, size, ref total, condition, "id desc");
                return (rows, total);
            }, result =>
            {
                userlist = DataList = result.rows;
                RecordTotal = result.total;
                if (result.rows == null || result.rows.Count == 0)
                    Growl.Info("无任何记录！");
            }, busy => IsRunning = busy ? "Visible" : "Hidden");
        }
        // 页码改变
        private void PageNextUpdated(object sender, RoutedEventArgs e)
        {
            if (recordTotal > size * pageIndex)
            {
                PageIndex++;
                Search();
            }
            else
            {
               Growl.Warning("已经是最后一页！");
            }
        }

        private void PageLastUpdated(object sender, RoutedEventArgs e)
        {
            if (pageIndex > 1)
            {
                PageIndex--;
                Search();
            }
            else
            {
               Growl.Warning("已经是第一页！");
            }
        }

        private void OpenMeasure(object sender, RoutedEventArgs e)
        {
            var id = (sender as System.Windows.Controls.Button).Tag.ToString();
            UserInfoEntity u = userInfoDAL.FindByID(Convert.ToInt32(id));
            if (u != null)
            {
                this.NavigationService.Navigate(new MeasureReePage(u));
            }
        }

        private void SerachBtn(object sender, RoutedEventArgs e)
        {
            PageIndex = 1;
            Search();
        }

        private void OpenDelete(object sender, RoutedEventArgs e)
        {
            var id = (sender as System.Windows.Controls.Button).Tag.ToString();
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show("是否确定继续操作？", "删除提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                bool isSuccess = userInfoDAL.Delete(Convert.ToInt32(id));
                if (isSuccess)
                {

                    Growl.Success("删除用户成功！" );
                    PageIndex = 1;
                    Search();
                }
                else
                {
                   Growl.Warning("删除用户失败！");
                }
            }
            else
            {
               Growl.Info("取消删除操作！");
            }

        }

        private void OpenUpdate(object sender, RoutedEventArgs e)
        {
            var id = (sender as System.Windows.Controls.Button).Tag.ToString();
            UserInfoEntity u = userInfoDAL.FindByID(Convert.ToInt32(id));;
            if (u != null)
            {
                Dialog.Show(new RegisterDialog(u,tabAction)).GetResultAsync<string>();
            }
            else
            {
                Growl.Warning("查无此用户信息！");
            }
        }

        private void BackBtn(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见 
            if (!isVisible)//不可见主动回收资源
            {
                search.Invalidate();
            }
        }

        private async void RigisterBtn(object sender, RoutedEventArgs e)
        {
            UserInfoEntity u = new UserInfoEntity();
            await Dialog.Show(new RegisterDialog(u,tabAction)).GetResultAsync<string>();
        }

        private void BatchRigisterBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                //创建打开文件对话框实例
                System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
                //设置文件过滤器。只显示Excel文件
                open.Filter = "Excel文件（*.xlsx;*.xls)| *.xlsx;*.xls|所有文件(*.*)|*.*";
                open.Title = "选择包含用户信息的Excel文件";
                string filePath = "";//存储选中的文件路径
                //显示对话框并检查用户是否点击了OK按钮
                if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //获取用户选择的文件路径
                    filePath = open.FileName;
                    // 在UI线程上更新运行状态为可见（显示加载指示器）
                    Dispatcher.Invoke(new Action(() =>
                    {
                        IsRunning = "Visible";
                    }));
                    //创建后台任务执行批量注册操作
                    var registrationTask = Task.Run(() =>
                    {
                        try
                        {
                            //从Excel文件中读取用户数据,并获取状态列的位置信息
                            var (users, statusColumnIndex) = ReadUsersFromExcelWithStatus(filePath);
                            int successCount = 0;//成功注册计数
                            int failCount = 0;//失败注册计数
                            //遍历所有读取到的用户，逐个进行注册
                            for (int i = 0; i < users.Count; i++)
                            {
                                UserInfoEntity user = users[i];
                                int excelRow = i + 2;//从Excel中的实际行好（从第2行开始，标题行是第1行）
                                //执行注册逻辑
                                bool registrationResult = RegisterUser(user);
                                // 根据注册结果进行相应处理
                                if (registrationResult)
                                {
                                    UpdateRegistrationStatus(filePath, excelRow, statusColumnIndex, "是");
                                    successCount++;
                                    LogUtil.Info($"用户 {user.UserName} 注册成功");
                                }
                                else
                                {
                                    failCount++;
                                    LogUtil.Info($"用户 {user.UserName} 注册失败");
                                }
                            }
                            Dispatcher.Invoke(new Action(() =>
                            {
                                IsRunning = "hidden";
                                HandyControl.Controls.Growl.Success($"批量注册完成！成功：{successCount}个，失败：{failCount}个,详细注册信息请查看Excel表格中最后一列状态位");
                            }));
                            Search();
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                IsRunning = "Hidden";
                            }));
                            Growl.Info($"批量注册失败：{ex.Message}");
                        }
                    });
                }
            }
            catch (Exception ex)

            {

            }
        }
        private (List<UserInfoEntity>, int statusColumnIndex) ReadUsersFromExcelWithStatus(string filePath)
        {
            List<UserInfoEntity> users = new List<UserInfoEntity>();
            int statusColumnIndex = 0;//状态列的索引
            //使用EPPlus库读取Excel文件(需要安装EPPlus Nuget包）
            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath)))
            {
                //检查工作簿中是否有工作表
                if (package.Workbook.Worksheets.Count == 0)
                {
                    throw new Exception("Excel文件中没有工作表");
                }
                //获取第一个工作表
                var worksheet = package.Workbook.Worksheets.First();
                //获取工作表的行数和列数
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;


                //查找状态列的位置（最后一列）
                statusColumnIndex = worksheet.Dimension.Columns;
                //从第二行开始读取（假设第一行是标题行）
                for (int row = 2; row <= rowCount; row++)
                {
                    // 检查状态列的值，只处理状态位“否”或空的行
                    string status = worksheet.Cells[row, statusColumnIndex].Value?.ToString() ?? "";
                    //如果状态已经是“是”，跳过改行（不注册）
                    if (status == "是")
                    {
                        LogUtil.Info($"跳过第{row}行，用户已经注册成功");
                        continue;
                    }
                    //创建用户对象
                    UserInfoEntity user = new UserInfoEntity();
                    //根据Excel列的位置读取数据
                    //假设Excel列顺序：A列-用户ID，B列-用户名，C列-性别，D列-身高，E列-体重,F列-出生日期
                    user.UserId = worksheet.Cells[row, 1].Value?.ToString() ?? "";
                    user.UserName = worksheet.Cells[row, 2].Value?.ToString() ?? "";
                    user.UserSex = worksheet.Cells[row, 3].Value?.ToString() ?? "";
                    user.UserHeight = Convert.ToDouble(worksheet.Cells[row, 4].Value?.ToString() ?? "");
                    user.UserWeight = Convert.ToDouble(worksheet.Cells[row, 5].Value?.ToString() ?? "");
                    user.UserBirthday = (worksheet.Cells[row, 6].Value?.ToString() ?? "");
                    //跳过空行（如果用户名为空则认为改行无效）
                    if (!string.IsNullOrEmpty(user.UserId))
                    {
                        users.Add(user);
                    }
                }
            }
            return (users, statusColumnIndex);
        }
        private bool RegisterUser(UserInfoEntity user)
        {

            DateTime date = DateTime.Parse(user.UserBirthday);
            user.UserBirthday = date.ToString("d");

            user.CreateTime = DateTime.Now.ToString();
            double[] Temp_Distance = new double[3];
            Temp_Distance[0] = (double)(0.8129 * user.UserHeight + 12.328);
            Temp_Distance[1] = (double)(0.2195 * user.UserHeight - 2.0734);
            Temp_Distance[2] = Temp_Distance[0] - Temp_Distance[1];
            user.UserDistance = (float)Math.Round((decimal)Temp_Distance[2], 1, MidpointRounding.AwayFromZero);
            DateTime bir;
            if (DateTime.TryParse(user.UserBirthday, out bir))
            {
                user.UserAge = DateTime.Now.Year - bir.Year;
                if (DateTime.Now.Month < bir.Month)
                {
                    user.UserAge -= 1;
                }
            }
            if (user.UserAge < 7 || user.UserAge > 100)
            {
                HandyControl.Controls.Growl.Warning("出生日期填写有误，请重新填写！");
                return false;
            }
            if (userLoginNameUsed(user.UserId))
            {
                HandyControl.Controls.Growl.Warning("用户名已被使用，请重新填写！");
                return false;
            }
            if (userInfoDAL.Insert(user) > 0)
            {
                HandyControl.Controls.Growl.Success("用户注册成功！", "Register");
                return true;
            }
            else
            {
                HandyControl.Controls.Growl.Warning("用户注册失败！", "Register");
                return false;
            }
        }
        /// <summary>
        /// 查询用户名是否被用过
        /// </summary>
        /// <param name="userLoginName"></param>
        /// <returns></returns>
        private bool userLoginNameUsed(string userLoginName)
        {
            List<UserInfoEntity> foundUsers = new List<UserInfoEntity>();
            string conditionStr = " userID ='" + userLoginName + "'";
            foundUsers = userInfoDAL.Finds(conditionStr);
            if (foundUsers.Count > 0)//大于1表示
                return true;
            return false;
        }
        /// <summary>
        /// 更新Excel中指定行的注册状态
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <param name="rowNumber">行号</param>
        /// <param name="columnIndex">列索引</param>
        /// <param name="status">状态值（"是"或"否"）</param>
        private void UpdateRegistrationStatus(string filePath, int rowNumber, int columnIndex, string status)
        {
            try
            {
                //使用EPPlus库更新Excel文件
                using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    //更新状态列的值
                    worksheet.Cells[rowNumber, columnIndex].Value = status;
                    //保存Excel文件
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                LogUtil.Info($"更新第{rowNumber}行“状态”失败：{ex.Message}");
            }
        }

    }
}
