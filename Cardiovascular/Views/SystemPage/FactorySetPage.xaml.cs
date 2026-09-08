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
using System.Threading;
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
        private int awaitingPortCheck;
        private readonly APPSettingsViewModel viewModel = APPSettingsViewModel.getInstance();
        public FactorySetPage()
        {
            InitializeComponent();
            DataContext = viewModel;
            Unloaded += (_, _) =>
            {
                Interlocked.Exchange(ref awaitingPortCheck, 0);
                if (serialPortManager != null)
                {
                    serialPortManager.InformMsgEvnet -= ReciveAlarm;
                    serialPortManager.InformDebugMsgEvnet -= msg;
                }
            };
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
            // NoError 是每批解析数据的通知，不是串口连接状态通知。
            if (Volatile.Read(ref awaitingPortCheck) == 0) return;
            string message;
            if (code == MCErrorCode.NoError)
            {
                if (dataList == null || !dataList.Any(x => x.frameType == CommandWord.REQ_BP_CONTROL))
                    return;
                message = "串口识别成功！";
            }
            else if (code == MCErrorCode.OpenSerialFail || code == MCErrorCode.TimeLimited)
            {
                message = "串口识别失败！";
            }
            else return;
            if (Interlocked.Exchange(ref awaitingPortCheck, 0) == 0) return;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsVisible) HandyControl.Controls.Growl.Info(message);
            }));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)
            {
                Interlocked.Exchange(ref awaitingPortCheck, 0);
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
                Interlocked.Exchange(ref awaitingPortCheck, 1);
                bool result = serialPortManager.SendDataToMCU(CommandWord.REQ_BP_CONTROL, SendDataBytes, viewModel.ShortWaitTime);
                if (result)
                {

                }
                else
                {
                    if (Interlocked.Exchange(ref awaitingPortCheck, 0) != 0)
                        HandyControl.Controls.Growl.Info("打开串口失败" );
                }
            }
            else
            {
                HandyControl.Controls.Growl.Info("关闭！"+ COM);
                Interlocked.Exchange(ref awaitingPortCheck, 0);
            }

        }

        private void msg(string m)
        {
            Console.WriteLine(m);
        }


    }
}
