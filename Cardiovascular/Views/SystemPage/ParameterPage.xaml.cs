using Cardio.Model;
using Cardio.Util;
using System;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;

namespace Cardio.Views.SystemPage
{
    /// <summary>
    /// FactoryPage.xaml 的交互逻辑
    /// </summary>
    public partial class ParameterPage : Page
    {
        private readonly APPSettingsViewModel viewModel = APPSettingsViewModel.getInstance();
        public ParameterPage()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadPrinter();
        }
        
        private void LoadPrinter()
        {
            PrinterCom.Items.Clear();
            PrinterCom.Items.Add("不选择");
            foreach (string sPrint in PrinterSettings.InstalledPrinters)//获取所有打印机名称
            {
                PrinterCom.Items.Add(sPrint);
            }
        }

        public void SaveBtn(object sender, RoutedEventArgs e)
        {
            if (viewModel.RemoveAndAdd != 0)
            {
                HandyControl.Controls.Growl.Warning("信息校验失败，请修改");
                return;
            }
            if (viewModel.SaveData())
            {
                HandyControl.Controls.Growl.Success("保存成功！");
                viewModel.LoadData();
            }
            else
            {
                HandyControl.Controls.Growl.Info("保存失败！");
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)//不可见主动回收资源
            {
                GC.Collect();
            }
        }
    }
}
