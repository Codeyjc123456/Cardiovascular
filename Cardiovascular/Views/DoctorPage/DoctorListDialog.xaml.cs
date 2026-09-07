using Cardio.Algorithm;
using Cardio.DAL;
using Cardio.Model;
using Cardio.SPCL;
using Cardio.Util;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using System;

using System.Threading;
using System.Windows;

namespace Cardio.Views.DoctorPage
{
    /// <summary>
    /// DoctorListDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DoctorListDialog
    {
        int size = 7;
        int recordTotal = 0;
        DataListViewModel<DoctorInfoEntity> model = new DataListViewModel<DoctorInfoEntity>();
        DoctorInfoEntity entity = new DoctorInfoEntity();
        APPSettingsViewModel APPSettingsViewModel = APPSettingsViewModel.getInstance();
        DoctorInfoDAL doctorDAL = DoctorInfoDAL.getInstance();
        private readonly Action tabAction;

        public DoctorListDialog()
        {
            InitializeComponent();
            Unloaded += (_, _) => search.Invalidate();
            DataContext = model;
            tabAction += TabCallBackExcute;
        }

        private void TabCallBackExcute()
        {
            Search();
        }

        private void RigisterBtn(object sender, RoutedEventArgs e)
        {
            RegitsterDoctor();
        }

        private async void RegitsterDoctor()
        {
            await Dialog.Show(new RegisterDoctor(tabAction)).GetResultAsync<string>();
        }

        private void PageNextUpdated(object sender, RoutedEventArgs e)
        {
            if (model.RecordTotal > size * model.PageIndex)
            {
                model.PageIndex++;
                Search();
            }
            else HandyControl.Controls.Growl.Warning("已经是最后一页！", "DoctorList");
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
                Growl.Warning("已经是第一页！", "DoctorList");
            }
        }

        private readonly LatestSearch search = new();

        private async void Search()
        {
            int page = model.PageIndex;
            await search.RunAsync(() =>
            {
                int total = 0;
                var rows = doctorDAL.Finds(page, size, ref total, "1=1", "id desc");
                return (rows, total);
            }, result =>
            {
                model.DataList = result.rows;
                recordTotal = model.RecordTotal = result.total;
                if (result.rows == null || result.rows.Count == 0)
                    Growl.Info("无任何记录！", "DoctorList");
            }, busy => model.IsRunning = busy ? "Visible" : "Hidden");
        }

        private void BackBt(object sender, RoutedEventArgs e)
        {
            model.CloseAction?.Invoke(); 
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)
                search.Invalidate();
            else
                Search();
        }

        private void OpenDelete(object sender, RoutedEventArgs e)
        {
            var id = (sender as System.Windows.Controls.Button).Tag.ToString();
            string conditionStr = " Id ='" + id + "'";
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show("是否确定继续操作？", "删除提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                bool isSuccess = doctorDAL.Delete(Convert.ToInt32(id));
                GlobalVariable.DoctorPwd = "";
                GlobalVariable.DoctorName = "";
                APPSettingsViewModel.APP_DoctorName = "";
                APPSettingsViewModel.APP_DoctorPWD = "";
                APPSettingsViewModel.SaveData();
                if (isSuccess)
                {
                    HandyControl.Controls.Growl.Success("删除用户成功！", "DoctorList");
                    model.PageIndex = 1;
                    Search();
                }
                else HandyControl.Controls.Growl.Warning("删除用户失败！", "DoctorList");
            }
            else HandyControl.Controls.Growl.Info("取消删除操作！", "DoctorList");
        }

        private void Default(object sender, RoutedEventArgs e)
        {
            var id = (sender as System.Windows.Controls.Button).Tag.ToString();
            string conditionStr = " Id ='" + id + "'";
            entity = doctorDAL.Find(conditionStr);
            if (entity != null)
            {
                GlobalVariable.DoctorPwd = entity.DoctorPwd;
                GlobalVariable.DoctorName = entity.DoctorName;
                APPSettingsViewModel.APP_DoctorName = entity.DoctorName;
                APPSettingsViewModel.APP_DoctorPWD = entity.DoctorPwd;
                APPSettingsViewModel.SaveData();
                Growl.Success("设置默认医师成功！", "DoctorList");
                return;
            }
            Growl.Info("设置默认医师失败！", "DoctorList");
        }
    }
}
