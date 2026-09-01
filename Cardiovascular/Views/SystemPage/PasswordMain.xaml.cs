using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cardio.Views.SystemPage
{
    /// <summary>
    /// PasswordMain.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordMain : Window
    {
        public string Password { get; private set; }
        public PasswordMain()
        {
            InitializeComponent();
            // 设置窗体的背景为透明色
            //this.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void dialog_Confirmed( object sender, EventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.GoBack();
            PasswordMain password = new PasswordMain();
            Password = passwordBox.Password;
            if (Password == "65301238")
            {
                Window.GetWindow(this).DialogResult = true;
            }
            else
            {
                HandyControl.Controls.Growl.Warning("密码输入不正确！");
            }
                
       

        }

        private void Button_Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
