using Cardio.Algorithm;
using Cardio.BLL;
using Cardio.DAL;
using Cardio.Model;
using Cardio.SPCL;
using Cardio.Util;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cardio.Views.DataManagePage.Mc;

namespace Cardio.Views.MeasurePage
{
    /// <summary>
    /// MeasureReePage.xaml 的交互逻辑
    /// </summary>
    public partial class MeasureReePage : Page
    {
        private Action<string> ReportAction;
        private volatile bool aiMeasurementActive;
        private string LastMsg = "";
        private PulseDataLocalDAL pulsedataDAL = null;
        private PulseDataLocalEntity pulsedata = new();
        private UserInfoEntity userInfo = new ();
        //定义串口
        private SerialPortManager serialPortManager = null;
        private StringMergeSplit stringMerge = new ();//定义一个字符串合并的类  
        private WorkStatus workStatus = WorkStatus.NoWork;                         
        private int[] iData = new int[2];//桡动脉
        private string strTip;
        //只发送一次读取血压结果的命令
        private bool PWVStopFist = true;
        //--------------------数据库操作
        private DateTime g_strDateTime;
        private long DataCount = 0;//桡动脉横坐标
        private int WaitForAck = 0;
        private int enumPwvMrsResp = 0;
        private int[] BP_ACK_Flag = new int[4];//BP血压模块应答分类：默认为0，
                                               //-2为打开气阀命令发送；
                                               //-3关闭阀门；
                                               //-9重新设置加压值
                                               //-11停止血压标志位
                                               //-12发送儿童血压标志位；
        private int get_bp_hr = 0;//返回血压模块心率
        private int Real_press;
        private int WaitTime = 60; //血压测量等待时间
        //保存两次测量的血压值
        private string strFirBraBp;
        private string strSecBraBp;
        //=====================PWV测量=================
        private long AICountOneSec;
        private long AICountTwoSec;
        #region
        //桡动脉诊断结果和建议
        private string strAIDiagnosisResult;
        private string strAIDiagnosisProposal;
        private int nMarkSelect;

        private List<double> RpRawData = new();//桡动脉原始数据
        private long RpRawDataNum;

        private long nCount;
        private List<double> DataOneSec = new List<double>();//临时数据用于筛选波形
        private List<double> DataTwoSec = new List<double>();
        private int nMarkWait;
        private bool SaveDataAfterAIAcquisitionFlag;
        #endregion
        //血压测量次数
        private bool bPressMrsStart = false;//血压测量按钮状态切换
       
        private int NumOfBpMrs = 0;     //血压测量次数1-3次
        private EnumData.MeasureMode PressMrsMode;
        private Variable.MrsBpValue[] arrGetBpSave = new Variable.MrsBpValue[2];//保存到处理好的血压数据
        private Variable.MrsBpValue[] arrGetBpHandle = new Variable.MrsBpValue[2];//处理血压响应的数据
        private Variable.MrsBpValue get_bp;//保存测量血压值
        private Variable.MrsBpValue FinalBpMrsValue;//最终的血压值
        private Variable.MrsBpValue g_typeBpMrsValue;
        private Variable.MrsIndexValue g_typeIndexValue;
        private DiagnosisResult.CardiacIndex g_typeCardiacIndex;
        private DiagnosisResult.VascularIndex g_typeVascularIndex;
        private string PulseMrsFinishFlag; //12秒桡动脉已经获取完毕：Yes 和 No两个值
        private byte BpModleIndex;
        #region 初始化
        MeasureViewModel measureViewModel = null;
        private readonly APPSettingsViewModel APPSettingUtil = APPSettingsViewModel.getInstance();
        System.Timers.Timer Timer_ABIDelay = null;
        System.Timers.Timer TimerBPTest = null;
        #endregion
        /// <summary>
        /// 构造对象 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="userTemp"></param>
        /// <param name="userUN"></param>
        public MeasureReePage(UserInfoEntity u)
        {
            InitializeComponent();
            userInfo = u;
            measureViewModel = new MeasureViewModel();
            DataContext = measureViewModel;
           
        }
        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            workStatus = WorkStatus.NoWork;
            APPSettingUtil.LoadData();
            //加载一下
            Thread thread = new Thread(() =>
            {
                Task.Delay(1000);
                Dispatcher.BeginInvoke(new Action(() => {
                    measureViewModel.IsRunningLoad = "Hidden";
                }));
            });
            thread.Start();
            Initialize();
            InitComponent();//初始化组件 
            InitTimer();
            InitShow();
            Recover();
            measureViewModel.InitializeZedGraph(AiSeries);
            LogUtil.Warn("真人测试");
            GlobalVariable.Pressure = 80;

            pulsedata.userCode = userInfo.UserCode;
            pulsedata.OperationgDoctor = userInfo.OperatingDoctor;
            pulsedata.orgId = userInfo.OrgId;

            //Ready.Visibility = Visibility.Hidden;
            //AITest.Visibility = Visibility.Visible;
            //measureViewModel.Tips = "温馨提示：血压测量完成！";
            //measureViewModel.Sbp = "120";
            //measureViewModel.Dbp = "80";

            //measureViewModel.Map = FinalBpMrsValue.Map.ToString();
        }
        private void Initialize()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                measureViewModel.Dbp = "--";
                measureViewModel.Sbp = "--";
                measureViewModel.Sevr = "--";
                measureViewModel.Hr = "--";
                measureViewModel.EdPct = "--";
                measureViewModel.Spti = "--";
                measureViewModel.Dpti = "--";
                measureViewModel.Pp = "--";
                measureViewModel.Cap = "--";
                measureViewModel.AIx = "--";
            }));
        }
        //用户信息更新，
        private void UserInformationShow()
        {
            measureViewModel.UserName = userInfo.UserName;
            measureViewModel.UserID = userInfo.UserId;
            measureViewModel.BirthDay = userInfo.UserAge.ToString();
            measureViewModel.UserSex = userInfo.UserSex.ToString();
            measureViewModel.UserHeight = userInfo.UserHeight.ToString() + "cm";
        }
        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitComponent()
        {
            pulsedataDAL = PulseDataLocalDAL.getInstance();
            Timer_ABIDelay = new System.Timers.Timer();
            TimerBPTest = new System.Timers.Timer();

            serialPortManager = SerialPortManager.getInstance();//串口业务
            serialPortManager.InformMsgEvnet = new SerialPortManager.InformMsg(ReciveAlarm);
            serialPortManager.InformDebugMsgEvnet = new SerialPortManager.InformDebugMsg(msg);

            //serialPortManager.SendData(CommandWord.REQ_PWV_START, 0x01);
            //Task.Delay(500);
            //serialPortManager.SendData(CommandWord.REQ_PWV_STOP, 0x01);
        }
        /// <summary>
        /// 初始化 定时器
        /// </summary>
        private void InitTimer()
        {
            TimerBPTest.Interval = 1000;// 1s 执行一次
            TimerBPTest.Enabled = false;
            TimerBPTest.AutoReset = true;
            TimerBPTest.Elapsed += new System.Timers.ElapsedEventHandler(TimerBPTest_Tick);
        }
        private void InitShow()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                measureViewModel.UserID = userInfo.UserId;
                measureViewModel.UserName = userInfo.UserName;
                measureViewModel.UserSex = userInfo.UserSex;
                measureViewModel.BirthDay = userInfo.UserBirthday.ToString();
                Ready.Visibility = Visibility.Visible;
                AITest.Visibility = Visibility.Hidden;
                OpenReport.Visibility = Visibility.Hidden;
                Hrup.Visibility = Visibility.Hidden;
                Edup.Visibility = Visibility.Hidden;
                Sptiup.Visibility = Visibility.Hidden;
                Dptiup.Visibility = Visibility.Hidden;
                Sevrup.Visibility = Visibility.Hidden;
                SBpup.Visibility = Visibility.Hidden;
                DBpup.Visibility = Visibility.Hidden;
                Ppup.Visibility = Visibility.Hidden;
                SBp2up.Visibility = Visibility.Hidden;
                AIxup.Visibility = Visibility.Hidden;
            }));
        }

        #region 清理
        public void Recover()
        {
            SetAiMeasurementActive(false);
            PressMrsMode = EnumData.MeasureMode.ExpertMode;
            Variable.Test_AI_num = 0;
            Variable.strMrsLocation = "肱踝";
            bPressMrsStart = true;
            Initialize();
            UserInformationShow();
            measureViewModel.Tips = "温馨提示：请做好测试前准备！";
            workStatus = WorkStatus.NoWork;
        }
        private void  TimerBPTest_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (workStatus != WorkStatus.NoWork)
            {
                if (this.WaitTime <= 0)
                {
                    TimerBPTest.Stop();
                    workStatus = WorkStatus.StartBPtest;
                    BpTest_Function(0x01);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        bPressMrsStart = true;
                        measureViewModel.Tips = "温馨提示：血压测量开始！";
                    }));
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        measureViewModel.Tips = "温馨提示：" + WaitTime.ToString() + "秒后自动进行第二次血压测量，请耐心稍等！";
                    }));
                    WaitTime = WaitTime - 1;
                }
            }
        }
        #endregion

        private async Task ProcessData()
        {
            //判断是保存血压数据还是脉搏波
            if (WaitForAck == MrsWaitForAck.MrsBP)
            {
                WaitForAck = 0;
                if (SaveDataAfterBP() == false)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        measureViewModel.Tips = "温馨提示：血压数据保存失败";
                    }));
                    Task.Delay(200);//异常错误处理
                    return;
                }
                //开始pwv测试前再发一次增加冗余，确保当前无工作在执行
                AlltestStopWhenPWVfail();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    measureViewModel.Tips = "温馨提示：血压数据测量完成！";
                }));
                workStatus = WorkStatus.NoWork;
                return;
            }
            else if (WaitForAck == MrsWaitForAck.MrsPulse)//桡动脉波形分析
            {
                try
                {
                    if (SaveDataAfterAIAcquisition() == false)//处理原始脉搏波数据
                    {
                        workStatus = WorkStatus.NoWork;
                        return;
                    }
                    if (g_typeVascularIndex.Cap == 0 && g_typeCardiacIndex.Hr == 0 && g_typeCardiacIndex.Ed == 0)//如果中心动脉压为0同时心率和射血时间也为0表明此次采集波形出现问题
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            measureViewModel.Tips = "温馨提示：桡动脉采集出现问题，请重新采集桡动脉！";
                        }));
                        workStatus = WorkStatus.NoWork;
                        return;
                    }
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        measureViewModel.Tips = "温馨提示：心血管功能测量完成！";
                    }));
                    workStatus = WorkStatus.NoWork;
                    pulsedata.AI_num = 1;
                    // 结果可用：先把 AI 诊断结果回填到记录，再自动打开医师诊断界面（保存时入库/更新）
                    pulsedata.AIDiagnosisResult = strAIDiagnosisResult;
                    pulsedata.AIDiagnosisProposal = strAIDiagnosisProposal;
                    Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        // 必须走 GetResultAsync：HandyControl 会在此时把 CloseAction 注入到 DataContext，
                        // 否则 Diagnosis 内点“保存/取消”时 CloseAction?.Invoke() 为空，无法自动关闭回到本页
                        await HandyControl.Controls.Dialog.Show(new Diagnosis(pulsedata)).GetResultAsync<string>();
                    }));
                }
                finally
                {
                    workStatus = WorkStatus.NoWork;
                    SetAiMeasurementActive(false);
                }
            }
        }
        private void UpdataToDatabase()
        {
            //保存数据
            int RawDataNum_Record;
            RawDataNum_Record = 12 * GlobalVariable.Sample_Rate;
            string RawPackData = "";
            RawPackData = stringMerge.MergeString(RpRawData);
            string str = "TestDateTime = '" + g_strDateTime + " '";
            pulsedata.Sbp = FinalBpMrsValue.Sbp;
            pulsedata.Dbp = FinalBpMrsValue.Dbp;
            pulsedata.Pp = FinalBpMrsValue.Sbp - FinalBpMrsValue.Dbp;
            pulsedata.Ed = g_typeCardiacIndex.Ed * 100;
            pulsedata.Sbp2 = g_typeVascularIndex.Cap;
            pulsedata.AIx = g_typeVascularIndex.AIx;
            pulsedata.Hr = g_typeCardiacIndex.Hr;
            pulsedata.Spti = Convert.ToInt32(g_typeCardiacIndex.Spti);
            pulsedata.Dpti = g_typeCardiacIndex.Dpti;
            pulsedata.Sevr = g_typeCardiacIndex.Sevr;
            pulsedata.EdPct = g_typeCardiacIndex.EdPct * 100;
            pulsedata.RpRawData = RawPackData;
            pulsedata.AIDiagnosisResult = strAIDiagnosisResult;
            pulsedata.AIDiagnosisProposal = strAIDiagnosisProposal;
            string Str_Testdate;
            Str_Testdate = string.Format("{0:yyyyMMddHHmmssffff}", g_strDateTime);
            Variable.MrsIndexValue.Upload_Report_Name = userInfo.UserId + "_" + Str_Testdate + ".pdf";//As100681....2022091302_.pdf
            pulsedata.Report_Name = Variable.MrsIndexValue.Upload_Report_Name;
            pulsedata.IsReportPrinted = "N";
            pulsedata.IsPDFReportPrinted = "N";
            string tips="";
            if (pulsedataDAL.Insert(pulsedata) > 0)
            {
                tips = "温馨提示：数据保存成功！";
                
            }
            else
            {
                tips = "温馨提示：数据保存失败！该次测试结果已保存到日志文件。"; 
                string testData = JsonConvert.SerializeObject(pulsedata);
                LogUtil.Info(testData);
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                measureViewModel.Tips = tips;
                measureViewModel.MeasureBtn_Img = "pack://application:,,,/Resources/Image/Measure/开始测量.jpg";
                TestBtn.IsEnabled = true;
                Variable.Test_ABI_num = 0;
                Variable.Test_AI_num = 0;
            }));
        }

        /// <summary>
        /// AI波形采集完成后处理原始数据，然后在分析数据
        /// </summas>
        /// <returns></returns>
        private bool SaveDataAfterAIAcquisition()
        {
            try
            {
                FeaturePoint featurepoint = new ();
                g_typeBpMrsValue.Sbp = FinalBpMrsValue.Sbp;
                g_typeBpMrsValue.Dbp = FinalBpMrsValue.Dbp;
                //g_typeBpMrsValue.Sbp = 120;
                //g_typeBpMrsValue.Dbp = 80;
                try
                {
                    if (featurepoint.Identify(g_typeBpMrsValue.Sbp, g_typeBpMrsValue.Dbp, RpRawData) == 0)//保存数据
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            measureViewModel.Tips = "温馨提示：波形分析出错，请重新测量";
                        }));
                        ChangeBtStyle(AITest, "BigBlueBtnStyle", "心血管测试");
                        SaveDataAfterAIAcquisitionFlag = false;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.measureViewModel.Tips = "温馨提示：分析桡动脉测量数据失败:稳定段数据较少！";
                    }));
                    AITestIntit();
                    ChangeBtStyle(AITest, "BigBlueBtnStyle", "心血管测试");
                    return false;
                }
                //心率值是否有误string result = ;//把数字字符串中的数字提取出来
                g_typeCardiacIndex.Hr = Convert.ToInt16(featurepoint.index[1, 0]);
                if (g_typeCardiacIndex.Hr < 40)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        measureViewModel.Tips = $"温馨提示：心率为{g_typeCardiacIndex.Hr}次/分,测量有误，请重新测量!";//直接退出检测
                    }));
                    return false;
                }

                g_typeVascularIndex.Sbp = g_typeBpMrsValue.Sbp;
                g_typeVascularIndex.Dbp = g_typeBpMrsValue.Dbp;
                g_typeVascularIndex.Pp = g_typeBpMrsValue.Sbp - g_typeBpMrsValue.Dbp;
                g_typeCardiacIndex.SBp = g_typeVascularIndex.Sbp;
                g_typeCardiacIndex.DBp = g_typeVascularIndex.Dbp;
                g_typeCardiacIndex.Hr = Convert.ToInt16(featurepoint.index[1, 0]);
                g_typeCardiacIndex.EdPct = Convert.ToSingle(featurepoint.index[1, 1]);
                g_typeCardiacIndex.Spti = Convert.ToInt16(featurepoint.index[1, 2]);
                g_typeCardiacIndex.Dpti = Convert.ToInt16(featurepoint.index[1, 3]);
                g_typeCardiacIndex.Sevr = Convert.ToSingle(featurepoint.index[1, 4]);

                g_typeVascularIndex.Cap = Convert.ToInt16(featurepoint.index[1, 5]);
                g_typeVascularIndex.AIx = Convert.ToSingle(featurepoint.index[1, 6]);
                g_typeCardiacIndex.Ed = Convert.ToSingle(featurepoint.index[1, 7]);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    measureViewModel.Tips = "温馨提示：脉搏波信号分析中";
                    //显示指标
                    DisplayAIIndex();
                    ChangeBtStyle(AITest, "BigBlueBtnStyle", "心血管测试");
                }));
               

                //分析处理结果 
                List<double> arrCardiacIndex = new List<double>();
                List<double> arrVascularIndex = new List<double>();
                arrCardiacIndex.Add(g_typeCardiacIndex.Hr);
                arrCardiacIndex.Add(g_typeCardiacIndex.Ed);
                arrCardiacIndex.Add(g_typeCardiacIndex.EdPct);
                arrCardiacIndex.Add(g_typeCardiacIndex.Spti);
                arrCardiacIndex.Add(g_typeCardiacIndex.Dpti);
                arrCardiacIndex.Add(g_typeCardiacIndex.Sevr);
                arrCardiacIndex.Add(g_typeCardiacIndex.SBp);
                arrCardiacIndex.Add(g_typeCardiacIndex.DBp);

                arrVascularIndex.Add(g_typeVascularIndex.Sbp);
                arrVascularIndex.Add(g_typeVascularIndex.Dbp);
                arrVascularIndex.Add(g_typeVascularIndex.Pp);
                arrVascularIndex.Add(g_typeVascularIndex.Cap);
                arrVascularIndex.Add(g_typeVascularIndex.AIx);
                //计算指标
                DiagnosisResult diagnosis = new DiagnosisResult();
                diagnosis.AnalyseAIDiagnosisResult(userInfo.UserAge, userInfo.UserSex, arrCardiacIndex, arrVascularIndex);
                strAIDiagnosisResult = diagnosis.strDiagnosisResult;//桡动脉诊断结果建议赋值给测试界面变量
                strAIDiagnosisProposal = diagnosis.strDiagnosisProposal;
                Record_Index.AIDiagnonsisResult = strAIDiagnosisResult;
                Record_Index.AIDiagnonsisProproal = strAIDiagnosisProposal;
                g_typeIndexValue.DiagnosisResult = diagnosis.strDiagnosisResult;
                g_typeIndexValue.DiagnosisProposal = diagnosis.strDiagnosisProposal;
                //传递给可以用来后续保存数据的值
                SaveDataAfterAIAcquisitionFlag = true;
                Variable.Test_AI_num = 1;
                //UpdataToDatabase();
                return SaveDataAfterAIAcquisitionFlag;
            }
            catch
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.measureViewModel.Tips = "温馨提示：分析桡动脉测量数据失败！";
                }));
                AITestIntit();
                ChangeBtStyle(AITest, "BigBlueBtnStyle", "心血管测试");
                return false;
            }
        }

        private void DisplayAIIndex()
        {
            measureViewModel.Sevr = g_typeCardiacIndex.Sevr.ToString("f2");
            g_typeCardiacIndex.EdPct *= 100;
            measureViewModel.EdPct = g_typeCardiacIndex.EdPct.ToString("f2");
            measureViewModel.Hr = g_typeCardiacIndex.Hr.ToString("");
            measureViewModel.Sbp = g_typeCardiacIndex.SBp.ToString("");
            measureViewModel.Dbp = g_typeCardiacIndex.DBp.ToString("");
            measureViewModel.Spti = g_typeCardiacIndex.Spti.ToString("");
            measureViewModel.Dpti = g_typeCardiacIndex.Dpti.ToString("");
            measureViewModel.Pp = g_typeVascularIndex.Pp.ToString("");
            measureViewModel.Cap = g_typeVascularIndex.Cap.ToString("");
            measureViewModel.AIx = g_typeVascularIndex.AIx.ToString("f2");
            OpenReport.Visibility = Visibility.Visible;
            //心肌活力率
            void Validate(Func<double> getVal, double min, double max, Action<Visibility> setVis, Action<Brush> setColor, Action<string> setImg = null)
            {
                double val = getVal();
                bool isOut = val < min || val > max;
                setVis(isOut ? Visibility.Visible : Visibility.Hidden);
                setColor(isOut ? Brushes.Red : Brushes.Black);
                if (setImg != null && isOut && val < min) 
                    setImg("pack://application:,,,/Resources/Image/Measure/降低.png");
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Validate(() => Convert.ToDouble(measureViewModel.Sevr), 1.0, 4, v => Sevrup.Visibility = v, b => lblSevr.Foreground = b, p => measureViewModel.SevrUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.Hr), 60, 100, v => Hrup.Visibility = v, b => Hr.Foreground = b, p => measureViewModel.HrUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.EdPct), 30, 45, v => Edup.Visibility = v, b => EdPct.Foreground = b, p => measureViewModel.EdUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.Spti), 1800, 2500, v => Sptiup.Visibility = v, b => Spti.Foreground = b, p => measureViewModel.SptiUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.Dpti), 2300, 3500, v => Dptiup.Visibility = v, b => Dpti.Foreground = b, p => measureViewModel.DptiUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.Pp), 30, 45, v => Ppup.Visibility = v, b => PP.Foreground = b, p => measureViewModel.PpUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.Cap), 85, 110, v => SBp2up.Visibility = v, b => Cap.Foreground = b, p => measureViewModel.CapUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.AIx), 0.75, 99, v => AIxup.Visibility = v, b => AIx.Foreground = b, p => measureViewModel.AIxUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.Sbp), 90, 130, v => SBpup.Visibility = v, b => SBP.Foreground = b, p => measureViewModel.SBpUp = p);
                Validate(() => Convert.ToDouble(measureViewModel.Dbp), 60, 90, v => DBpup.Visibility = v, b => DBP.Foreground = b, p => measureViewModel.DBpUp = p);
            }));
            //显示波形
            strTip = "温馨提示：脉脉搏数据采集完成，正在保存... ...";
            measureViewModel.AIData.Clear();
            for (int i = 0; i < RpRawData.Count; i++)
            {
                measureViewModel. AIData.Add(RpRawData[i]);
                AiSeries.Refresh();
            }
        }

        #region 定时器事件
        //处理检测到的血压数据
        private void HandleBPData(ref Variable.MrsBpValue FourLimbBp)
        {
            //标准检测
            if (PressMrsMode == EnumData.MeasureMode.ExpertMode)
            {
                if (NumOfBpMrs <= 1)
                {
                    arrGetBpHandle[NumOfBpMrs].Sbp = FourLimbBp.Sbp;
                    arrGetBpHandle[NumOfBpMrs].Dbp = FourLimbBp.Dbp;
                    arrGetBpHandle[NumOfBpMrs].Map = FourLimbBp.Map;
                    arrGetBpSave[NumOfBpMrs] = arrGetBpHandle[NumOfBpMrs];
                    if (NumOfBpMrs < 1)
                    {
                        NumOfBpMrs = NumOfBpMrs + 1;
                        bPressMrsStart = false;
                        WaitTime = 60;   // 每次新测量轮次前重置倒计时，避免上一轮残留为0导致立刻进行第二次测量
                        TimerBPTest.Start();
                    }
                    //测量两次血压已完成
                    else
                    {
                        try
                        {
                            FinalBpMrsValue.Sbp = (arrGetBpHandle[0].Sbp + arrGetBpHandle[1].Sbp) / 2;
                            FinalBpMrsValue.Dbp = (arrGetBpHandle[0].Dbp + arrGetBpHandle[1].Dbp) / 2;
                            FinalBpMrsValue.Map = (arrGetBpHandle[0].Map + arrGetBpHandle[1].Map) / 2;
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                measureViewModel.Tips = ex.Message + "血压值计算失败！";
                            }));
                        }
                        
                        InitABIMrsRelatedVariables();
                       
                        CompleteABIMeasurement();
                        WaitForAck = MrsWaitForAck.MrsBP;
                        Task.Delay(1000);
                        _ = ProcessData();
                    }

                }

            }
            //快速测试模式
            if (PressMrsMode == EnumData.MeasureMode.ExperienceMode)
            {
                FinalBpMrsValue.Sbp = FourLimbBp.Sbp;
                FinalBpMrsValue.Dbp = FourLimbBp.Dbp;
                FinalBpMrsValue.Map = FourLimbBp.Map;
                arrGetBpSave[0] = FinalBpMrsValue;
                g_typeBpMrsValue = FinalBpMrsValue;
                //'变量清零
                InitABIMrsRelatedVariables();
                //  '两次血压测量完成
                CompleteABIMeasurement();
                //判断下一步应该执行什么操作
                Task.Delay(1000);
                _ = ProcessData();

            }
        }
        private void InitABIMrsRelatedVariables()
        {
            NumOfBpMrs = 0;//         '2次获取的血压值
            get_bp = default;
            Array.Clear(arrGetBpHandle,0,arrGetBpHandle.Length) ;
        }

        //血压测量完成计算ABI值判断健康状态
        private void CompleteABIMeasurement()
        {
            //修改血压相关控件状态把所有的textbox框都填上响应的数值
            measureViewModel.Sbp = FinalBpMrsValue.Sbp.ToString();
            measureViewModel.Dbp = FinalBpMrsValue.Dbp.ToString();
            measureViewModel.Map = FinalBpMrsValue.Map.ToString();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Ready.Visibility = Visibility.Hidden;
                AITest.Visibility = Visibility.Visible;
                measureViewModel.Tips = "温馨提示：血压测量完成！";
            }));
        }
        private void BuildBloodPressureStrings()
        {
            strFirBraBp = $"Left:{arrGetBpSave[0].Sbp}+{arrGetBpSave[0].Dbp}+{arrGetBpSave[0].Map}";
            strSecBraBp = $"Left:{arrGetBpSave[1].Sbp}+{arrGetBpSave[1].Dbp}+{arrGetBpSave[1].Map}";
        }
        private bool SaveDataAfterBP()
        {
            // 构建血压数据字符串
            BuildBloodPressureStrings();

            // 首次保存数据（ABI和AI都未测试时）
            if (Variable.Test_ABI_num == 0 && Variable.Test_AI_num == 0)
            {
                SaveFirstTimeData();
                return true;
            }
            // 第二轮及以后测完血压：覆盖前一次保存的数据
            SaveFirstTimeData();
            return true;
        }
        /// <summary>
        /// 首次保存数据
        /// </summary>
        private void SaveFirstTimeData()
        {
            g_strDateTime = DateTime.Now;
            Variable.ErrorTime = g_strDateTime;
            pulsedata.userId = userInfo.UserId;
            pulsedata.userName = userInfo.UserName;
            pulsedata.userSex = userInfo.UserSex;
            pulsedata.userAge = userInfo.UserAge;
            pulsedata.userHeight = userInfo.UserHeight;
            pulsedata.userWeight = userInfo.UserWeight;
            pulsedata.userBirthday = userInfo.UserBirthday.ToString();
            pulsedata.TestDateTime = g_strDateTime.ToString();
            pulsedata.ABI_num = 1;
            int totalSbp = 0, totalDbp = 0, count = 0;
            // 收集所有非零血压值
            if (g_typeBpMrsValue.Sbp != 0)
            {
                totalSbp += g_typeBpMrsValue.Sbp;
                totalDbp += g_typeBpMrsValue.Dbp;
                count++;
            }
            if (count > 0)
            {
                g_typeBpMrsValue.Sbp = totalSbp / count;
                g_typeBpMrsValue.Dbp = totalDbp / count;
            }
            else
            {
                g_typeBpMrsValue.Sbp = 0;
                g_typeBpMrsValue.Dbp = 0;
            }
            Record_Index.Sbp = g_typeBpMrsValue.Sbp;
            Record_Index.Dbp = g_typeBpMrsValue.Dbp;
            if (TestMode.LBBP_Select)
            {
                Record_Index.Sbp = g_typeBpMrsValue.Sbp;
                Record_Index.Dbp = g_typeBpMrsValue.Dbp;
            }
            pulsedata.Sbp = Record_Index.Sbp;
            pulsedata.Dbp = Record_Index.Dbp;
            pulsedata.Pp = Record_Index.Sbp - Record_Index.Dbp;
            pulsedata.Map = g_typeBpMrsValue.Map;
            pulsedata.BpHr = get_bp_hr;
            pulsedata.FristBraBP = strFirBraBp;
            pulsedata.SecondBraBP = strSecBraBp;
            pulsedata.HaveUpload = "N";
            pulsedata.IsRepertPrinted = "N";

            LogUtil.Info(JsonConvert.SerializeObject(pulsedata));
            Variable.Test_ABI_num = 1;
            Array.Clear(arrGetBpSave, 0, arrGetBpSave.Length);
        }
        private bool HandleAIPulseData(ref long iPos, ref int iData)
        {
            nCount = nCount + 1;
            //采样频率将至250Hz运算时间降低
            if (iPos % 2 == 0)
            {
                DataOneSec.Add((double)iData);//获取的一半采样点
                DataTwoSec.Add((double)iData);
                AICountTwoSec = AICountTwoSec + 1;
                AICountOneSec = AICountOneSec + 1;
            }
            //开始记录波形，到8结束，共12秒
            if (nMarkSelect >= 3 && nMarkSelect <= 9)
            {
                if (RpRawDataNum < 12 * GlobalVariable.Sample_Rate)
                {
                    RpRawData.Add(iData);
                    RpRawDataNum = RpRawDataNum + 1;
                }
            }
            int MaxValue;
            int MinValue;
            int DifValue;
            //放大倍数一秒调节一次
            if (nMarkSelect < 2 && AICountOneSec >= 1 && AICountOneSec == GlobalVariable.Sample_Rate / 2)
            {
                MaxValue = (int)DataOneSec.Max();
                MinValue = (int)DataOneSec.Min();
                DifValue = MaxValue - MinValue;
                if ((MinValue <= 100 && MaxValue - MinValue <= 800) || (MinValue > 100 && DifValue <= 850))
                {
                    //放大倍数加1
                    
                    if (!serialPortManager.SendData(CommandWord.REQ_PWV_INC_GAIN, 0x02))//桡动脉
                    {
                        measureViewModel.Tips = "脉搏波形幅度增加命令发送失败，请检查设备通信状况";
                        return false;
                    }
                }
                if (MaxValue >= 3900 || DifValue > 3700)
                {
                    //放大倍数减一
                    
                    if (!serialPortManager.SendData(CommandWord.REQ_PWV_DEC_GAIN, 0x02)) //桡动脉
                    {
                        measureViewModel.Tips = "脉搏波形幅度减小命令发送失败，请检查设备";
                        return false;
                    }
                }
                AICountOneSec = 0;
                DataOneSec.Clear();
            }
            //----------------------------------2秒钟筛选一次波形---------------------------------------------
            if (AICountTwoSec >= 1 && AICountTwoSec == GlobalVariable.Sample_Rate)
            {
                nMarkWait = nMarkWait + 1;
                if (nMarkSelect >= 2 && nMarkSelect <= 8)
                {
                    nMarkSelect = nMarkSelect + 1;
                    if (nMarkSelect >= 3)
                    {
                        measureViewModel.Tips = "温馨提示：脉搏波信号采集中，保持稳定";
                    }
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        measureViewModel.TestStatus = (Convert.ToDouble(nMarkSelect) / 12 * 100).ToString("f2");
                    }));

                    Console.WriteLine("nMarkSelect" + nMarkSelect);
                }

                if (nMarkSelect >= 9)
                {
                    if (!serialPortManager.SendData(CommandWord.REQ_PWV_STOP, 0x01))
                    {
                        strTip = "温馨提示：脉搏数据采集完成，脉搏停止指令没有发送成功，请检查设备连接状况！";
                        measureViewModel.Tips = strTip;
                    }
                    else
                    {
                        PulseMrsFinishFlag = "Yes";
                        //避免程序卡顿，停止测量指令发送后，休息300ms
                        Task.Delay(300);
                        enumPwvMrsResp = PulseMrsResp.PulseStop;
                        AICountOneSec = 0;
                        AICountTwoSec = 0;
                        DataOneSec.Clear();
                        DataTwoSec.Clear();
                        nMarkSelect = 0;
                        nCount = 0;
                        nMarkWait = 0;
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            strTip = "温馨提示：脉搏波测量完成！";
                            measureViewModel.AITest_Img = "pack://application:,,,/Resources/Image/Measure/心血管测试.png";
                        }));
                        if (PulseMrsFinishFlag == "Yes")
                        {
                            strTip = "温馨提示：桡动脉脉搏数据采集完成，正在保存... ...";
                            WaitForAck = MrsWaitForAck.MrsPulse;
                            measureViewModel.TestStatus = "100";
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                measureViewModel.Tips = strTip;
                            }));
                            bPressMrsStart = true;
                            PWVStopFist = true;
                            Task.Delay(200);
                        }
                    }
                    return true;
                }
                if (DataTwoSec.Count > 0)
                {
                    MaxValue = (int)DataTwoSec.Max();
                    MinValue = (int)DataTwoSec.Min();
                }
                else
                {
                    MaxValue = 0;
                    MinValue = 0;
                }
                DifValue = MaxValue - MinValue;
                AcquireData clsAcquisition;
                clsAcquisition = new AcquireData();
                //对每次获取的2秒脉搏波数据进行判断处理
                if (nMarkSelect < 2 && MinValue > 50 && DifValue > 600 && DifValue < 3700 && MaxValue < 3900)
                {
                    if (clsAcquisition.SelectWaveform(DataTwoSec, GlobalVariable.Sample_Rate) == 1)
                    {
                        nMarkSelect = nMarkSelect + 1;
                        //Growl.Info()
                    }
                }
                AICountTwoSec = 0;
                DataTwoSec.Clear();
                //30秒内测量未结束，40秒内提示，测量不停止：超过40秒测量自动停止
                if (nMarkSelect < 2 && nMarkWait >= 15)
                {
                    bool isForceStop = nMarkWait >= 20;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        measureViewModel.Tips = isForceStop
                            ? "温馨提示：桡动脉波测量停止，请重新绑扎后测量"
                            : "温馨提示：脉搏波信号不好，请重新绑扎或静息后测量";

                        if (isForceStop)
                        {
                            measureViewModel.AITest_Img = "pack://application:,,,/Resources/Image/Measure/心血管测试.png";
                            AITestIntit();
                        }
                    }));

                    if (isForceStop)
                    {
                        serialPortManager.SendData(CommandWord.REQ_PWV_STOP, 0x01);
                        workStatus = WorkStatus.NoWork;
                        SetAiMeasurementActive(false);
                        //serialPortManager.SendDataToMCU(CommandWord.REQ_PWV_STOP, null, APPSettingUtil.ShortWaitTime);
                        return false;
                    }
                }
            }
            return true;
        }
        private void BpTest_Function(byte commandBP)
        {
            workStatus = WorkStatus.StartBPtest;
            serialPortManager.SendData(CommandWord.REQ_BP_START, commandBP);
        }
        private void AlltestStopWhenPWVfail()
        {
            Task.Delay(1000);
            //指令发送失败
            BpModleIndex = 0x05;
            serialPortManager.OpenControl(BpModleIndex);
        }
        #endregion

        #region 处理串口数据事件
        private void ReciveAlarm(MCErrorCode code, List<ReceiveDataStructure> dataList)
        {
            if (code == MCErrorCode.NoError && dataList != null)
            {
                //不同类型的帧分开存储
                List<ReceiveDataStructure> dataValueAndTypePulse = new List<ReceiveDataStructure>();//脉搏波数据
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (dataList[i].frameType == CommandWord.REQ_PWV_START_Back)
                    {
                        if (dataList[i].dataValue == 1)   //桡动脉波形图
                        {
                            for (int p = 0; p < 14; p++)
                            {
                                iData[0] = dataList[i + 2 + p].dataValue;
                                DataCount++;
                                if (DataCount % 2 == 0)
                                {
                                    measureViewModel.AIData.Add(iData[0]);
                                    if (measureViewModel.AIData.Count % 50 == 0)
                                        AiSeries.Refresh();
                                    HandleAIPulseData(ref DataCount, ref iData[0]);
                                }
                                if (measureViewModel.AIData.Count >= 6000)
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        DataCount = 0;
                                        measureViewModel. AIData.Clear();
                                    }));
                                }
                            }
                            i += 15;
                        }
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_INC_GAIN_Back)
                    {
                        i += 2;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_DEC_GAIN_Back)
                    {
                        i = i + 2;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_INIT_SET)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            measureViewModel.Tips = "温馨提示：血压测量开始！";
                        }));
                        BpTest_Function(0x01);
                        i += 3;
                    }

                    else if (dataList[i].frameType == CommandWord.REQ_BP_OBJECT_SET_Back)
                    {
                        if (dataList[i + 1].dataValue == 0x4F && dataList[i + 2].dataValue == 0x6F)//"O"响应
                        {
                            int idx = dataList[3 + i].dataValue - 1;
                            if (idx < 0 || idx > 4)
                                return;
                            
                            if( (dataList[3 + i].dataValue) == 0x01)
                            {
                                if (BP_ACK_Flag[0] == -12)
                                {
                                    BP_ACK_Flag[0] = 0;
                                }
                                else
                                { 
                                    strTip = "温馨提示：温馨提示：血压发送成功";
                                    measureViewModel.Tips = strTip;
                                }
                            }
                        }
                        else if (dataList[i + 1].dataValue == 0x42 && dataList[i + 2].dataValue == 0x7C)
                        {
                            strTip = "温馨提示：左上臂袖带忙";
                            //只要提示忙则立马发送四肢停止测量命令，重新发送儿童血压测量命令,出现袖带忙的情况一般比较少见
                            measureViewModel.Tips = strTip;
                            serialPortManager.SendData(CommandWord.REQ_BP_STOP, 0x00);//停止血压测量
                            Task.Delay(2000);//停止完血压测量后，休息2000ms后重新开始充气测量
                            measureViewModel.Tips = "温馨提示：PWV测量开始";
                        }
                        else if (dataList[i + 1].dataValue == 0x4B && dataList[i + 2].dataValue == 0x73)
                        {
                            strTip = "温馨提示：左上臂袖带测量完成";
                            measureViewModel.Tips = strTip;
                        }
                        i += 3;
                    }
                   
                    else if (dataList[i].frameType == CommandWord.REQ_BP_START_Back)
                    {
                        if ((dataList[i + 1].dataValue == 0x4F) && (dataList[i + 2].dataValue == 0x6F))////收到响应指令“O”
                        {
                           strTip = "温馨提示：正在进行血压测量";       
                        }
                        else if ((dataList[i + 1].dataValue == 0x42) && (dataList[i + 2].dataValue == 0x7C))////收到响应指令“B”
                        {
                            strTip = "温馨提示：血压模块忙";
                        }
                        else if ((dataList[i + 1].dataValue == 0x4B) && (dataList[i + 2].dataValue == 0x73))////收到响应指令“K”（指令执行完成）
                        {
                            strTip = "温馨提示：血压数据测量完成";
                            if (serialPortManager.SendData(CommandWord.REQ_BP_GET_RESULT, TestMode.CommandWordSend))
                            {
                                if (NumOfBpMrs < 1)
                                {
                                    strTip = "温馨提示：正在读取血压测量结果!";
                                }
                                else
                                {
                                    strTip = "温馨提示：请点击心血管测量按钮继续测量!";
                                    ChangeBtStyle(TestBtn, "BigBlueBtnStyle", "开始测量");
                                }
                            }
                        }
                        measureViewModel.Tips = strTip;
                        i += 3;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_STOP_Back)
                    {
                        if ((dataList[1 + i].dataValue == 0x41) && (dataList[2 + i].dataValue == 0x7D))////收到响应指令“A”
                        {
                            if (workStatus == WorkStatus.CloseValue) //判断如果是关闭阀门，则发送关闭阀门指令
                            {
                                Task.Delay(300);
                                if (serialPortManager.CloseControl(0x00))
                                {
                                    //DisPlayTips("关闭阀门中......");
                                }
                            }
                        }
                        //判断bpx是那一位数据，返回对应的测量结果
                        else if ((dataList[1 + i].dataValue == 0x4B) && (dataList[2 + i].dataValue == 0x73))////收到响应指令“K”
                        {
                            strTip = "血压测量停止";
                            Growl.Info("温馨提示：" + strTip);
                            strTip = "";
                        }
                        i += 3;
                    }
                    //血压数据返回信息
                    else if (dataList[i].frameType == CommandWord.REQ_BP_GET_RESULT_Back)
                    {
                        get_bp.Sbp = dataList[1 + i].dataValue;
                        get_bp.Dbp = dataList[2 + i].dataValue;
                        get_bp_hr = dataList[5 + i].dataValue;
                        get_bp.Map = dataList[6 + i].dataValue;
                        //当四肢全采到血压值后进入，对血压值进行处理
                        string fault = "";
                        try
                        {
                            if (get_bp.Sbp == 0)
                            {
                                fault = GetBPResultFault(dataList[6].dataValue);
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    measureViewModel.Tips = fault;
                                }));
                                return;
                            }
                            HandleBPData(ref get_bp);//get_Bp为保存了下位机测得的血压信息
                        }
                        catch
                        {
                            measureViewModel.Tips = "温馨提示：测量过程中出现异常，请重新测量！";
                        }

                        i += 11;
                    }
                    
                    else if (dataList[i].frameType == CommandWord.REQ_BP_PRESSURE_Back)
                    {
                        Real_press = dataList[i + 1].dataValue;
                        i += 3;
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_BP_CONTROL_Back)
                    {
                        if (dataList[1 + i].dataValue == 0x4B && dataList[2 + i].dataValue == 0x73)//响应“O”"K"
                        {
                            //如果相等，则表示是pwv过程还未测试完成时，发送的打开阀门按钮，那么此时发送停止采集pwv
                            if (enumPwvMrsResp == PWVMrsResp.PwvStart)
                            {

                                serialPortManager.SendData(CommandWord.REQ_PWV_STOP, 0x05);
                            }
                        }
                        measureViewModel.Tips = strTip;
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
                        measureViewModel.Tips = strTip;
                        i += 3;
                    }
                
                    else if (dataList[i].frameType == CommandWord.REQ_BP_READ_NO)
                    {
                        if ((dataList[1].dataValue == 79) && (dataList[2].dataValue == 111))////收到响应指令“O”
                        {
                            strTip = "温馨提示：正在进行校准....";
                        }
                        measureViewModel.Tips = strTip;
                        i += 5;
                    }
                  
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_STOP_Back)
                    {
                        if (PWVStopFist)
                        {
                            PWVStopFist = false;
                            if (PulseMrsFinishFlag == "Yes")
                            {
                                strTip = "温馨提示：桡动脉脉搏数据采集完成，正在保存... ...";
                                WaitForAck = MrsWaitForAck.MrsPulse;
                                measureViewModel.TestStatus = "100";
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    measureViewModel.Tips = strTip;
                                })); 
                                Task.Delay(1000);
                                _ = ProcessData();
                                bPressMrsStart = true;
                                PWVStopFist = true;
                                Task.Delay(200);
                            }
                        }
                    }
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_INC_GAIN_Back)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            measureViewModel.Tips = "温馨提示：脉搏波增益成功";
                        }));
                    }
                
                    else if (dataList[i].frameType == CommandWord.REQ_PWV_DEC_GAIN_Back)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            measureViewModel.Tips = "温馨提示：脉搏波减益成功";
                        }));
                    }
                }
            }

            else if (code == MCErrorCode.OpenSerialFail)
            {
                HandyControl.Controls.Growl.Warning("打开串口失败！");
                Recover();
            }

            else if (code == MCErrorCode.TimeLimited)//超时
            {
                HandyControl.Controls.Growl.Warning("接收超时！");
                if (serialPortManager.SendTimes >= 2)
                {
                    HandyControl.Controls.Growl.Warning("通讯故障！");
                    //Recover();
                }
            }
        }
        private string GetBPResultFault(int ECFault)
        {
            string FaultStr = "";
            FaultStr = (ECFault) switch
            {
                0x01 => "从袖带获得脉搏波信号弱",
                0x02 => "从袖带获得脉搏波信号不稳定",
                0x03 => "血压值超出测量范围",
                0x04 => "超过测量时限",
                0x55 => "气动堵塞",
                0x56 => "用户终止读取BP值",
                0x57 => "充气超时、漏气或袖带松动",
                0x58 => "安全超时",
                0x59 => "袖带过压",
                0x5a => "电源超出范围或其他硬件问题",
                0x5b => "如未授权的命令或超出范围的自动归零",
                0x61 => "传感器超出范围",
                0x62 => "EEPROM校准数据故障",
                _ => "测量模块错误，请重新测量"
            };
            bPressMrsStart = true;   //测量按钮状态切换
            measureViewModel.MeasureBtn_Img = "pack://application:,,,/Resources/Image/Measure/开始测量.jpg";
            ChangeBtStyle(TestBtn, "BigBlueBtnStyle", "开始测量");
            return "血压结果读取失败，" + FaultStr;
        }
        private void msg(string m)
        {
            Console.WriteLine("打印信息："+m);
        }
        #endregion

        private void BackBtn(object sender, RoutedEventArgs e)
        {
            if (aiMeasurementActive) return;
            //this.NavigationService.Navigate(new OpenReportPage(pulsedata, ReportAction));
            if (measureViewModel.IsRunning == "Visible")
            {
                Growl.Info("正在测试，是否确定继续操作？");
                return;
            }
            else
            {
                bPressMrsStart = true;
                this.NavigationService.GoBack();
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
        private void InitMreasureRelatedControls()
        {
            Ready.Visibility = Visibility.Visible;
            measureViewModel.AIData.Clear();
            DataCount = 0;
        }

        //初始化
        private void InitMreasureRelatedVariables()
        {
            get_bp = default;
            Array.Clear(arrGetBpHandle, 0, arrGetBpHandle.Length);
            for (int i = 0; i < 4; i++)
                BP_ACK_Flag[i] = 0;
            //避免发送信息导致第二次不能测试
            PulseMrsFinishFlag = "No";
            Variable.Test_ABI_num = 0;
            Variable.Test_AI_num = 0;
            AICountOneSec = 0;
            AICountTwoSec = 0;
            //采集桡动脉
            RpRawDataNum = 0;
            RpRawData.Clear();
            NumOfBpMrs = 0;
            WaitForAck = MrsWaitForAck.MrsBP;
            enumPwvMrsResp = PWVMrsResp.PwvStop;
            if (NumOfBpMrs == 0)
            {
                Array.Clear(arrGetBpSave, 0, arrGetBpSave.Length);
            }
        }
        
        //血压测量
        private void Pic_test_Click(object sender, RoutedEventArgs e)
        {
            if (aiMeasurementActive) return;
            if (measureViewModel.IsRunning == "Visible")
            {
                Growl.Info("温馨提示：正在分析数据，请稍等！");
                return;
            }
         
            if (bPressMrsStart)
            {
                pulsedata = new PulseDataLocalEntity();//初始化测试数据实体
                InitMreasureRelatedControls();
                InitMreasureRelatedVariables();
                Initialize();
                //转到结束测试
                bPressMrsStart = false;
                if (Variable.Test_ABI_num == 0)
                {
                    workStatus = WorkStatus.StartBPtest;
                    BpTest_Function(0x01);
                }
                //由设置预定压力发送血压测量命令此处不需要
                workStatus = WorkStatus.StartBPtest;
                ChangeBtStyle(TestBtn, "BigGYBtnStyle", "结束测量");
                return;
            }
            else
            {
                workStatus = WorkStatus.NoWork;
                bPressMrsStart = true;
                WaitForAck = 0;
                //容错处理，3秒后有没有接收到停止响应应答，则提示出错
                Task.Delay(500);
                Timer_ABIDelay.Enabled = false;
                //发送停止命令
                if (!serialPortManager.SendData(CommandWord.REQ_BP_STOP, 0x05))
                {
                    return;
                }
                Task.Delay(300);
                serialPortManager.OpenControl(0x00);
                TimerBPTest.Stop();
                Task.Delay(500);
                ChangeBtStyle(TestBtn, "BigBlueBtnStyle", "开始测量");
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    measureViewModel.Tips = "温馨提示：测量停止！";
                }));
            }
        }
        //测量报告
        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            if (aiMeasurementActive) return;
            if (measureViewModel.IsRunning == "Visible")
            {
                Growl.Info("温馨提示：正在分析数据，请稍等！");
                return;
            }
            if (pulsedata.AI_num == 1)
            {
                UpdataToDatabase();
                this.NavigationService.Navigate(new OpenReportPage(pulsedata, ReportAction));
            }
            else
            {
                Growl.Info("心血管还未测试，请测试完成后再打开报表文件！");
                return;
            }
        }
        //心血管测量
        private void Pic_AI_test_Click(object sender, RoutedEventArgs e) //桡动脉测量开始
        {
            //workStatus = WorkStatus.NoWork;
            Variable.Test_ABI_num = 1;
            Ready.Visibility = Visibility.Hidden;
            if (measureViewModel.IsRunning == "Visible")
            {
                Growl.Info("温馨提示：正在分析数据，请稍等！");
                return;
            }
            if (workStatus == WorkStatus.NoWork && Variable.Test_ABI_num != 0)
            {
                AITestIntit();
                workStatus = WorkStatus.StartAItest;
                SetAiMeasurementActive(true);
                try
                {
                    if (!serialPortManager.SendData(CommandWord.REQ_PWV_START, 0x01))
                    {
                        workStatus = WorkStatus.NoWork;
                        SetAiMeasurementActive(false);
                        measureViewModel.Tips = "温馨提示：脉搏波测量启动失败，请检查设备连接！";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    workStatus = WorkStatus.NoWork;
                    SetAiMeasurementActive(false);
                    LogUtil.Error("启动心血管测试", ex.ToString());
                    measureViewModel.Tips = "温馨提示：脉搏波测量启动失败，请检查设备连接！";
                    return;
                }
                measureViewModel.Tips = "温馨提示：脉搏波测量开始！";
                ChangeBtStyle(AITest, "BigGYBtnStyle", "结束测量");
            }
            else if (workStatus == WorkStatus.StartAItest)
            {
                PulseMrsFinishFlag = "No";
                if (!serialPortManager.SendData(CommandWord.REQ_PWV_STOP, 0x01))
                {
                    measureViewModel.Tips = "温馨提示：停止指令发送失败，请检查设备连接！";
                    return;
                }
                workStatus = WorkStatus.NoWork;
                SetAiMeasurementActive(false);
                ChangeBtStyle(AITest, "BigBlueBtnStyle", "心血管测试");
                measureViewModel.Tips = "温馨提示：脉搏波测量结束！";
            }
            else
            {
                Growl.Info("当前有其他任务正在执行，请稍后测试心血管功能。");
            }
        }
        public void AITestIntit()
        {
            measureViewModel.AIData.Clear();
            RpRawData.Clear();
            DataCount = 0;
            RpRawDataNum = 0;
            nMarkWait = 0;
            nMarkSelect = 0;
            nCount = 0;
            AICountOneSec = 0;
            AICountTwoSec = 0;
            DataOneSec.Clear();
            DataTwoSec.Clear();
            measureViewModel.TestStatus = "0";
            ChangeBtStyle(AITest, "BigBlueBtnStyle", "心血管测试");
        }
        private void SetAiMeasurementActive(bool active)
        {
            aiMeasurementActive = active;
            void UpdateButtons()
            {
                // 使用当前状态，避免串口回调排队后把已经恢复的按钮再次禁用。
                bool enabled = !aiMeasurementActive;
                BackButton.IsEnabled = enabled;
                TestBtn.IsEnabled = enabled;
                OpenReport.IsEnabled = enabled;
                // AITest 保持可用，让用户可以主动结束测量。
            }

            if (Dispatcher.CheckAccess()) UpdateButtons();
            else Dispatcher.BeginInvoke((Action)UpdateButtons);
        }

        private void ChangeBtStyle(Button btn, string styleName, string content)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                var style = Application.Current.FindResource(styleName) as System.Windows.Style;
                btn.Style = style;
                btn.Content = content;
            }));
        }
    }
}
