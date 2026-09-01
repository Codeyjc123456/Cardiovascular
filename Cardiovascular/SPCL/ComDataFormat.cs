using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * 产生发送数据帧
 * 完成时间：2016.08.10 00：11
 * 完成人：汪锡  ahuwx@mail.ustc.edu.cn
 */

namespace Cardio.SPCL 
{
    /// <summary>
    /// 数据通信的格式（产生发送数据的帧）
    /// </summary>
    public class ComDataFormat
    {
        public static byte startBitDataSend = 0x41;
        public static byte HKstartBit = 0xF0;
        public static byte endBitDataSend = 0xFF;
        public static byte sumBitDataSend = 0xFF;
        CommandWord cmdWord = new CommandWord();
        CRCcheck crcCheck = new CRCcheck();
        private Encryption encryption = new Encryption();
        /// <summary>
        /// 产生需要发送的一帧数据
        /// </summary>
        /// <param name="cmd">命令类型</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public byte[] DataToSend(byte cmd, List<byte> datas)
        {
            //byte[] paras = GenerateParasData(cmd, data);  //由发送的数字转换成发送的数据byte
            List<byte> bufferToSend = new List<byte>();
            if (cmd == CommandWord.HKcmd)//华科模块命令格式
            {
                if (datas[0] == CommandWord.HKSet)
                {
                    bufferToSend.Add(cmd);
                    foreach(byte data in datas)
                    {
                        bufferToSend.Add(data);
                    }
                    byte CKSUM = (byte)(ComDataFormat.HKstartBit + datas[0]);
                    CKSUM = (byte)(CKSUM & 0x00FF);
                    bufferToSend.Add(CKSUM);
                }
                else
                {
                    bufferToSend.Add(cmd);//命令类型位/起始位
                    bufferToSend.Add(datas[0]); //
                    byte CKSUM = (byte)(ComDataFormat.HKstartBit + datas[0]);
                    CKSUM = (byte)(CKSUM & 0x00FF);
                    bufferToSend.Add(CKSUM);
                }
            }
            else
            {
                bufferToSend.Add(ComDataFormat.startBitDataSend); //起始位
                                                                  //数据位
                if (cmd == CommandWord.REQ_PWV_STOP /*|| cmd == cmdWord.REQ_BP_STOP*/)
                {
                    bufferToSend.Add(0x05);
                    bufferToSend.Add(cmd);//命令类型位
                    //foreach (byte data in datas)
                    //{
                    //    bufferToSend.Add(data);
                    //}
                }
                else if (cmd == CommandWord.REQ_BP_INIT_SET)
                {
                    bufferToSend.Add(0x08);
                    bufferToSend.Add(cmd);
                    foreach (byte data in datas)
                    {
                        bufferToSend.Add(data);
                    }
                }
                else
                {
                    bufferToSend.Add(0x07);
                    bufferToSend.Add(cmd);//命令类型位

                    foreach (byte data in datas)
                    {
                        bufferToSend.Add(data);
                    }
                }
                bufferToSend.Add(ComDataFormat.endBitDataSend);//结束位
                bufferToSend.Add(ComDataFormat.sumBitDataSend);//校验位
            }
            return bufferToSend.ToArray();
        }
    }
}
