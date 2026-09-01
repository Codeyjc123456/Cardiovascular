using Cardio.Views;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cardiovascular
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
            CenterWindowOnScreen();
            //ScreenTaskBar.Hide();
            string strProcessName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(strProcessName).Length > 1)
            {
                //ScreenTaskBar.Show();
                var myWindow = System.Windows.Window.GetWindow(this);
                myWindow.Close();
                System.Windows.Application.Current.Shutdown();
            }
            //this.Frame.Navigate(new AdminLoginPage());
        }
        private void CenterWindowOnScreen()
        {
            this.Left = 0;
            this.Top = 0;
        }
        private void LoadDialogAsync(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new AdminLoginPage());
        }
    }
}