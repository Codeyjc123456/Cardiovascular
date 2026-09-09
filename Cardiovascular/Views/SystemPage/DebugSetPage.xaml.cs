﻿using Cardio.Algorithm;
using Cardio.Model;
using Cardio.SPCL;
using Cardio.Util;
using CardioVascular.Model;
using HandyControl.Controls;
using ScottPlot;
using ScottPlot.WPF;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using Timer = System.Timers.Timer;

namespace CardioVascular.Views.SystemPage
{
    /// <summary>
    /// DebugSetPage.xaml 的交互逻辑
    /// </summary>
    public partial class DebugSetPage : Page
    {
        int PreCount = 0;//连续10个压力值是“0”则重新启动血压测试
        bool PressStart = true;//默认点击后开始测量
        private readonly APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();
        private DebugSetViewModel debugViewModel = new DebugSetViewModel();
        private string LastMsg = "";
        private string? strTip;
        private string? strDebug;
        private WorkStatus workStatus = WorkStatus.NoWork;
        private byte Bt_Pressureindex = 0x01;
        private SerialPortManager? serialPortManager = null;
        private int RpRawDataNum;
        private int SampleNum = 0;
        private List<double> AIData = [];
        private int Bp_ACK_Flag;//标志开关闭阀门
        private Variable.MrsBpValue g_typeBpMrsValue;
        private bool AIFlag;
        private bool SaveDataAfterAIAcquisitionFlag;
        private Timer? TimerGetRealPressure = null;
        private DispatcherTimer? LoopPressureOpen = null;
        private List<double> RpRawData = [];//桡动脉原始数据
        #region 心率监测
        FeatureExtraction? feature = null; 
        double secondsDifference;
        TimeSpan timeDifference;
        private DateTime SampleStartTime;
        double rate;
        #endregion
        public DebugSetPage()
        {
            InitializeComponent();
            this.DataContext = debugViewModel;
        }
        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            serialPortManager = SerialPortManager.getInstance();//串口管理的单例
            serialPortManager.InformMsgEvnet = new SerialPortManager.InformMsg(ReciveAlarm);
            serialPortManager.InformDebugMsgEvnet = new SerialPortManager.InformDebugMsg(msg);

            TimerGetRealPressure = new Timer();
            LoopPressureOpen = new  DispatcherTimer();
            InitTimer();

            debugViewModel.CorrectLBSbp = "1";
            debugViewModel.CorrectLBDbp = "2";

            //serialPortManager.SendData(CommandWord.REQ_PWV_START, 0x01);
            //Task.Delay(500);
            //serialPortManager.SendData(CommandWord.REQ_PWV_STOP, 0x01);
            IntChart();
            feature = new FeatureExtraction();
            APPSettingUtil.LoadData();
            debugViewModel.APP_CorSbp = APPSettingUtil.APP_CorSbp;
            debugViewModel.APP_CorDbp = APPSettingUtil.APP_CorDbp;
            debugViewModel.APP_CorHr = APPSettingUtil.APP_CorHr;
            debugViewModel.APP_CorMap = APPSettingUtil.APP_CorMap;
        }
        private void InitTimer()
        {
            TimerGetRealPressure?.Interval = 200;//1s 执行一次
            TimerGetRealPressure?.Enabled = false;
            TimerGetRealPressure?.AutoReset = true;
            TimerGetRealPressure?.Elapsed += new System.Timers.ElapsedEventHandler(TimerGetRealPressure_Tick);
            LoopPressureOpen?.Interval = TimeSpan.FromMilliseconds(30000);//30s  启动一次
            LoopPressureOpen?.Tick += LoopPressureOpen_Tick;
            LoopPressureOpen?.Start();
            LoopPressureOpen?.Stop();
        }
        private void LoopPressureOpen_Tick(object sender, EventArgs e)
        {
            if (workStatus == WorkStatus.NoWork || workStatus == WorkStatus.StartBPtest || workStatus == WorkStatus.PreStart)
            {
                serialPortManager?.OpenControl(Bt_Pressureindex);
            }
            //打开阀门标志
            Bp_ACK_Flag = -2;
            Task.Delay(2000);
            //在等待2s以后发送关闭阀门
            Dispatcher.BeginInvoke(new Action(() =>
            {
                TimerGetRealPressure?.Stop();//停止读取袖带指令，避免接受不到控制阀门的指令
                PressStart = true;
            }));
            Task.Delay(500);
            for(int i = 0; i < 3; i++)
            {
                serialPortManager?.OpenControl(Bt_Pressureindex);
                workStatus = WorkStatus.CloseValue;//给出工作状态“关闭阀门”下一步就是执行该命令
                Bp_ACK_Flag = -3;
                DisPlayTips("关闭阀门中......");
                Task.Delay(500);
            }
            //关闭阀门以后开始执行压力检测
            TimerGetRealPressure?.Start();
        }
        private void SaveBtn(object sender, RoutedEventArgs e)
        {
            if (APPSettingUtil.RemoveAndAdd != 0)
            {
                Growl.Warning("信息校验失败，请修改");
                return;
            }
            APPSettingUtil.APP_CorSbp = debugViewModel.APP_CorSbp;
            APPSettingUtil.APP_CorDbp = debugViewModel.APP_CorDbp;
            APPSettingUtil.APP_CorMap = debugViewModel.APP_CorMap;
            APPSettingUtil.APP_CorHr = debugViewModel.APP_CorHr;
            if (APPSettingUtil.SaveData())
            {
                Growl.Success("保存成功！");
                APPSettingUtil.LoadData();
                debugViewModel.APP_CorSbp = APPSettingUtil.APP_CorSbp;
                debugViewModel.APP_CorDbp = APPSettingUtil.APP_CorDbp;
                debugViewModel.APP_CorHr = APPSettingUtil.APP_CorHr;
                debugViewModel.APP_CorMap = APPSettingUtil.APP_CorMap;
            }
            else
            {
                Growl.Info("保存失败！");
            }
        }
        private void ClearWave()
        {
                AIData.Clear();
            RpRawDataNum = 0;
            RpRawData.Clear();
            AIFlag = true;
        }
        private void cmdStartPulseMrs_Click(object sender, RoutedEventArgs e)
        {
            
            if (workStatus == WorkStatus.NoWork)
            {
                ClearWave();
                workStatus = WorkStatus.StartPWVtest;
                SampleStartTime = DateTime.Now;
                SampleNum = 0;
                serialPortManager?.SendData(CommandWord.REQ_PWV_START, Bt_Pressureindex);
                ChangeBtStyle(PWVBtn, "BigGYBtnStyle", "测试中...");
            }
            else if (workStatus == WorkStatus.StartPWVtest) //只有当工作状态是空或者开始血压测量时才会发送停止血压测量
            {
                AIFlag = false;
                workStatus = WorkStatus.NoWork;
                if (serialPortManager.SendData(CommandWord.REQ_PWV_STOP, 0x01))
                {
                    Growl.Info("脉搏波测量停止成功。");
                }
                else
                {
                    Growl.Info("脉搏波【停止测量】命令发送失败，请检查设备通信状况！");
                }
                ChangeBtStyle(PWVBtn, "BigBlueBtnStyle", "开始采集");
            }
            else
            {
                Growl.Info("当前有其他任务在执行，请稍后再试！" + "WorkStatus:" + workStatus);
            }
        }
        private void cmdIncGain_Click(object sender, RoutedEventArgs e)
        {
            if (workStatus == WorkStatus.StartPWVtest || workStatus == WorkStatus.NoWork) //只有当工作状态是空或者开始血压测量时才会发送增益信号
            {
                serialPortManager?.SendData(CommandWord.REQ_PWV_INC_GAIN, 0x02);
            }
            else
                Growl.Info("当前有其他任务在执行，请稍后再试！" + "WorkStatus:" + workStatus);
        }
        private void cmdDecGain_Click(object sender, RoutedEventArgs e)
        {
            if (workStatus == WorkStatus.StartPWVtest || workStatus == WorkStatus.NoWork) //只有当工作状态是空或者开始血压测量时才会发送增益信号
            {
                serialPortManager?.SendData(CommandWord.REQ_PWV_DEC_GAIN, 0x02);
            }
            else
                Growl.Info("当前有其他任务在执行，请稍后再试！" + "WorkStatus:" + workStatus);
        }
        private void cmdSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int wave = Convert.ToInt32(debugViewModel.Pressure);
                serialPortManager?.InitSet(CommandWord.REQ_BP_INIT_SET, wave);
            }
            catch (Exception ex)
            {
                LogUtil.Info(ex.Message);
            }
        }
        private void cmdCloseValve_Click(object sender, RoutedEventArgs e)
        {
            if (workStatus == WorkStatus.NoWork || workStatus == WorkStatus.StartBPtest || workStatus == WorkStatus.PreStart || workStatus == WorkStatus.CloseValue)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    TimerGetRealPressure?.Stop();//停止读取袖带指令，避免接受不到控制阀门的指令
                    PressStart = true;
                }));
                ChangeBtStyle(PreStart, "BigBlueBtnStyle", "压力测试");
                Task.Delay(500);
                serialPortManager?.CloseControl(Bt_Pressureindex);
                workStatus = WorkStatus.CloseValue;//给出工作状态“关闭阀门”下一步就是执行该命令             
                Bp_ACK_Flag = -3;//关闭阀门标志
            }
            else
                HandyControl.Controls.Growl.Info("当前有任务正在执行，");
        }

        private void cmdOpenValve_Click(object sender, RoutedEventArgs e)
        {
            if (workStatus == WorkStatus.NoWork || workStatus == WorkStatus.StartBPtest || workStatus == WorkStatus.PreStart || workStatus == WorkStatus.CloseValue)
            {
                serialPortManager?.OpenControl(Bt_Pressureindex);
                //打开阀门标志
                Bp_ACK_Flag = -2;
            }
        }

        private void cmdStartPressMrs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (workStatus == WorkStatus.NoWork)
                {
                    PreCount = 0;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        debugViewModel.SBP = "";//展示收缩压的值
                        debugViewModel.DBP = "";//舒张压
                        debugViewModel.MAP = "";//平均压
                        debugViewModel.BPHr = "";//脉搏
                        debugViewModel.RealPressure = "";//实时压力
                    }));
                    //获取Bt_Pressureindex的指令值
                    serialPortManager?.SendData(CommandWord.REQ_BP_START, Bt_Pressureindex);
                    workStatus = WorkStatus.StartBPtest;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        TimerGetRealPressure?.Start();
                        PressStart = true;
                    }));
                    ChangeBtStyle(BPStart, "BigGYBtnStyle", "执行中...");

                }
                else if (workStatus == WorkStatus.StartBPtest)
                {
                    serialPortManager?.SendData(CommandWord.REQ_BP_STOP, Bt_Pressureindex);
                    workStatus = WorkStatus.NoWork;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        TimerGetRealPressure?.Stop();
                        PressStart = true;
                    }));
                    ChangeBtStyle(BPStart, "BigBlueBtnStyle", "血压测量");
                }
                else if (workStatus == WorkStatus.GetResult)
                {
                    cmdGetPressResult_Click(sender, e);
                }
                else
                    Growl.Info("当前有其他任务正在执行！" + "WorkStatus:" + workStatus);
            }
            catch (Exception ex)
            {
                DisPlayTips(ex.Message);
            }
        }

        private void cmdGetPressResult_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                TimerGetRealPressure?.Stop();
            }));
            serialPortManager?.SendData(CommandWord.REQ_BP_GET_RESULT, Bt_Pressureindex);//向下位机发送读取结果的命令，并返回bool类型的isSuccess
        }

        private void ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible)//不可见主动回收资源
            {
                GC.Collect();
            }
        }
        private void StartTime_Click(object sender, RoutedEventArgs e)
        {
            if (PressStart)
            {
                PressStart = false;
                workStatus = WorkStatus.PreStart;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    TimerGetRealPressure?.Start();
                }));
                ChangeBtStyle(PreStart, "BigGYBtnStyle", "测试中...");
                LoopPressureOpen?.Start();
            }
            else
            {
                workStatus = WorkStatus.NoWork;
                TimerGetRealPressure?.Stop();
                PressStart = true;
                ChangeBtStyle(PreStart, "BigBlueBtnStyle", "压力测试");
                LoopPressureOpen?.Stop();
            }
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            TimerGetRealPressure?.Stop();
        }

        private void TimerGetRealPressure_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            serialPortManager?.SendData(CommandWord.REQ_BP_PRESSURE, Bt_Pressureindex);
        }
        public List<double> XDataList = new List<double>();

        private void IntChart()
        {
            var AICurve = zg_ai.Plot.Add.Signal(AIData);
            AICurve.LegendText = "AI";
            AICurve.Color = Colors.Black;
            AICurve.Axes.YAxis = zg_ai.Plot.Axes.Left;
            zg_ai.Plot.Axes.SetLimitsY(0, 4400);
            zg_ai.Plot.Axes.SetLimitsX(0, 6000);
        }
        private void msg(string m)
        {
            DisPlayTips(m);
        }

        private void ReciveAlarm(MCErrorCode code, List<ReceiveDataStructure> dataList)
        {
            if (code == MCErrorCode.NoError && dataList != null)
            {
                //不同类型的帧分开存储
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (dataList[i].frameType == CommandWord.REQ_PWV_START)
                    {
                        if (dataList[i].dataValue == 1)//桡动脉
                        {
                            for (int k = 0; k < 14; k++)//每次传输14个脉搏波数据
                            {
                                RpRawDataNum++;//计算心率需要用到的变量
                                SampleNum++;
                                RpRawData.Add(dataList[i + 2 + k].dataValue);//把点保存起来采样率减少一半
                                AIData.Add(dataList[i + 2 + k].dataValue);
                            }
                            i += 15;

                            if (AIData.Count % 4 == 0)
                            {
                                zg_ai.Refresh();
                            }
                            if (AIData.Count >= 6000)
                            {
                                AIData.Clear();
                            }
                        }
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_INC_GAIN)
                    {
                        DisPlayTips("温馨提示：脉搏波增益成功");
                        i += 2;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_DEC_GAIN)
                    {
                        DisPlayTips("温馨提示：脉搏波减益成功");
                        i += 2;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_INIT_SET)
                    {
                        if ((dataList[i + 1].dataValue == 0x4B) && (dataList[i + 2].dataValue == 0x73))
                        {
                            strDebug = strDebug + ":" +
                                  dataList[1 + i].dataValue.ToString("X")
                                 + "," +
                                 dataList[2 + i].dataValue.ToString("X")
                                 + "," +
                                 dataList[3 + i].dataValue.ToString("X");
                            DisPlayTips("设定压力成功:" + strDebug);
                        }
                        i += 3;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_OBJECT_SET)
                    {
                        if ((dataList[i + 1].dataValue == 79) && (dataList[i + 2].dataValue == 111))////收到响应指令“O”
                        {
                            strTip = "血压测量开始:";
                            strDebug = strDebug + "O" + ":" + "0X" +
                                dataList[i + 1].dataValue.ToString("X") + "," +
                                dataList[i + 2].dataValue.ToString("X");
                            DisPlayTips(strTip + strDebug);
                            strDebug = "";
                        }
                        else if ((dataList[i + 1].dataValue == 66) && (dataList[i + 2].dataValue == 124))////收到响应指令“B”
                        {
                            strTip = "血压模块忙:";
                            strDebug = strDebug + " B" + ":" + "0X" + dataList[i + 1].dataValue.ToString("X") + "," +
                                 "0X" + dataList[i + 2].dataValue.ToString("X");
                            DisPlayTips(strTip + strDebug);
                            strDebug = "";
                        }
                        else if ((dataList[i + 1].dataValue == 75) && (dataList[i + 2].dataValue == 115))////收到响应指令“K”
                        {
                            strTip = "血压测量完成:";
                            strDebug = strDebug + "K" + ":" + "0X" + dataList[1].dataValue.ToString("X") + "," +
                                 "0X" + dataList[2].dataValue.ToString("X");
                            DisPlayTips(strTip + strDebug);
                            strDebug = "";
                        }
                        i += 3;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_START)
                    {
                        if ((dataList[i + 1].dataValue == 79) && (dataList[i + 2].dataValue == 111))////收到响应指令“O”
                        {
                            strTip = "血压测量开始:";
                            strDebug = strDebug + "O" + ":" + "0X" +
                                dataList[i + 1].dataValue.ToString("X") + "," +
                                dataList[i + 2].dataValue.ToString("X");
                            DisPlayTips(strTip + strDebug);
                            strDebug = "";
                        }
                        else if ((dataList[i + 1].dataValue == 66) && (dataList[i + 2].dataValue == 124))////收到响应指令“B”
                        {
                            
                            strTip = "血压模块忙:";
                            strDebug = strDebug + " B" + ":" + "0X" + dataList[i + 1].dataValue.ToString("X") + "," +
                                 "0X" + dataList[i + 2].dataValue.ToString("X");
                            DisPlayTips(strTip + strDebug);
                            strDebug = "";
                        }
                        else if ((dataList[i + 1].dataValue == 75) && (dataList[i + 2].dataValue == 115))////收到响应指令“K”
                        {
                            strTip = "血压测量完成:";     
                            workStatus = WorkStatus.GetResult;//收到了"K"响应，此时应当发送读取测试压力值
                            strDebug = strDebug + "K" + ":" + "0X" + dataList[1].dataValue.ToString("X") + "," +
                                 "0X" + dataList[2].dataValue.ToString("X");
                            DisPlayTips(strTip + strDebug);
                            strDebug = "";
                        }
                        i += 3;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_STOP)
                    {
                        if ((dataList[1].dataValue == 65) && (dataList[2].dataValue == 125))////收到响应指令“A”
                        {
                            strTip = "";
                            strTip = "温馨提示：血压测量停止！";
                            strDebug = strDebug + "A" + ":" + "0X" + dataList[0].dataValue.ToString("X") + "," +
                                 "0X" + dataList[1].dataValue.ToString("X");
                            DisPlayTips(strTip);
                            strDebug = "";
                            if (workStatus == WorkStatus.CloseValue) //判断如果是关闭阀门，则发送关闭阀门指令
                            {
                                Task.Delay(300);
                                serialPortManager?.CloseControl(Bt_Pressureindex);
                            }
                        }
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_GET_RESULT)
                    {
                        strDebug = strDebug + "k" + ":" +
                               "0X" + dataList[0].dataValue.ToString("X") + "," +
                               "0X" + dataList[1].dataValue.ToString("X") + "," +
                               "0X" + dataList[2].dataValue.ToString("X") + "," +
                               "0X" + dataList[6].dataValue.ToString("X") + "," +
                               "0X" + dataList[10].dataValue.ToString("X");
                        Dispatcher.Invoke(new Action(() =>
                        {
                            debugViewModel.SBP = (dataList[1].dataValue - Convert.ToDouble(APPSettingUtil. APP_CorSbp)).ToString();
                            debugViewModel.DBP = (dataList[2].dataValue - Convert.ToDouble(APPSettingUtil.APP_CorDbp)).ToString();
                            debugViewModel.BPHr = (dataList[5].dataValue-Convert.ToDouble(APPSettingUtil.APP_CorHr)).ToString();
                            debugViewModel.MAP =( dataList[6].dataValue-Convert.ToDouble(APPSettingUtil.APP_CorMap)).ToString();
                        }));
                        g_typeBpMrsValue.Sbp = dataList[1].dataValue;
                        g_typeBpMrsValue.Dbp = dataList[2].dataValue;
                        strTip = "血压读取完成！:";
                        workStatus = WorkStatus.NoWork;
                        DisPlayTips(strTip + strDebug);
                        strDebug = "";
                        i += 12;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_PRESSURE)
                    {
                        Dispatcher.Invoke(new Action(() => {
                            debugViewModel.RealPressure = dataList[i + 1].dataValue.ToString();
                        }));
                        if (dataList[i + 1].dataValue <= 1)
                            PreCount++;
                        else
                            PreCount = 0;
                        if (PreCount >= 5)
                        {
                            ChangeBtStyle(BPStart, "BigBlueBtnStyle", "血压测量");
                            PreCount = 0;
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                TimerGetRealPressure?.Stop();
                                workStatus = WorkStatus.NoWork;
                                DisPlayTips("温馨提示：血压测量完成!");
                            }));
                            strDebug = strDebug + "读取袖带压力，k" + ":" +
                            "0X" + dataList[i + 1].dataValue.ToString("X") + "," +
                            "0X" + dataList[i + 3].dataValue.ToString("X");
                            DisPlayTips("温馨提示：正在读取袖带压力!");
                            strDebug = "";
                        }
                        i += 3;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_CONTROL)
                    {
                        if ((dataList[1].dataValue == 79) && (dataList[2].dataValue == 111))////收到响应指令“O”
                        {
                            if (Bp_ACK_Flag == -3)
                            {
                                strDebug = "";
                                strDebug = strDebug + "o" + ":" +
                                "0X" + dataList[0].dataValue.ToString("X") + "," +
                                "0X" + dataList[1].dataValue.ToString("X") + "," +
                                "0X" + dataList[2].dataValue.ToString("X") + "," +
                                "0X" + dataList[3].dataValue.ToString("X");
                                strTip = "温馨提示：正在关闭阀门.... :";
                                DisPlayTips(strTip + strDebug);

                            }
                            else if (Bp_ACK_Flag == -2)
                            {
                                strDebug = "";
                                strDebug = strDebug + "o" + ":" +
                                "0X" + dataList[0].dataValue.ToString("X") + "," +
                                "0X" + dataList[1].dataValue.ToString("X") + "," +
                                "0X" + dataList[2].dataValue.ToString("X") + "," +
                                "0X" + dataList[3].dataValue.ToString("X");
                                strTip = "温馨提示：正在打开阀门.... :";
                                DisPlayTips(strTip + strDebug);
                            }
                        }
                        else if ((dataList[1].dataValue == 75) && (dataList[2].dataValue == 115))////收到响应指令“K”
                        {
                            if (Bp_ACK_Flag == -3)
                            {
                                strDebug = strDebug + "k" + ":" +
                                "0X" + dataList[0].dataValue.ToString("X") + "," +
                                "0X" + dataList[1].dataValue.ToString("X") + "," +
                                "0X" + dataList[2].dataValue.ToString("X") + "," +
                                "0X" + dataList[3].dataValue.ToString("X");
                                strTip = "温馨提示：阀门关闭！:";
                                DisPlayTips(strTip + strDebug);
                                workStatus = WorkStatus.NoWork;
                            }
                            else if (Bp_ACK_Flag == -2)
                            {
                                strDebug = strDebug + "o" + ":" +
                                "0X" + dataList[0].dataValue.ToString("X") + "," +
                                "0X" + dataList[1].dataValue.ToString("X") + "," +
                                "0X" + dataList[2].dataValue.ToString("X") + "," +
                                "0X" + dataList[3].dataValue.ToString("X");
                                strTip = "温馨提示：阀门开启.... :";
                                workStatus = WorkStatus.NoWork;
                            }
                        }
                        DisPlayTips(strTip + strDebug);
                        i += 3;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_PRESS_CAL)
                    {
                        if ((dataList[1].dataValue == 79) && (dataList[2].dataValue == 111))////收到响应指令“O”
                        {
                            strTip = "温馨提示：正在进行校准....";
                        }
                        else if ((dataList[1].dataValue == 75) && (dataList[2].dataValue == 115))////收到响应指令“K”
                        {
                            strTip = "温馨提示：血压模块校准完成！";
                        }
                        DisPlayTips(strTip);
                        HandyControl.Controls.Growl.Info(strTip);
                        i += 3;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_READ_NO)
                    {
                        if ((dataList[1].dataValue == 79) && (dataList[2].dataValue == 111))////收到响应指令“O”
                        {
                            strTip = "温馨提示：正在进行校准....";
                        }
                        HandyControl.Controls.Growl.Info(strTip);
                        i += 5;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_STOP)
                    {
                        strTip = "脉搏波测量停止成功。";
                        if (AIFlag)
                        {
                            AIFlag = false;
                            SaveDataAfterAIAcquisition();
                        }
                        DisPlayTips(strTip);
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_INC_GAIN)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Growl.Info("温馨提示：脉搏波增益成功");
                        }));
                        DisPlayTips("温馨提示：脉搏波增益成功");
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_DEC_GAIN)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Growl.Info("温馨提示：脉搏波减益成功");
                        }));
                        DisPlayTips("温馨提示：脉搏波减益成功");
                    }
                }
            }
            else if (code == MCErrorCode.OpenSerialFail)
            {
                Growl.Warning("打开串口失败！");
                TimerGetRealPressure?.Stop();

            }
            else if (code == MCErrorCode.TimeLimited)//超时
            {
                Growl.Warning("接收超时！");
            }
        }

        public void DisPlayTips(string text)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                if (LastMsg == text)
                {
                    return;
                }
                TipsStruct tipsStruct = new TipsStruct();
                tipsStruct.Name = text;
                debugViewModel.TipsPPGList.Add(tipsStruct);
                LastMsg = text;
                TipsMsg.Items.Refresh();
                TipsMsg.ScrollIntoView(TipsMsg.Items[debugViewModel.TipsPPGList.Count - 1]);
            }));
        }
        private bool SaveDataAfterAIAcquisition()
        {
            workStatus = WorkStatus.NoWork;
            try
            {
                FeaturePoint featurepoint = new FeaturePoint();
                g_typeBpMrsValue.Sbp = 120;
                g_typeBpMrsValue.Dbp = 70;
                if (featurepoint.Identify(120,70, RpRawData) == 0)//保存数据
                {
                    SaveDataAfterAIAcquisitionFlag = false;
                    return SaveDataAfterAIAcquisitionFlag;
                }
                //心率值是否有误string result = ;//把数字字符串中的数字提取出来
                if (int.Parse(System.Text.RegularExpressions.Regex.Replace(featurepoint.index[1, 0], @"[^0-9]+", "")) < 40)
                {
                    DisPlayTips("温馨提示：波形分析出错，请重新测量");
                    return false;
                }
                debugViewModel.HR = featurepoint.index[1, 0].ToString();
                debugViewModel.ED = Convert.ToSingle(featurepoint.index[1, 1]).ToString("F2");
                debugViewModel.SPTI = featurepoint.index[1, 2].ToString();
                debugViewModel.Dpti = featurepoint.index[1, 3].ToString();
                debugViewModel.Sever = Convert.ToSingle(featurepoint.index[1, 4]).ToString("F3");
                return true;
            }
            catch(Exception ex)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    DisPlayTips("分析桡动脉测量数据失败！" + ex.Message);
                }));
                return false;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //保存压力校准数值
            Growl.Info("保存成功！");
        }
         private void ChangeBtStyle(System.Windows.Controls.Button btn, string styleName, string content)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                var style = Application.Current.FindResource(styleName) as System.Windows.Style;
                btn.Style = style;
                btn.Content = content;
            }));
        }
        private void ProcessRp_Click(object sender, RoutedEventArgs e)
        {
            if (RpRawDataNum < 3 * GlobalVariable.Sample_Rate)
            {
                HandyControl.Controls.Growl.Info("采样数据点较少，无法计算心率值");
                return;
            }
            try
            {
                double HeartRate = feature.ProcessRpRawData(RpRawData, RpRawDataNum);
                
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    debugViewModel.Period = (HeartRate * 2).ToString(); // Assuming txtPeriod is a TextBox control
                    HeartRate = (int)((60.0 * GlobalVariable.Sample_Rate) / (HeartRate * 2));
                    debugViewModel.DebugHR = HeartRate.ToString();
                }));
            }
            catch (Exception ex)
            {

            }
        }
    }
}
