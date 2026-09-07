using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 实现数据加密的类，从原来版本里面复制过来的
 * 完成时间：2016.08.09 23：53
 * 完成人：汪锡  ahuwx@mail.ustc.edu.cn
 */

namespace Cardio.SPCL
{
    /// <summary>
    /// 数据加密的类
    /// </summary>
    public class Encryption
    {
        private static byte[] Unique_ID = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; //下位机唯一的ID号
        private static byte SP = 0x20;  //空格

        private byte[] CRC_ID;  //CRC多项式，用于生成CRCTable   CRCcheck类中的Polynomial转换为byte[]即可
        private CRCcheck crcCheck = new CRCcheck();

        //打乱顺序表
        private int[] bExchangeTable = {18, 0, 19, 1, 20, 2, 21, 3,
                                22, 4, 23, 5, 24, 6, 25, 7,
                                12, 8, 13, 9, 14, 10, 15, 11,
                                16, 28, 17, 29, 26, 30, 27, 31 };

        /// <summary>
        /// 产生1-12之间随机数12个
        /// </summary>
        /// <returns></returns>
        private List<byte> RandomNum()
        {
            List<byte> randomNum = new List<byte>();
            for (int i = 0; i < 12; i++)
            {
                Random rd = new Random();
                int randnum = rd.Next(1, 12);
                randomNum.Add((byte)randnum);
                //Task.Delay(20);
            }
            return randomNum;
        }

        /// <summary>
        /// 由下位机ID和随机数生成6byte的S序列
        /// </summary>
        /// <param name="UniqueID">下位机的ID</param>
        /// <param name="randomNum">随机数</param>
        /// <returns></returns>
        private List<byte> CalcuS(byte[] UniqueID, List<byte> randomNum)
        {
            List<byte> Snum = new List<byte>();
            //List<byte> randomNum = RandomNum();
            for (int i = 0; i < 6; i++)
            {
                byte S = (byte)(UniqueID[randomNum[2 * i]] + UniqueID[randomNum[2 * i + 1]]);
                Snum.Add(S);
            }
            return Snum;
        }

        /// <summary>
        /// 产生加密部分的数据区，没有打乱顺序
        /// </summary>
        /// <returns></returns>
        private List<byte> EncryptionDataOrder()
        {
            List<byte> encryptionData = new List<byte>();
            //产生随机数
            List<byte> randomNum = RandomNum();
            encryptionData.AddRange(randomNum);
            //由下位机ID和随机数生成6byte的S序列
            List<byte> Snum = CalcuS(Unique_ID, randomNum);
            encryptionData.AddRange(Snum);
            //时间序列
            DateTime DTnow = DateTime.Now;
            encryptionData.Add((byte)(DTnow.Year % 100)); //年份取后两位 如2014--14
            encryptionData.Add((byte)DTnow.Month);
            encryptionData.Add((byte)DTnow.Day);
            encryptionData.Add((byte)DTnow.Hour);
            encryptionData.Add((byte)DTnow.Minute);
            encryptionData.Add((byte)DTnow.Second);
            //CRC多项式
            CRC_ID = BitConverter.GetBytes(CRCcheck.Polynomial);
            encryptionData.AddRange(CRC_ID);
            //加密帧中的三个空格
            encryptionData.Add(SP);
            encryptionData.Add(SP);
            encryptionData.Add(SP);
            //加密帧中数据部分有个EOT
            encryptionData.Add(ComDataFormat.endBitDataSend);
            return encryptionData;
        }

        /// <summary>
        /// 产生打乱顺序的加密数据
        /// </summary>
        /// <returns></returns>
        public List<byte> EncryptionData()
        {
            List<byte> encryptionData = new List<byte>();
            List<byte> encryptionDataOrder = EncryptionDataOrder(); //产生数据区，没有打乱顺序
            //形成打乱顺序的加密数据
            for (int i = 0; i < encryptionDataOrder.Count; i++)
            {
                encryptionData.Add(encryptionDataOrder[bExchangeTable[i]]);
            }

            return encryptionData;
        }

    }
}
