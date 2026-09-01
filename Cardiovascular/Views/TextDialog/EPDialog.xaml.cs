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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cardio.Views.TextDialog
{
    /// <summary>
    /// Dialog.xaml 的交互逻辑
    /// </summary>
    public partial class EPDialog : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private Action<string> tabAction;
        public EPDialog(Action<string> tAction)
        {
            InitializeComponent();
            tabAction = tAction;
        }

        private void CancleButton_Click(object sender, RoutedEventArgs e)
        {
            tabAction.Invoke("取消");
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            tabAction.Invoke("确定");
        }
    }
}
