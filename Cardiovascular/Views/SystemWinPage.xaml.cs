using Cardio.Views.SystemPage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using HandyControl.Controls;
using Cardio.Util;
using Cardio.Model;
using CardioVascular.Views.SystemPage;

namespace Cardio.Views
{
    public partial class SystemWinPage : Page
    {
        private readonly APPSettingsViewModel viewModel = APPSettingsViewModel.getInstance();
        public SystemWinPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            OpenParamterPageBtn(null, null);
        }

        private void OpenParamterPageBtn(object sender, MouseButtonEventArgs e)
        {
            this.SystemFrame.Content = new Frame() { Content = new ParameterPage() };
        }

        private void OpenDebugPageBtn(object sender, MouseButtonEventArgs e)
        {
            if (viewModel.APP_PWD != "")
            {
                string userInput = ShowInputBox("请输入密码：", "");
                if (userInput != viewModel.APP_PWD)
                {
                    Growl.Info("密码不正确！");
                    e.Handled = true;
                    return;
                }
            }
            this.SystemFrame.Content = new Frame() { Content = new DebugSetPage() };
           // this.NavigationService.Navigate(new DebugPage());
        }

        private void OpenFactorysetPageBtn(object sender, MouseButtonEventArgs e)
        {
            if (viewModel.APP_PWD != "")
            {
                string userInput = ShowInputBox("请输入密码：", "");
                if (userInput != viewModel.APP_PWD)
                {
                    HandyControl.Controls.Growl.Info("密码不正确！");
                    e.Handled = true;
                    return;
                }
            }
            this.SystemFrame.Content = new Frame() { Content = new FactorySetPage() };
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
                GC.Collect();
            }
        }

        public static string ShowInputBox(string prompt, string title)
        {
            InputBoxWindow inputBox = new InputBoxWindow(prompt);
            inputBox.Title = title;

            bool? result = inputBox.ShowDialog();
            if (result == true)
            {
                return inputBox.UserInput;
            }
            else
            {
                if (inputBox.UserInput == "")//如果输入密码为空。，则不弹窗
                {
                    return "1";
                }
                else
                    return string.Empty;
            }
        }
    }
}
