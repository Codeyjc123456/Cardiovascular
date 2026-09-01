using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 命令字：数据帧的类型的定义 和 数据帧下面数据种类的定义
 * Frm开头的为数据帧类型（Frame);Data开头的为数据类型
 * 根据通信协议，随时更新
 * 完成时间：2016.08.10 10：53
 * 更新时间：2016.08.10 10：53
 * 完成人：汪锡  ahuwx@mail.ustc.edu.cn
 */

namespace Cardio.SPCL
{
    #region 命令
    //public enum UpAndDownProtocolCmd : byte
    //{
    //    /// <summary>
    //    /// 加密命令
    //    /// </summary>
    //    CmdEncrypt = 0x00,

    //    /// <summary>
    //    /// 开机自检
    //    /// </summary>
    //    CmdSelfCheck = 0x01,

    //    /// <summary>
    //    /// 是否重新上电
    //    /// </summary>
    //    CmdPowerAgain = 0x02,

    //    /// <summary>
    //    /// 流量标定
    //    /// </summary>
    //    CmdFlowCalibrate = 0x03,

    //    /// <summary>
    //    /// 浓度标定1准备标定
    //    /// </summary>
    //    CmdConCalibrateOnePrep = 0x04,

    //    /// <summary>
    //    /// 浓度标定组分1
    //    /// </summary>
    //    CmdConCalibrateOne = 0x05,

    //    /// <summary>
    //    /// 浓度标定2准备标定
    //    /// </summary>
    //    CmdConCalibrateTwoPrep = 0x06,

    //    /// <summary>
    //    /// 浓度标定组分2
    //    /// </summary>
    //    CmdConCalibrateTwo = 0x07,

    //    /// <summary>
    //    /// 浓度标定3准备标定或采环境气体
    //    /// </summary>
    //    CmdConCalibrateThreePrep = 0x08,

    //    /// <summary>
    //    /// 浓度标定3或环境气体浓度
    //    /// </summary>
    //    CmdConCalibrateThree = 0x09,

    //    /// <summary>
    //    /// 环境温湿度
    //    /// </summary>
    //    CmdEnvironmentTandH = 0x0a,

    //    /// <summary>
    //    /// 确定抽气速度
    //    /// </summary>
    //    CmdFlowRateDetermine = 0x0b,

    //    /// <summary>
    //    /// 增加抽气速度
    //    /// </summary>
    //    CmdFlowRateUp = 0x0c,

    //    /// <summary>
    //    /// 减小抽气速度
    //    /// </summary>
    //    CmdFlowRateDown = 0x0d,

    //    /// <summary>
    //    /// 开始测试
    //    /// </summary>
    //    CmdBeginTest = 0x0e,

    //    /// <summary>
    //    /// 结束测试
    //    /// </summary>
    //    CmdEndTest = 0x10,

    //}
    #endregion

    #region 数据帧
    //public enum DataReceiveFrameType : byte
    //{
    //    /// <summary>
    //    /// 加密命令
    //    /// </summary>
    //    FrmEncrypt = 0x00,

    //    /// <summary>
    //    /// 开机自检
    //    /// </summary>
    //    FrmSelfCheck = 0x01,

    //    /// <summary>
    //    /// 是否重新上电
    //    /// </summary>
    //    FrmPowerAgain = 0x02,

    //    /// <summary>
    //    /// 流量标定
    //    /// </summary>
    //    FrmFlowCalibrate = 0x03,

    //    /// <summary>
    //    /// 浓度标定1准备标定
    //    /// </summary>
    //    FrmConCalibrateOnePrep = 0x04,

    //    /// <summary>
    //    /// 浓度标定组分1
    //    /// </summary>
    //    FrmConCalibrateOne = 0x05,

    //    /// <summary>
    //    /// 浓度标定2准备标定
    //    /// </summary>
    //    FrmConCalibrateTwoPrep = 0x06,

    //    /// <summary>
    //    /// 浓度标定组分2
    //    /// </summary>
    //    FrmConCalibrateTwo = 0x07,

    //    /// <summary>
    //    /// 浓度标定3准备标定或采环境气体
    //    /// </summary>
    //    FrmConCalibrateThreePrep = 0x08,

    //    /// <summary>
    //    /// 浓度标定3或环境气体浓度
    //    /// </summary>
    //    FrmConCalibrateThree = 0x09,

    //    /// <summary>
    //    /// 环境温湿度
    //    /// </summary>
    //    FrmEnvironmentTandH = 0x0a,

    //    /// <summary>
    //    /// 确定抽气速度
    //    /// </summary>
    //    FrmFlowRateDetermine = 0x0b,

    //    /// <summary>
    //    /// 增加抽气速度
    //    /// </summary>
    //    FrmFlowRateUp = 0x0c,

    //    /// <summary>
    //    /// 减小抽气速度
    //    /// </summary>
    //    FrmFlowRateDown = 0x0d,

    //    /// <summary>
    //    /// 开始测试浓度
    //    /// </summary>
    //    FrmBeginTestCon = 0x0e,

    //    /// <summary>
    //    /// 开始测试环境参数
    //    /// </summary>
    //    FrmBeginTestEnvironment = 0x0f,

    //    /// <summary>
    //    /// 结束测试
    //    /// </summary>
    //    FrmEndTest = 0x10,

    //}
    #endregion

    #region 数据类型
    //public enum DataReceiveDataType : byte
    //{
    //    /// <summary>
    //    /// 加密数据
    //    /// </summary>
    //    DataEncrypt = 0x00,

    //    /// <summary>
    //    /// 开机自检结果
    //    /// </summary>
    //    DataSelfCheck = 0x01,

    //    /// <summary>
    //    /// 是否预热
    //    /// </summary>
    //    DataWarmup = 0x02,

    //    /// <summary>
    //    /// 预热时间
    //    /// </summary>
    //    DataWarmupTime = 0x03,

    //    /// <summary>
    //    /// 流量标定数据
    //    /// </summary>
    //    DataFlowCalibrate = 0x04,

    //    /// <summary>
    //    /// 环境氧气浓度
    //    /// </summary>
    //    DataEnvironmentConO2 = 0x05,

    //    /// <summary>
    //    /// 环境CO2浓度
    //    /// </summary>
    //    DataEnvironmentConCO2 = 0x06,

    //    /// <summary>
    //    /// 环境温度
    //    /// </summary>
    //    DataEnvironmentT = 0x07,

    //    /// <summary>
    //    /// 环境湿度
    //    /// </summary>
    //    DataEnvironmentHumidity = 0x08,

    //    /// <summary>
    //    /// 确定抽气速度O2浓度数据
    //    /// </summary>
    //    DataFlowRateDetermineConO2 = 0x09,

    //    /// <summary>
    //    /// 确定抽气速度CO2浓度数据
    //    /// </summary>
    //    DataFlowRateDetermineConCO2 = 0x0a,

    //    /// <summary>
    //    /// 确定抽气速度压差数据
    //    /// </summary>
    //    DataFlowRateDetermineFlow = 0x0b,

    //    /// <summary>
    //    /// 正式测试氧气浓度数据
    //    /// </summary>
    //    DataStartTestConO2 = 0x0c,

    //    /// <summary>
    //    /// 正式测试二氧化碳浓度数据
    //    /// </summary>
    //    DataTestConCO2 = 0x0d,

    //    /// <summary>
    //    /// 正式测试的压差数据
    //    /// </summary>
    //    DataTestPressDiff = 0x0e,

    //    /// <summary>
    //    /// 正式测试的温度数据（流速相关）
    //    /// </summary>
    //    DataTestT = 0x0f,

    //    /// <summary>
    //    /// 正式测试的湿度数据（流速相关）
    //    /// </summary>
    //    DataTestHumidity = 0x10,

    //    /// <summary>
    //    /// 正式测试气压数据（流速相关）
    //    /// </summary>
    //    DataTestPressure = 0x11,

    //    /// <summary>
    //    /// 开始测试温度数据（浓度相关）
    //    /// </summary>
    //    DataTestTforCon = 0x12,

    //    /// <summary>
    //    /// 开始测试湿度数据（浓度相关）
    //    /// </summary>
    //    DataTestHumidityforCon = 0x13,

    //    /// <summary>
    //    /// 开始测试气压（浓度相关）
    //    /// </summary>
    //    DataTestPressureforCon = 0x14,


    //    /// <summary>
    //    /// 查询状态
    //    /// </summary>
    //    DataQueryStatus = 0x15,


    //    /// <summary>
    //    /// 仅仅是命令
    //    /// </summary>
    //    OnlyCmd = 0x16,

    //    /// <summary>
    //    /// 错误
    //    /// </summary>
    //    Error = 0x17,


    //    ///// <summary>
    //    ///// 氧传感器计算浓度的温度
    //    ///// </summary>
    //    //TforCon = 0x0c,

    //    ///// <summary>
    //    ///// 浓度标定环境浓度 计算氧气浓度时候的电压
    //    ///// </summary>
    //    //VforCon = 0x0d,

    //    ///// <summary>
    //    ///// CO2传感器是否标定好
    //    ///// </summary>
    //    //IsConCO2CalibWell = 0x0e,

    //    ///// <summary>
    //    ///// 固定抽气速度的档位
    //    ///// </summary>
    //    //ExhaustFlowRate = 0x1c,

    //}
    #endregion

    /// <summary>
    /// 命令字：数据帧的类型的定义 和 数据帧下面数据种类的定义
    /// </summary>
    public class CommandWord
    {
        #region old数据帧类型
        /// <summary>
        /// 加密数据帧
        /// </summary>
        public byte FrmEncrypt { get { return 0x00; } }

        /// <summary>
        /// 开机自检数据帧
        /// </summary>
        public byte FrmSelfCheck { get { return 0x01; } }

        /// <summary>
        /// 重新上电数据帧
        /// </summary>
        public byte FrmPowerAgain { get { return 0x02; } }

        /// <summary>
        /// 系统状态查询
        /// </summary>
        public byte FrmQueryStatus { get { return 0x03; } }

        /// <summary>
        /// 设置下次测量的预期压力值
        /// </summary>
        public byte FrmNextPreSet { get { return 0x04; } }




        /// <summary>
        /// 环境温湿度数据帧
        /// </summary>
        public byte FrmEnvironmentTandH { get { return 0x0a; } }



        /// <summary>
        /// 开始测试浓度数据帧
        /// </summary>
        public static byte FrmBeginTestCon { get { return 0x0e; } }

        /// <summary>
        /// 开始测试环境参数数据帧
        /// </summary>
        public static byte FrmBeginTestEnvironment { get { return 0x0f; } }

        /// <summary>
        /// 结束测试数据帧
        /// </summary>
        public static byte FrmEndTest { get { return 0x10; } }

        /// <summary>
        /// 华科模块命令  
        /// </summary>
        public static byte HKcmd { get { return 0xF0; } }

        /// <summary>
        /// 华科模块命令 读设备序列号
        /// </summary>
        public static byte HKGetDeviceNum { get { return 0x31; } }

        /// <summary>
        /// 华科模块命令 开始采集
        /// </summary>
        public static byte HKTest { get { return 0x32; } }

        /// <summary>
        ///  华科模块命令 停止采集
        /// </summary>
        public static byte HKEndTest { get { return 0x33; } }

        /// <summary>
        ///  华科模块命令 设置放大倍数
        /// </summary>
        public static byte HKSet { get { return 0x34; } }

        /// <summary>
        /// 开始测试前的采集环境气体的准备标定
        /// </summary>
        public static byte FrmAirConBeforeTestPre { get { return 0x28; } }

        /// <summary>
        /// 开始测试前的采集环境气体标定
        /// </summary>
        public byte FrmAirConBeforeTest { get { return 0x29; } }


        #region 暂时不用的

        /// <summary>
        /// 确定抽气速度数据帧
        /// </summary>
        public byte FrmFlowRateDetermine { get { return 0x0b; } }

        /// <summary>
        /// 增加抽气速度数据帧
        /// </summary>
        public byte FrmFlowRateUp { get { return 0x0c; } }

        /// <summary>
        /// 减小抽气速度数据帧
        /// </summary>
        public byte FrmFlowRateDown { get { return 0x0d; } }

        /// <summary>
        /// 固定抽气速度
        /// </summary>
        public byte FrmExhaustFlowRate { get { return 0x12; } }



        /// <summary>
        /// 准备测试
        /// </summary>
        public byte FrmTestPrepare { get { return 0x13; } }

        #endregion




        #endregion


        #region 动脉硬化
        /// <summary>
        /// 验证MCU串号命令
        /// </summary>
        public static byte REQ_CHECK_MCUID { get { return 0x01; } }

        /// <summary>
        /// USB停止工作命令
        /// </summary>
        public static byte REQ_USB_STOP { get { return 0x02; } }

        /// <summary>
        /// 系统状态查询
        /// </summary>
        public static byte REQ_GET_SYS_STA { get { return 0x03; } }

        /// <summary>
        /// 预期压力设定
        /// </summary>
        public static byte REQ_BP_INIT_SET { get { return 0x04; } }
        /// 预期压力设定响应
        /// </summary>
        public static byte REQ_BP_INIT_SET_Back { get { return 0x04; } }
        /// <summary>
        /// 儿童血压测量
        /// </summary>
        public static byte REQ_BP_OBJECT_SET { get { return 0x05; } }
        /// 儿童血压测量响应
        /// </summary>
        public static byte REQ_BP_OBJECT_SET_Back { get { return 0x05; } }

        /// <summary>
        /// 成人血压测量
        /// </summary>
        public static byte REQ_BP_START { get { return 0x06; } }
        /// <summary>
        /// 成人血压测量响应
        /// </summary>
        public static byte REQ_BP_START_Back { get { return 0x06; } }

        /// <summary>
        /// 血压测量停止
        /// </summary>
        public static byte REQ_BP_STOP { get { return 0x07; } }
        /// <summary>
        /// 血压测量停止响应
        /// </summary>
        public static byte REQ_BP_STOP_Back { get { return 0x07; } }

        /// <summary>
        /// 获取血压测量结果
        /// </summary>
        public static byte REQ_BP_GET_RESULT { get { return 0x08; } }
        /// <summary>
        /// 获取血压测量结果响应
        /// </summary>
        public static byte REQ_BP_GET_RESULT_Back { get { return 0x08; } }

        /// <summary>
        ///获取袖带压力
        /// </summary>
        public static byte REQ_BP_PRESSURE { get { return 0x09; } }
        /// <summary>
        ///获取袖带压力响应
        /// </summary>
        public static byte REQ_BP_PRESSURE_Back { get { return 0x09; } }

        /// <summary>
        ///控制气动装置指令
        /// </summary>
        public static byte REQ_BP_CONTROL { get { return 0x0A; } }
        /// <summary>
        ///控制气动装置指令响应
        /// </summary>
        public static byte REQ_BP_CONTROL_Back { get { return 0x0A; } }

        /// <summary>
        ///BP模块压力校准命令【在此命令之前，需要控制气动装置
        /// </summary>
        public static byte REQ_BP_PRESS_CAL { get { return 0x0B; } }

        /// <summary>
        ///BP模块压力校准读取命令【在此命令之前，需要进行压力校准，】
        /// </summary>
        public static byte REQ_BP_READ_NO { get { return 0x0C; } }

        /// <summary>
        ///脉搏波测量开始指令
        /// </summary>
        public static byte REQ_PWV_START { get { return 0x0D; } }
        /// <summary>
        ///脉搏波测量开始指令响应
        /// </summary>
        public static byte REQ_PWV_START_Back { get { return 0x0D; } }

        /// <summary>
        ///脉搏波测量停止指令
        /// </summary>
        public static byte REQ_PWV_STOP { get { return 0x0E; } }
        /// <summary>
        ///脉搏波测量停止指令响应
        /// </summary>
        public static byte REQ_PWV_STOP_Back { get { return 0x0E; } }

        /// <summary>
        ///脉搏波增益增大指令
        /// </summary>
        public static byte REQ_PWV_INC_GAIN { get { return 0x0F; } }
        /// <summary>
        ///脉搏波增益增大指令响应
        /// </summary>
        public static byte REQ_PWV_INC_GAIN_Back { get { return 0x0F; } }

        /// <summary>
        ///脉搏波增益减小指令
        /// </summary>
        public static byte REQ_PWV_DEC_GAIN { get { return 0x10; } }
        /// <summary>
        ///脉搏波增益减小指令响应
        /// </summary>
        public static byte REQ_PWV_DEC_GAIN_Back { get { return 0x10; } }

        /// <summary>
        ///固件信息读取，
        /// </summary>
        public static byte REQ_GET_FIRM { get { return 0x11; } }

        /// <summary>
        ///固件信息修改
        /// </summary>
        public static byte REQ_SET_FIRM { get { return 0x12; } }

        /// <summary>
        ///MCU复位指令
        /// </summary>
        public static byte REQ_MCU_RESET { get { return 0x13; } }

        #endregion
        #region 动脉硬化数据类型
        /// <summary>
        ///  系统状态1
        /// </summary>
        public static byte DataStatus { get { return 0x03; } }

        /// <summary>
        ///  系统状态2
        /// </summary>
        public static byte DatawErrors { get { return 0x04; } }

        /// <summary>
        ///  系统状态3
        /// </summary>
        public static byte DatabCheckSta { get { return 0x05; } }

        /// <summary>
        ///  设置下次测量的预期压力值
        /// </summary>
        public static byte Data_BP_INIT_SET { get { return 0x06; } }

        /// <summary>
        ///  儿童血压测量应答
        /// </summary>
        public static byte Data_BP_OBJECT_SET { get { return 0x07; } }

        /// <summary>
        ///  成人血压测量应答
        /// </summary>
        public static byte Data_BP_START { get { return 0x08; } }

        /// <summary>
        ///  血压测量中止指令
        /// </summary>
        public static byte Data_BP_STOP { get { return 0x09; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public static byte Data_BP_sys { get { return 0x0A; } }
        public static byte Data_BP_Result { get { return 0x0A; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public static byte Data_BP_dia { get { return 0x0B; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public static byte Data_BP_BTC { get { return 0x0C; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public static byte Data_BP_BPS { get { return 0x0D; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public static byte Data_BP_RATE { get { return 0x0E; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public static byte Data_BP_MAP { get { return 0x0F; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public static byte Data_BP_EC { get { return 0x10; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public static byte Data_BP_SP1 { get { return 0x11; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public byte Data_BP_SP2 { get { return 0x12; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public byte Data_BP_CKSUM { get { return 0x13; } }

        /// <summary>
        ///  获取血压测量结果指令
        /// </summary>
        public byte Data_BP_BPX { get { return 0x14; } }

        /// <summary>
        ///  袖带压力返回应答
        /// </summary>
        public byte Data_BP_prs { get { return 0x15; } }

        /// <summary>
        ///  设置下次测量的预期压力值  10
        /// </summary>
        public byte Data_BP_CONTROL { get { return 0x16; } }

        /// <summary>
        ///  BP模块压力校准命令【在此命令之前，需要控制气动装置 11
        /// </summary>
        public byte Data_BP_PRESS_CAL { get { return 0x17; } }

        /// <summary>
        ///  BP模块压力校准读取命令【在此命令之前，需要进行压力校准，】12
        /// </summary>
        public byte Data_BP_READ_NO { get { return 0x18; } }

        /// <summary>
        ///  脉搏波测量开始指令 13
        /// </summary>
        public byte Data_PWV_Time { get { return 0x19; } }

        /// <summary>
        ///  脉搏波测量开始指令 13
        /// </summary>
        public byte Data_PWV_Position { get { return 0x1A; } }

        /// <summary>
        ///  脉搏波测量开始指令 13
        /// </summary>
        public byte Data_PWV_Pulse { get { return 0x1B; } }
        /// <summary>
        /// 桡动脉脉搏波测量开始指令
        /// </summary>
        public byte Data_PWV_AI_Pulse { get { return 0x1B; } }
        /// <summary>
        ///  脉搏波测量停止指令 14
        /// </summary>
        public byte Data_PWV_STOP { get { return 0x1C; } }

        /// <summary>
        ///  脉搏波增益增大指令 15
        /// </summary>
        public byte Data_PWV_INC_GAIN { get { return 0x1D; } }

        /// <summary>
        ///  心电信号 17
        /// </summary>
        public byte Data_ECG { get { return 0x20; } }

        /// <summary>
        ///  脉搏波增益减小指令 16
        /// </summary>
        public byte Data_PWV_DEC_GAIN { get { return 0x1E; } }

        /// <summary>
        ///  脉搏波测量数据返回 21  左臂
        /// </summary>
        public byte Data_LB_Pulse { get { return 0x21; } }

        /// <summary>
        ///  脉搏波测量数据返回 22  左脚
        /// </summary>
        public byte Data_LA_Pulse { get { return 0x22; } }

        /// <summary>
        ///  脉搏波测量数据返回 23  右臂
        /// </summary>
        public byte Data_RB_Pulse { get { return 0x23; } }

        /// <summary>
        ///  脉搏波测量数据返回 24  右脚
        /// </summary>
        public byte Data_RA_Pulse { get { return 0x24; } }

        /// <summary>
        ///  脉搏波测量数据返回 25  桡动脉
        /// </summary>
        public byte Data_Radial_Pulse { get { return 0x25; } }

        /// <summary>
        ///  华科数据包 大包 32
        /// </summary>
        public byte Data_HKSample { get { return 0x09; } }
        public byte Data_HKJD { get { return 0x00; } }
        public byte Data_HKGD { get { return 0x01; } }
        public byte Data_HKECG { get { return 0x02; } }
        /// <summary>
        ///  华科数据包 小包 32
        /// </summary>
        public byte Data_HKXinyin { get { return 0x03; } }
        #endregion

        #region 代谢车数据类型
        /// <summary>
        /// 加密数据
        /// </summary>
        public byte DataEncrypt { get { return 0x00; } }

        /// <summary>
        /// 开机自检结果
        /// </summary>
        public byte DataSelfCheck { get { return 0x01; } }

        /// <summary>
        /// 是否预热
        /// </summary>
        public byte DataWarmup { get { return 0x02; } }

        /// <summary>
        /// 预热时间
        /// </summary>
        public byte DataWarmupTime { get { return 0x03; } }

        /// <summary>
        /// 流量标定数据
        /// </summary>
        public byte DataFlowCalibrate { get { return 0x04; } }

        /// <summary>
        /// 浓度标定中用于计算氧气浓度所需要的温度
        /// </summary>
        public byte DataCalibrateTforCon { get { return 0x05; } }

        /// <summary>
        /// 用户计算氧气浓度的ADC值
        /// </summary>
        public byte DataCalibrateADCforCon { get { return 0x06; } }

        /// <summary>
        /// CO2传感器是否标定好
        /// </summary>
        public byte DataConCO2CalibWell { get { return 0x07; } }

        /// <summary>
        /// 环境氧气浓度
        /// </summary>
        public byte DataEnvironmentConO2 { get { return 0x08; } }

        /// <summary>
        /// 环境CO2浓度
        /// </summary>
        public byte DataEnvironmentConCO2 { get { return 0x09; } }

        /// <summary>
        /// 环境温度
        /// </summary>
        public byte DataEnvironmentT { get { return 0x0a; } }

        /// <summary>
        /// 环境湿度
        /// </summary>
        public byte DataEnvironmentHumidity { get { return 0x0b; } }

        /// <summary>
        /// 确定抽气速度O2浓度数据
        /// </summary>
        public byte DataFlowRateDetermineConO2 { get { return 0x0c; } }

        /// <summary>
        /// 确定抽气速度CO2浓度数据
        /// </summary>
        public byte DataFlowRateDetermineConCO2 { get { return 0x0d; } }

        /// <summary>
        /// 确定抽气速度压差数据
        /// </summary>
        public byte DataFlowRateDetermineFlow { get { return 0x0e; } }

        /// <summary>
        /// 正式测试氧气浓度数据
        /// </summary>
        public byte DataTestConO2 { get { return 0x0f; } }

        /// <summary>
        /// 正式测试二氧化碳浓度数据
        /// </summary>
        public byte DataTestConCO2 { get { return 0x10; } }

        /// <summary>
        /// 正式测试的压差数据
        /// </summary>
        public byte DataTestPressDiff { get { return 0x11; } }

        /// <summary>
        /// 正式测试的温度数据（流速相关）
        /// </summary>
        public byte DataTestT { get { return 0x12; } }

        /// <summary>
        /// 正式测试的湿度数据（流速相关）
        /// </summary>
        public byte DataTestHumidity { get { return 0x13; } }

        /// <summary>
        /// 正式测试气压数据（流速相关）
        /// </summary>
        public byte DataTestPressur { get { return 0x14; } }

        /// <summary>
        /// 开始测试温度数据（浓度相关）
        /// </summary>
        public byte DataTestTforCon { get { return 0x15; } }

        /// <summary>
        /// 开始测试湿度数据（浓度相关）
        /// </summary>
        public byte DataTestHumidityforCon { get { return 0x16; } }

        /// <summary>
        /// 开始测试气压（浓度相关）
        /// </summary>
        public byte DataTestPressureforCon { get { return 0x17; } }


        /// <summary>
        /// 查询状态
        /// </summary>
        public byte DataQueryStatus { get { return 0x18; } }


        /// <summary>
        /// 采集环境气体
        /// </summary>
        public byte DataAirConCO2BeforeTest { get { return 0x19; } }


        /// <summary>
        /// 仅仅是命令
        /// </summary>
        public byte OnlyCmd { get { return 0x29; } }

        /// <summary>
        /// 错误
        /// </summary>
        public byte Error { get { return 0x2a; } }


        #endregion
    }
}
