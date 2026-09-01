using Cardio.Model;
using Cardio.SPCL;
using Cardio.Util;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cardio.Views.SystemPage
{
    /// <summary>
    /// FactorySetPage.xaml 的交互逻辑
    /// </summary>
    public partial class FactorySetPage : Page
    {
        SerialPortManager serialPortManager = null;
        private readonly APPSettingsViewModel viewModel = APPSettingsViewModel.getInstance();
        public FactorySetPage()
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadPrinter();
        }

        private void LoadPrinter()
        {
            BPCOM.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)//获取所有打印机名称
            {
                BPCOM.Items.Add(port);
            }
            BPCOM.SelectedItem = viewModel.APP_BPPort;
           
            serialPortManager = SerialPortManager.getInstance();//串口业务
            serialPortManager.InformMsgEvnet = new SerialPortManager.InformMsg(ReciveAlarm);
            serialPortManager.InformDebugMsgEvnet = new SerialPortManager.InformDebugMsg(msg);
        }

        private void ReciveAlarm(MCErrorCode code, List<ReceiveDataStructure> dataList)
        {
            if (code == MCErrorCode.NoError)
            {
                HandyControl.Controls.Growl.Info("串口识别成功！");
            }
            else if (code == MCErrorCode.OpenSerialFail)
            {
                HandyControl.Controls.Growl.Info("串口识别失败！");
            }
            else if (code == MCErrorCode.TimeLimited)
            {
                HandyControl.Controls.Growl.Info("串口识别失败！");
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)
            {
                GC.Collect();//回收内存
            }
        }
        private void SaveBtn(object sender, RoutedEventArgs e)
        {

            if (viewModel.RemoveAndAdd != 0)
            {
                HandyControl.Controls.Growl.Warning("信息校验失败，请修改");
                return;
            }

            string COM = BPCOM.Text.ToString();
            if (string.IsNullOrEmpty(COM))
            {
                HandyControl.Controls.Growl.Info("请设置通讯串口！");
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

        private void BPSelect(object sender, RoutedEventArgs e)
        {
            string COM = BPCOM.Text.ToString();
            if (string.IsNullOrEmpty(COM))
            {
                HandyControl.Controls.Growl.Info("请选择串口！");
                return;
            }

            if (viewModel.IsHasBP == "打开")
            {
                HandyControl.Controls.Growl.Info("打开！" + COM);
                List<byte> SendDataBytes = new List<byte>();
                SendDataBytes.Add(0x02);
                SendDataBytes.Add(0x05);
                bool result = serialPortManager.SendDataToMCU(CommandWord.REQ_BP_CONTROL, SendDataBytes, viewModel.ShortWaitTime);
                if (result)
                {

                }
                else
                {
                    HandyControl.Controls.Growl.Info("打开串口失败" );
                }
            }
            else
            {
                HandyControl.Controls.Growl.Info("关闭！"+ COM);
            }

        }

        private void msg(string m)
        {
            Console.WriteLine(m);
        }


    }
}
