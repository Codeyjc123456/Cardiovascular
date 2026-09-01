using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Model
{
    //class WorkStatus
    //{
    //}
    enum WorkStatus
    {
        #region 动脉硬化工作状态
        NoWork,//无工作 0
        StartBPtest,//开始血压测量
        StopBPtest,//停止血压测量
        StartPWVtest,//开始脉搏测量
        StopPWVtest,//停止脉搏测量
        OpenValue,//打开阀门
        CloseValue,//关闭阀门
        GetResult,//读取血压结果

        StartEPtest,//开始测量心电心音
        StopEPtest,//停止测量心电心音

        StartAItest,//开始测量心血管
        StopAItest,//停止测量心血管
        PreStart//获取实时压力
        #endregion
    }
}
