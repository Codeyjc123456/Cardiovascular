using Cardio.Util;
using Cardio.Views.DataManagePage;
using Cardio.Views.DataManagePage.Mc;
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

namespace Cardio.Views
{
    /// <summary>
    /// DataManageWinPage.xaml 的交互逻辑
    /// </summary>
    public partial class DataManageWinPage : Page, INotifyPropertyChanged
    {
        #region 数据绑定
        public event PropertyChangedEventHandler PropertyChanged;
        private string dataListImgSource = "pack://application:,,,/Resources/Image/Datamanage/m_openDataList.png";
        public string DataListImgSource
        {
            get { return dataListImgSource; }
            set
            {
                dataListImgSource = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DataListImgSource"));
            }
        }
        private string uploadDataImgSource = "pack://application:,,,/Resources/Image/datamanage/m_upload.png";
        public string UploadDataImgSource
        {
            get { return uploadDataImgSource; }
            set
            {
                uploadDataImgSource = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("UploadDataImgSource"));
            }
        }
        #endregion
        public DataManageWinPage()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SystemFrame.Content = new Frame() { Content = new DataListPage() };
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DataListImgSource = "pack://application:,,,/Resources/Image/datamanage/ms_openDataList.png";
                UploadDataImgSource = "pack://application:,,,/Resources/Image/datamanage/m_upload.png";
            }));
        }

        private void OpenDataListPageBtn(object sender, RoutedEventArgs e)
        {
            this.SystemFrame.Content = new Frame() { Content = new DataListPage() };
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DataListImgSource = "pack://application:,,,/Resources/Image/datamanage/ms_openDataList.png";
            }));
        }
        private void OpenUploadDataBtn(object sender, RoutedEventArgs e)
        {
            //this.SystemFrame.Content = new Frame() { Content = new UploadDataPage() };
            //DataListImgSource = "pack://application:,,,/Resources/Image/datamanage/m_openDataList.png";
            //UploadDataImgSource = "pack://application:,,,/Resources/Image/datamanage/ms_upload.png";
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
    }
}
