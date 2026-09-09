using Cardio.Model;
using Cardio.Util;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

/*
 * 自定义串口管理类，线程安全的单例模式，全局维护和使用
 * 完成时间：2022.11.23
 * 完成人：王友才
*/
namespace Cardio.SPCL
{
    public class SerialPortManager
    {
        ComDataFormat comDataFormat = new ();  //形成发送数据帧的类
        private static SerialPortManager serialPortInstance;//定义单例对象
        private static readonly object lockObj = new object();//定义锁对象
        private SerialPort serialPort = null;//定义串口对象
        private System.Timers.Timer serialPortRecive_timer = new System.Timers.Timer(1000);//定义串口发送接收定时器
        private int reciveCounter = 0;//发送之后等待接收数据计数器
        private int reciveCounterMax = 0;//定时器阈值   
        public static string port = APPSettingsViewModel.getInstance().APP_BPPort;//定义串口
        private int bundRate = 115200;//定义波特率
        private List<byte> myReadBuffer = [];//缓存，用于存放接收到的数据
        public delegate void InformMsg(MCErrorCode code, List<ReceiveDataStructure> dataList);//创建委托 消息通知
        public InformMsg InformMsgEvnet;
        public delegate void InformDebugMsg(string msg);//创建委托 用于调试
        public InformDebugMsg InformDebugMsgEvnet;
        public int SendTimes = 1;
        private readonly int ShortWaitTime = 3;


        private readonly System.Timers.Timer _readTimer = new System.Timers.Timer(28);//下位机采样频率为28ms一次
        private readonly object _readLock = new object();
        //定义构造函数 初始化一些操作
        private SerialPortManager() 
        {
            serialPort = new SerialPort();
            serialPortRecive_timer.Elapsed += new System.Timers.ElapsedEventHandler(ReciveCountAccurate);
            serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort_DataReceived);//绑定串口接收事件
            //_readTimer.AutoReset = true;
            //_readTimer.Elapsed += ReadTimer_Elapsed; // 定时器接收串口数据
            //_readTimer.Start();
        }
        /// <summary>
        /// 定义静态方法获取唯一对象
        /// </summary>
        /// <param></param>
        /// <return></return>
        public static SerialPortManager getInstance() 
        {
            if (serialPortInstance==null)
            {
                lock (lockObj) 
                {
                    if (serialPortInstance == null)
                    {
                        serialPortInstance = new SerialPortManager();
                    }
                }
            }
            port = APPSettingsViewModel.getInstance().APP_BPPort;
            return serialPortInstance;
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param></param>
        /// <return></return>
        public bool OpenSerialPort()
        {
            try
            {
                if (serialPort.IsOpen)
                {}
                else
                {
                    serialPort.PortName = port;
                    serialPort.BaudRate = bundRate;
                    serialPort.ReceivedBytesThreshold = 1;
                    serialPort.Open();
                }
                return true;
            }
            catch(Exception ex)
            {
                LogUtil.Info("serial",ex.Message);
                InformMsgEvnet(MCErrorCode.OpenSerialFail, null);//打开串口失败
                return false;
            }
        }
        /// <summary>
        /// 发送之后 记时
        /// </summary>
        /// <param name="cmd">命令类型</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        private void ReciveCountAccurate(object sender, System.Timers.ElapsedEventArgs e) 
        {
            reciveCounter++;//等待接收计数累加
            if (reciveCounter > reciveCounterMax)//等待超时
            {
                serialPortRecive_timer.Enabled = false;//关闭定时器
                reciveCounter = 0;//计数清零
                InformMsgEvnet(MCErrorCode.TimeLimited, null);
            }
        }
        /// <summary>
        /// 串口发送数据
        /// </summary>
        /// <param name="cmd">命令类型</param>
        /// <param name="data">数据</param>
        /// <param name="time">超时阈值 单位s</param>
        /// <returns></returns>
        public bool SendDataToMCU(byte cmd, List<byte> data, int time)
        {
            bool isSuccess = false;
            byte[] buffer = comDataFormat.DataToSend(cmd, data);//形成需要发送的数据串格式
            InformDebugMsgEvnet("发送数据" + ExtractData.ByteToString(buffer));
            try
            {
                isSuccess = true;  //置位发送成功标记
                bool result = OpenSerialPort();//打开串口
                if (result)
                {
                    serialPort.DiscardInBuffer();  // 清空接收缓存
                    serialPort.Write(buffer, 0, buffer.Length);  //写入数据发送
                    reciveCounterMax = time;//重置阈值
                    reciveCounter = 0;//重置计数器
                    serialPortRecive_timer.Enabled = true;//启动定时器
                }
                else
                {
                    isSuccess= false;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                LogUtil.Error("串口", ex.Message);
                reciveCounter = 0;//重置计数器
                serialPortRecive_timer.Enabled = false;//关闭定时器
                //出现异常需要提示信息
                InformMsgEvnet(MCErrorCode.OpenSerialFail, null);//打开串口失败
            }
            return isSuccess;
        }
        //串口接收数据
        private void ReadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 防止重入
            if (!Monitor.TryEnter(_readLock)) return;
            {
                try
                {
                    // 检查串口是否打开
                    if (!serialPort.IsOpen) return;

                    serialPortRecive_timer.Enabled = false;
                    reciveCounter = 0;
                    // 检查是否有数据可读
                    int byteToRead = serialPort.BytesToRead;
                    if (byteToRead == 0) return;

                    // 限制每次最大读取 2048 字节，防止一次处理过多
                    int bytesToRead = Math.Min(byteToRead, 2048);
                    byte[] bufferTemp = new byte[bytesToRead];
                    int actualRead = serialPort.Read(bufferTemp, 0, bytesToRead);

                    if (actualRead > 0)
                    {
                        // 只有实际读到字节才解除等待，空轮询不能掩盖真实超时。
                        
                        // 添加到缓存
                        myReadBuffer.AddRange(bufferTemp);

                        List<ReceiveDataStructure> dataValueAndTypes = ExtractData.GetUsefullInfos(myReadBuffer);
                        // 触发事件
                        if (dataValueAndTypes.Count > 0)
                        {
                            InformMsgEvnet?.Invoke(MCErrorCode.NoError, dataValueAndTypes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error("定时读取串口异常", ex.Message);
                }
                finally
                {
                    Monitor.Exit(_readLock);
                }
            }
        }

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // 关定时器
            serialPortRecive_timer.Enabled = false;
            reciveCounter = 0;
            //读取数据到自定义的用户缓存中，下面处理对象全是针对用户缓存myReadBuffer
            int byteToRead = serialPort.BytesToRead;
            if (byteToRead==0)//huoqushebeide length ,开始时间和结束时间放宽一点，
            { return; }
            byte[] bufferTemp = new byte[byteToRead];
            serialPort.Read(bufferTemp, 0, byteToRead);

            myReadBuffer.AddRange(bufferTemp);
            if(myReadBuffer.Count > 5)
            {
                //提取数据中的有用信息，按照结构体类型放到dataValueAndType中
                List<ReceiveDataStructure> dataValueAndTypes = ExtractData.GetUsefullInfos(myReadBuffer);

                if (dataValueAndTypes.Count > 0)
                {
                    InformMsgEvnet?.Invoke(MCErrorCode.NoError, dataValueAndTypes);//串口每次触发datareceived事件时裁剪好的数据都会被复制//串口每次触发datareceived事件时裁剪好的数据都会被复制
                } 
            }
        }
        //发送设定预期压力
        public void InitSet(int Wvalue_temp, int whichOne)
        {
            byte Value_temp;
            byte diValue;
            int wave = Convert.ToInt32(Wvalue_temp);
            if (wave > 255)
            {
                Value_temp = 0xff;
                diValue = Convert.ToByte(wave - 255);
            }
            else
            {
                Value_temp = 0x00;
                diValue = Convert.ToByte(wave);
            }
            SendDataToMCU(CommandWord.REQ_BP_INIT_SET, [diValue, Value_temp, (byte)whichOne], ShortWaitTime);
        }
        //可发送指令（REQ_BP_OBJECT_SET,REQ_BP_START,REQ_BP_STOP,REG_BP_GET_RESULT,REQ_PWV_Start)
        public bool SendData(byte cmd, int whichOne) => SendDataToMCU(cmd, [0, (byte)whichOne], ShortWaitTime);
        //控制阀门
        public bool OpenControl(int whichOne) => SendDataToMCU(CommandWord.REQ_BP_CONTROL, [2, (byte)whichOne], ShortWaitTime);
        public bool CloseControl(int whichOne) => SendDataToMCU(CommandWord.REQ_BP_CONTROL, [6, (byte)whichOne], ShortWaitTime);

    }
}
