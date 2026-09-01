using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 接收数据，并存储为一定格式，便于后续根据相应的格式做相应的处理
 * 根据通信协议，随时更新
 * 完成时间：2016.08.10 10：56
 * 更新时间：2016.08.10 16：54
 * 完成人：汪锡  ahuwx@mail.ustc.edu.cn
 */

namespace Cardio.SPCL
{
    /// <summary>
    /// 接收数据并提取有用的信息，存储格式为ReceiveDataStructure
    /// </summary>
    public class DataReceive
    {
        /// <summary>
        /// 接收数据的起始位
        /// </summary>
        public byte startBitDataReceive = 0xEE;
        public byte HKstartBitDataReceive = 0xF0;
        /// <summary>
        /// 接收数据的停止位
        /// </summary>
        public byte endBitDataReceive = 0xFF;

        /// <summary>
        /// 命令字类的对象
        /// </summary>
        private CommandWord CommandWord = new CommandWord();

        /// <summary>
        /// CRC校验的对象
        /// </summary>
        private CRCcheck crcCheck = new CRCcheck();


        //ceshi
        public delegate void FunShow(List<byte> data);
        public FunShow FunShowEvent;
        
        /// <summary>
        /// 寻找收到的数据中所有的起始位索引号
        /// </summary>
        /// <param name="receivedData">接收到的数据数组</param>
        /// <returns>返回起始位索引号组成的数组</returns>
        private List<int> FindStartbitIndex(List<byte> receivedData)
        {
            List<int> indexNum = new List<int>();  //存储索引号的数组
            if (receivedData.Count > 4)  //长度不小于5找起始位才有意义
            {
                //遍历倒数第五位之前的所有位置（后面如果不足四位，即使有起始位也不是完整一帧数据，所以不考虑）
                for (int i = 0; i < receivedData.Count - 4; i++)
                {
                    if (receivedData[i] == startBitDataReceive)   //某一位等于起始位
                    {
                        //if(receivedData[i+1]==0x3F && receivedData[i+2] == 0x20)
                        indexNum.Add(i);  //是起始位则加入到起始位索引号数组中
                    }
                }
            }
            return indexNum;
        }
        /// <summary>
        /// 寻找收到的华科模块数据中所有的起始位索引号
        /// </summary>
        /// <param name="receivedData">接收到的数据数组</param>
        /// <returns>返回起始位索引号组成的数组</returns>
        private List<int> FindHKStartbitIndex(List<byte> receivedData)
        {
            List<int> indexNum = new List<int>();  //存储索引号的数组
            if (receivedData.Count > 2)  //长度不小于3找起始位才有意义
            {
                //遍历倒数第五位之前的所有位置（后面如果不足四位，即使有起始位也不是完整一帧数据，所以不考虑）
                for (int i = 0; i < receivedData.Count - 2; i++)
                {
                    if (receivedData[i] == HKstartBitDataReceive)   //某一位等于起始位
                    {
                        //针对HK模块特殊的通信模式 ：如采集信号中包含FO 31/33/34 提高数据分割效率
                        if (receivedData[i + 1] == 0x31)
                        {
                            indexNum.Add(i);
                        }
                        else if (receivedData[i + 1] == 0x03 || receivedData[i + 1] == 0x09)
                            indexNum.Add(i);  //是起始位则加入到起始位索引号数组中
                        else if (receivedData[i + 1] == 0x33 || receivedData[i + 1] == 0x34)
                        {
                            indexNum.Add(i);
                        }
                    }
                }
            }
            return indexNum;
        }
        /// <summary>
        /// 寻找收到的数据中所有的结束位索引号
        /// </summary>
        /// <param name="receivedData">接收到的数据数组</param>
        /// <returns>返回结束位索引号组成的数组</returns>
        private List<int> FindEndbitIndex(List<byte> receivedData)
        {
            List<int> indexNum = new List<int>();
            if (receivedData.Count > 4)
            {
                for (int i = 0; i < receivedData.Count - 1; i++)  // i < receivedData.Count - 1  保留一位用以CRC位
                {
                    if (receivedData[i] == endBitDataReceive && receivedData[i + 1] == endBitDataReceive)
                    {
                        indexNum.Add(i);
                    }
                }
            }
            return indexNum;
        }




        /// <summary>
        /// 提取一帧数据中的数据和类型，返回到自定义的数据结构数组中
        /// </summary>
        /// <param name="dataTemp">去掉CRC位的一帧数据</param>
        /// <returns></returns>
        private List<ReceiveDataStructure> ExtractToDataStructure(List<byte> dataTemp)
        {
            if (dataTemp.Count == 0)
            {
                return null;
            }
            List<ReceiveDataStructure> usefullInfo = new List<ReceiveDataStructure>();
            ReceiveDataStructure data = new ReceiveDataStructure();
            data.frameType = dataTemp[2];
            if (dataTemp[2] == CommandWord.REQ_GET_SYS_STA)//3
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.DataStatus;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];
                data.dataType = CommandWord.DataStatus;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4] * 256 + dataTemp[5];
                data.dataType = CommandWord.DatawErrors;
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_BP_INIT_SET)//4
            {
                //0x01
                data.dataValue = dataTemp[2];//1
                data.dataType = CommandWord.Data_BP_INIT_SET;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];//2
                data.dataType = CommandWord.Data_BP_INIT_SET;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4];//3
                data.dataType = CommandWord.Data_BP_INIT_SET;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[5];//4
                data.dataType = CommandWord.Data_BP_INIT_SET;
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_BP_OBJECT_SET)//5
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.Data_BP_OBJECT_SET;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];
                data.dataType = CommandWord.Data_BP_OBJECT_SET;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4];
                data.dataType = CommandWord.Data_BP_OBJECT_SET;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[5];
                data.dataType = CommandWord.Data_BP_OBJECT_SET;
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_BP_START)//6[0]
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.Data_BP_START;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];//0x4f[1]
                data.dataType = CommandWord.Data_BP_START;//0x06[2]
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4];//[1]
                data.dataType = CommandWord.Data_BP_START;//[2]
                usefullInfo.Add(data);
                data.dataValue = dataTemp[5];
                data.dataType = CommandWord.Data_BP_START;
                usefullInfo.Add(data);
                //data.dataValue = dataTemp[6];
                //data.dataType = CommandWord.Data_BP_START;
                //usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_BP_STOP)//7
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.Data_BP_STOP;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];
                data.dataType = CommandWord.Data_BP_STOP;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4];
                data.dataType = CommandWord.Data_BP_STOP;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[5];
                data.dataType = CommandWord.Data_BP_STOP;
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_BP_GET_RESULT)//8
            {
                try//如果读取数据出现了问题则直接给对应的值赋一个固定的值
                {
                    //dataTemp.Clear();
                    data.dataValue = dataTemp[2];//0
                    data.dataType = CommandWord.DataStatus;
                    usefullInfo.Add(data);
                    //sys 0
                    data.dataValue = dataTemp[4] * 256 + dataTemp[3];//1
                    data.dataType = CommandWord.Data_BP_sys;
                    usefullInfo.Add(data);
                    //dia 1
                    data.dataValue = dataTemp[6] * 256 + dataTemp[5];//2
                    data.dataType = CommandWord.Data_BP_dia;
                    usefullInfo.Add(data);
                    //BTS 2-3
                    data.dataValue = dataTemp[7];
                    data.dataType = CommandWord.Data_BP_BTC;
                    usefullInfo.Add(data);
                    data.dataValue = dataTemp[8];
                    data.dataType = CommandWord.Data_BP_BPS;
                    usefullInfo.Add(data);
                    //Rate 4    (bP-Hr)
                    data.dataValue = dataTemp[18] * 256 + dataTemp[17];
                    data.dataType = CommandWord.Data_BP_RATE;
                    usefullInfo.Add(data);
                    //Map 5
                    data.dataValue = dataTemp[20] * 256 + dataTemp[19];
                    data.dataType = CommandWord.Data_BP_MAP;
                    usefullInfo.Add(data);
                    //EC 6 
                    data.dataValue = dataTemp[21];
                    data.dataType = CommandWord.Data_BP_EC;
                    usefullInfo.Add(data);
                    //SP1 7 
                    data.dataValue = dataTemp[22];
                    data.dataType = CommandWord.Data_BP_SP1;
                    usefullInfo.Add(data);
                    //SP2 8
                    data.dataValue = dataTemp[23];
                    data.dataType = CommandWord.Data_BP_SP2;
                    usefullInfo.Add(data);
                    //CKSUM 9
                    data.dataValue = dataTemp[24];
                    data.dataType = CommandWord.Data_BP_CKSUM;
                    usefullInfo.Add(data);
                    //BPX  10
                    data.dataValue = dataTemp[25];
                    data.dataType = CommandWord.Data_BP_BPX;
                    usefullInfo.Add(data);
                }
                catch
                {
                    return usefullInfo;
                }
            }
            else if (dataTemp[2] == CommandWord.REQ_BP_PRESSURE)//9
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.DataStatus;
                usefullInfo.Add(data);
                //袖带压力
                //if(dataTemp[3]>0)
                //data.dataValue = dataTemp[3]*256 + dataTemp[4];
                data.dataValue = dataTemp[3]  + dataTemp[4] *256;
                data.dataType = CommandWord.Data_BP_prs;
                usefullInfo.Add(data);

                data.dataValue = dataTemp[5];
                data.dataType = CommandWord.Data_BP_CKSUM;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[6];//bpx
                data.dataType = CommandWord.Data_BP_BPX;//
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_BP_CONTROL)//10
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.Data_BP_CONTROL;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];
                data.dataType = CommandWord.Data_BP_CONTROL;//
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4];
                data.dataType = CommandWord.Data_BP_CONTROL;//
                usefullInfo.Add(data);
                data.dataValue = dataTemp[5];
                data.dataType = CommandWord.Data_BP_CONTROL;//bpx
                usefullInfo.Add(data);

            }
            else if (dataTemp[2] == CommandWord.REQ_BP_PRESS_CAL)//11
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.Data_BP_PRESS_CAL;
                usefullInfo.Add(data);
                data.dataType = CommandWord.Data_BP_PRESS_CAL;
                data.dataValue = dataTemp[3];
                usefullInfo.Add(data);

                data.dataType = CommandWord.Data_BP_PRESS_CAL;
                data.dataValue = dataTemp[4];
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_BP_READ_NO)//12
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.Data_BP_READ_NO;
                usefullInfo.Add(data);
                data.dataType = CommandWord.Data_BP_READ_NO;
                data.dataValue = dataTemp[3];
                usefullInfo.Add(data);

                data.dataType = CommandWord.Data_BP_READ_NO;
                data.dataValue = dataTemp[4];
                usefullInfo.Add(data);

                data.dataType = CommandWord.Data_BP_READ_NO;
                data.dataValue = dataTemp[5];
                usefullInfo.Add(data);

                data.dataType = CommandWord.Data_BP_CKSUM;
                data.dataValue = dataTemp[6];
                usefullInfo.Add(data);

                data.dataType = CommandWord.Data_BP_BPX;
                data.dataValue = dataTemp[7];
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_PWV_START)//13
            {
                data.dataValue = dataTemp[3];
                data.dataType = CommandWord.Data_PWV_Pulse;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4] * 256;
                data.dataType = CommandWord.Data_PWV_Pulse;
                usefullInfo.Add(data);
                //脉搏波数据
                if (dataTemp[3] == 0)
                {
                    //还是三十个就是取的位数提前了一位
                    for (int i = 0; i <= 6; i++)
                    {
                        data.dataValue = dataTemp[i * 8 + 5] * 256 + dataTemp[i * 8 + 6];
                        data.dataType = CommandWord.Data_PWV_Pulse;
                        usefullInfo.Add(data);

                        data.dataValue = dataTemp[i * 8 + 7] * 256 + dataTemp[i * 8 + 8];
                        data.dataType = CommandWord.Data_PWV_Pulse;
                        usefullInfo.Add(data);

                        data.dataValue = dataTemp[i * 8 + 9] * 256 + dataTemp[i * 8 + 10];
                        data.dataType = CommandWord.Data_PWV_Pulse;
                        usefullInfo.Add(data);

                        data.dataValue = dataTemp[i * 8 + 11] * 256 + dataTemp[i * 8 + 12];
                        data.dataType = CommandWord.Data_PWV_Pulse;
                        usefullInfo.Add(data);
                    }
                }
                else if(dataTemp[3]==1)//桡动脉数据
                {
                    //同上
                    for (int i = 0; i <= 13; i++)
                    {
                        data.dataValue = dataTemp[i * 4 + 5]*256  + dataTemp[i * 4 + 6] 　;
                        data.dataType = CommandWord.Data_Radial_Pulse;
                        usefullInfo.Add(data);
                    }
                }
            }
            else if (dataTemp[2] == CommandWord.REQ_PWV_STOP)//14
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.OnlyCmd;
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_PWV_INC_GAIN)//15
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.Data_PWV_Position;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];
                data.dataType = CommandWord.Data_PWV_INC_GAIN;
                usefullInfo.Add(data);

                data.dataValue = dataTemp[4];
                data.dataType = CommandWord.Data_PWV_Position;
                usefullInfo.Add(data);
            }
            else if (dataTemp[2] == CommandWord.REQ_PWV_DEC_GAIN)//16
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.Data_PWV_DEC_GAIN;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];
                data.dataType = CommandWord.Data_PWV_DEC_GAIN;
                usefullInfo.Add(data);

                data.dataValue = dataTemp[4];
                data.dataType = CommandWord.Data_PWV_Position;
                usefullInfo.Add(data);
            }
            else if (dataTemp[1] == 0x3F && dataTemp[2] == 0x20)
            {
                data.dataValue = dataTemp[3] * 256 * 256 + dataTemp[4] * 256 + dataTemp[5];
                data.dataType = CommandWord.Data_ECG;
                usefullInfo.Add(data);

                data.dataValue = dataTemp[6] * 256 * 256 + dataTemp[7] * 256 + dataTemp[8];
                data.dataType = CommandWord.Data_ECG;
                usefullInfo.Add(data);
            }
            return usefullInfo;
        }
        /// <summary>
        /// 提取一帧数据中的数据和类型，返回到自定义的数据结构数组中
        /// </summary>
        /// <param name="dataTemp">去掉CRC位的一帧数据</param>
        /// <returns></returns>
        private List<ReceiveDataStructure> HKExtractToDataStructure(List<byte> dataTemp)
        {
            if (dataTemp.Count == 0)
            {
                return null;
            }
            List<ReceiveDataStructure> usefullInfo = new List<ReceiveDataStructure>();
            ReceiveDataStructure data = new ReceiveDataStructure();
            data.frameType = dataTemp[0];///来自华科模块数据
            if (dataTemp[1] == CommandWord.HKGetDeviceNum)//0x31  获取设备编号
            {
                data.dataValue = dataTemp[2];
                data.dataType = CommandWord.HKGetDeviceNum;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[3];
                data.dataType = CommandWord.HKGetDeviceNum;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4];
                data.dataType = CommandWord.HKGetDeviceNum;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[5];
                data.dataType = CommandWord.HKGetDeviceNum;
                usefullInfo.Add(data);
            }
            else if (dataTemp[1] == CommandWord.Data_HKSample)//0x32 数据采集
            {
                data.dataValue = dataTemp[2] * 256 + dataTemp[3];
                data.dataType = CommandWord.Data_HKXinyin;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[4] * 256 + dataTemp[5];
                data.dataType = CommandWord.Data_HKJD;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[6] * 256 + dataTemp[7];
                data.dataType = CommandWord.Data_HKGD;
                usefullInfo.Add(data);
                data.dataValue = dataTemp[8] * 256 + dataTemp[9];
                data.dataType = CommandWord.Data_HKECG;
                usefullInfo.Add(data);
            }
            else if (dataTemp[1] == CommandWord.Data_HKXinyin)//0x32 数据采集  小包
            {
                data.dataValue = dataTemp[2] * 256 + dataTemp[3];
                data.dataType = CommandWord.Data_HKXinyin;
                usefullInfo.Add(data);
            }
            else if (dataTemp[1] == CommandWord.HKEndTest)//0x33 停止擦剂
            {
                data.dataValue = 0x33;
                data.dataType = CommandWord.HKEndTest;
                usefullInfo.Add(data);
            }
            else if (dataTemp[1] == CommandWord.HKSet)//0x34 配置模块放大系数
            {
                data.dataValue = dataTemp[1];
                data.dataType = CommandWord.HKSet;
                usefullInfo.Add(data);
            }
            return usefullInfo;
        }

        /// <summary>
        /// 读取用户缓存中完整的帧信息，返回ReceiveDataStructure格式的数据，最后清除用户缓存中已经处理过的数据
        /// </summary>
        /// <param name="readBuffer"></param>
        /// <returns></returns>
        public List<ReceiveDataStructure> GetUsefullInfo(List<byte> readBuffer)
        {
            //readBuffer.Count
            List<ReceiveDataStructure> dataValeAndType = new List<ReceiveDataStructure>();
            if (readBuffer.Count >= 5)  //缓存长度大于5开始判断
            {
                //找到缓存的包头和包尾
                List<int> startBitIndex = FindStartbitIndex(readBuffer);
                List<int> endBitIndex = FindEndbitIndex(readBuffer);
                if (startBitIndex.Count >= 1 && endBitIndex.Count >= 1)  //数据中包含起始位和结束位
                {
                    int numRemove = 0;
                    for (int i = 0; i < endBitIndex.Count; i++)    //有几帧数据就读取几帧出来
                    {
                        for (int j = 0; j < startBitIndex.Count; j++)     //找一帧数据出来
                        {
                            if (startBitIndex[j] + readBuffer[startBitIndex[j] + 1] == endBitIndex[i] + 2
                                    && readBuffer[startBitIndex[j] + 1] > 4)  //起始位加上数据长度 等于结束位的索引的值（第一个ff的索引）加2  确保数据的长度是对的并且数据帧不少于4，
                            {
                                //
                                List<byte> dataTempNoCRC = new List<byte>();
                                for (int k = startBitIndex[j]; k <= endBitIndex[i]; k++)
                                {
                                    //把截取好的需要的数据添加到dataTempNoCRC中去
                                    dataTempNoCRC.Add(readBuffer[k]);
                                }
                                //提取数据添加到dataValeAndType中
                                dataValeAndType.AddRange(ExtractToDataStructure(dataTempNoCRC));
                                //移除已经处理的部分
                                numRemove = endBitIndex[i] + 2;
                                
                                break;
                            }
                        }
                    }
                    if (numRemove > 0)
                    {
                        readBuffer.RemoveRange(0, numRemove);   //移除用户缓存中已经处理过的数据
                    }
                }
            }
            return dataValeAndType;
        }
        /// <summary>
        /// 读取华科模块缓存中完整的帧信息，返回ReceiveDataStructure格式的数据，最后清除用户缓存中已经处理过的数据
        /// </summary>
        /// <param name="readBuffer"></param>
        /// <returns></returns>
        public List<ReceiveDataStructure> GetHKInfo(List<byte> readBuffer)
        {
            List<ReceiveDataStructure> dataValeAndType = new List<ReceiveDataStructure>();
            if (readBuffer.Count >= 3)  //缓存长度大于5开始判断
            {
                List<int> startBitIndex = FindHKStartbitIndex(readBuffer);
                if (startBitIndex.Count >= 1)  //数据中包含起始位和结束位  
                {
                    int numRemove = 0;
                    int readRufferIndex = 0;
                    for (int j = 0; j < startBitIndex.Count; j++)     //找一帧数据出来
                    {
                        //取出数据计算校验位
                        List<byte> dataTempNoCRC = new List<byte>();
                        int dataLen;
                        //针对HK模块通信协议非标准的情况  分别处理
                        readRufferIndex = startBitIndex[j] + 1;
                        if ((readBuffer[readRufferIndex] == 0x03) || (readBuffer[readRufferIndex] == 0x09))
                        {
                            if ((startBitIndex[j] + readBuffer[readRufferIndex]) <= (readBuffer.Count - 2) == true)
                            {
                                dataTempNoCRC.Clear();
                                for (int k = 0; k <= readBuffer[startBitIndex[j] + 1]; k++)
                                {
                                    dataTempNoCRC.Add(readBuffer[k + startBitIndex[j]]);
                                }
                                dataLen = dataTempNoCRC.Count;
                                //提取数据添加到dataValeAndType中
                                //如果检验位正确k
                                if (readBuffer[startBitIndex[j] + dataLen] == crcCheck.CalcCKSUM(dataTempNoCRC.ToArray(), (ushort)dataLen))
                                {
                                    //recieveNum++;   //测试
                                    //提取数据添加到dataValeAndType中
                                    dataValeAndType.AddRange(HKExtractToDataStructure(dataTempNoCRC));
                                    //移除已经处理的部分(数据长度）
                                    numRemove = startBitIndex[j] + readBuffer[readRufferIndex] + 2;
                                }
                            }
                        }
                        else if (readBuffer[readRufferIndex] == 0x31)//读设备序列号
                        {
                            if ((startBitIndex[j]) <= (readBuffer.Count - 7))
                            {
                                dataTempNoCRC.Clear();
                                for (int p = 0; p <= 5; p++)
                                {
                                    dataTempNoCRC.Add(readBuffer[startBitIndex[j] + p]);
                                }
                                dataLen = dataTempNoCRC.Count;
                                if (readBuffer[startBitIndex[j] + dataLen] == crcCheck.CalcCKSUM(dataTempNoCRC.ToArray(), (ushort)dataLen))
                                {
                                    //提取数据添加到dataValeAndType中
                                    dataValeAndType.AddRange(HKExtractToDataStructure(dataTempNoCRC));
                                    //移除已经处理的部分(数据长度）
                                    numRemove = startBitIndex[j] + 7;
                                }
                            }
                        }
                        else if ((readBuffer[readRufferIndex] == 0x33) || (readBuffer[readRufferIndex] == 0x34))//关闭自动采样\设置放大倍数
                        {
                            if ((startBitIndex[j]) <= (readBuffer.Count - 3))
                            {
                                dataTempNoCRC.Clear();
                                dataTempNoCRC.Add(readBuffer[startBitIndex[j]]);
                                dataTempNoCRC.Add(readBuffer[readRufferIndex]);
                                dataLen = dataTempNoCRC.Count;
                                if (readBuffer[startBitIndex[j] + dataLen] == crcCheck.CalcCKSUM(dataTempNoCRC.ToArray(), (ushort)dataLen))
                                {
                                    //提取数据添加到dataValeAndType中
                                    dataValeAndType.AddRange(HKExtractToDataStructure(dataTempNoCRC));
                                    //移除已经处理的部分(数据长度）
                                    numRemove = startBitIndex[j] + 3;
                                }
                            }
                        }
                    }
                    if (numRemove > 0)
                    {
                        readBuffer.RemoveRange(0, numRemove);   //移除用户缓存中已经处理过的数据
                    }
                }
            }
            return dataValeAndType;
        }
        /// <summary>
        /// 将不同类型的数据帧分开
        /// </summary>
        /// <param name="dataValueAndTypeAll"></param>
        /// <param name="dataValueAndType1"></param>
        /// <param name="dataValueAndType2"></param>
        /// <param name="dataValueAndType3"></param>
        public void GetEachFrameClass(List<ReceiveDataStructure> dataValueAndTypeAll, ref List<ReceiveDataStructure> dataValueAndType1,
            ref List<ReceiveDataStructure> dataValueAndType2, ref List<ReceiveDataStructure> dataValueAndType3)
        {
            dataValueAndType1.Add(dataValueAndTypeAll[0]);   //添加第一个数据
            int classNum = 1;
            byte frameDataTypeLast = dataValueAndTypeAll[0].frameType;
            byte frameDataType1 = dataValueAndTypeAll[0].frameType;
            byte frameDataType2 = 0x00;
            byte frameDataType3 = 0x00;
            if (dataValueAndTypeAll.Count > 1)
            {
                for (int i = 1; i < dataValueAndTypeAll.Count; i++)
                {
                    if (dataValueAndTypeAll[i].frameType != frameDataTypeLast)
                    {
                        if (frameDataTypeLast == frameDataType1)
                        {
                            if (dataValueAndType2.Count == 0)   //第一次遇到第二种类型的帧
                            {
                                classNum = 2;
                                frameDataTypeLast = dataValueAndTypeAll[i].frameType;
                                frameDataType2 = dataValueAndTypeAll[i].frameType;
                            }
                            else if (dataValueAndType2.Count != 0)   //第二种类型已经存在
                            {
                                if (dataValueAndTypeAll[i].frameType == frameDataType2)    //这次的是第二种类型
                                {
                                    classNum = 2;
                                    frameDataTypeLast = dataValueAndTypeAll[i].frameType;
                                }
                                else if (dataValueAndTypeAll[i].frameType != frameDataType2)  //这次的不是第二种类型
                                {
                                    if (dataValueAndType3.Count == 0)   //第一次遇到第三种类型
                                    {
                                        classNum = 3;
                                        frameDataTypeLast = dataValueAndTypeAll[i].frameType;
                                        frameDataType3 = dataValueAndTypeAll[i].frameType;
                                    }
                                    else if (dataValueAndType3.Count != 0)  //不是第一次遇到第三种类型
                                    {
                                        if (dataValueAndTypeAll[i].frameType == frameDataType3)    //这次的是第三种类型
                                        {
                                            classNum = 3;
                                            frameDataTypeLast = dataValueAndTypeAll[i].frameType;
                                        }
                                        else
                                        {
                                            //MessageBox.Show("有其他情况1！");
                                        }
                                    }
                                }

                            }
                        }
                        else if (frameDataTypeLast == frameDataType2)
                        {
                            if (dataValueAndTypeAll[i].frameType == frameDataType1)   //这次是第一种
                            {
                                classNum = 1;
                                frameDataTypeLast = frameDataType1;
                            }
                            else
                            {
                                if (dataValueAndType3.Count == 0)  //第一次遇到第三种类型
                                {
                                    classNum = 3;
                                    frameDataTypeLast = dataValueAndTypeAll[i].frameType;
                                    frameDataType3 = dataValueAndTypeAll[i].frameType;
                                }
                                else  //不是第一次遇到第三种类型
                                {
                                    if (dataValueAndTypeAll[i].frameType == frameDataType3)
                                    {
                                        classNum = 3;
                                        frameDataTypeLast = dataValueAndTypeAll[i].frameType;
                                    }
                                    else
                                    {
                                        //MessageBox.Show("有其他情况2！");
                                    }
                                }
                            }
                        }
                        else if (frameDataTypeLast == frameDataType3)
                        {
                            if (dataValueAndTypeAll[i].frameType == frameDataType1)   //这次是第一种
                            {
                                classNum = 1;
                                frameDataTypeLast = frameDataType1;
                            }
                            else if (dataValueAndTypeAll[i].frameType == frameDataType2)   //这次是第二种
                            {
                                classNum = 2;
                                frameDataTypeLast = frameDataType2;
                            }
                            else
                            {
                                //MessageBox.Show("有其他情况3！");
                            }
                        }
                    }
                    if (classNum == 1)
                    {
                        dataValueAndType1.Add(dataValueAndTypeAll[i]);
                    }
                    else if (classNum == 2)
                    {
                        dataValueAndType2.Add(dataValueAndTypeAll[i]);
                    }
                    else if (classNum == 3)
                    {
                        dataValueAndType3.Add(dataValueAndTypeAll[i]);
                    }
                }
            }
        }


    }
}
