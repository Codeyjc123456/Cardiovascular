using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Algorithm
{
    public class FeatureExtraction
    {
        #region  变量定义
        #endregion
        //函数15
        //计算心率
        public double ProcessRpRawData(List<double> RpRawData, int RpRawDataNum)
        {
            int LenRawData;
            LenRawData = RpRawData.Count - 1;
            //五点三次平滑滤波
            //List<double>  TempData = new List<double>();
            double[] TempData = new double[RpRawDataNum - 1];
            for (int i = 1; i <= 20; i++)
            {
                TempData[0] = (69 * RpRawData[0] + 4 * RpRawData[1] - 6 * RpRawData[2] + 4 * RpRawData[3] - RpRawData[4]) / 70.0;
                TempData[1] = (2 * RpRawData[0] + 27 * RpRawData[1] + 12 * RpRawData[2] - 8 * RpRawData[3] + 2 * RpRawData[4]) / 35.0;

                for (int j = 2; j < LenRawData - 2; j++)
                {
                    TempData[j] = (-3 * RpRawData[j - 2] + 12 * RpRawData[j - 1] + 17 * RpRawData[j] + 12 * RpRawData[j + 1] - 3 * RpRawData[j + 2]) / 35.0;
                }

                TempData[LenRawData - 2] = (2 * RpRawData[LenRawData - 4] - 8 * RpRawData[LenRawData - 3] + 12 * RpRawData[LenRawData - 2] + 27 * RpRawData[LenRawData - 1] + 2 * RpRawData[LenRawData]) / 35.0;
                TempData[LenRawData - 1] = (-1 * RpRawData[LenRawData - 4] + 4 * RpRawData[LenRawData - 3] - 6 * RpRawData[LenRawData - 2] + 4 * RpRawData[LenRawData - 1] + 69 * RpRawData[LenRawData]) / 70.0;

                for (int j = 0; j < LenRawData; j++)
                {
                    RpRawData[j] = TempData[j];
                }
            }

            //分段计算均值
            double[] SegmentMax = new double[10];
            double AvgSegmentMax = 0;

            for (int i = 0; i < 10; i++)
            {
                SegmentMax[i] = 0;
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = i * (LenRawData - 2) / 10; j <= (i + 1) * (LenRawData - 2) / 10; j++)
                {
                    if (RpRawData[j] > SegmentMax[i])
                    {
                        SegmentMax[i] = RpRawData[j];
                    }
                }
                AvgSegmentMax += SegmentMax[i];
            }

            AvgSegmentMax /= 10;
            //寻找峰值点位置
            int RpPeakPosNum = 0;
            int[] RpPeakPos = new int[0];

            for (int i = 5; i <= LenRawData - 5; i++)
            {
                if ((RpRawData[i] - RpRawData[i - 1] > 0) && (RpRawData[i] - RpRawData[i + 1] > 0) && (RpRawData[i] > 0.95 * AvgSegmentMax))
                {
                    if (RpPeakPosNum == 0)
                    {
                        Array.Resize(ref RpPeakPos, RpPeakPosNum + 1);
                        RpPeakPos[RpPeakPosNum] = i;
                        RpPeakPosNum++;
                    }
                    else
                    {
                        // 两极值点间隔不能小于48个点即0.2秒=300次/分
                        if (i - RpPeakPos[RpPeakPosNum - 1] > 40)
                        {
                            Array.Resize(ref RpPeakPos, RpPeakPosNum + 1);
                            RpPeakPos[RpPeakPosNum] = i;
                            RpPeakPosNum++;
                        }
                    }
                }
            }
            RpPeakPosNum--;
            //计算周期
            float[] RpPeriod = new float[0];
            int RpPeriodNum = 0;
            float MeanPeriod = 0;

            for (int i = 0; i < RpPeakPos.Length - 1; i++)
            {
                Array.Resize(ref RpPeriod, RpPeriodNum + 1);
                RpPeriod[RpPeriodNum] = (float)(RpPeakPos[i + 1] - RpPeakPos[i]);
                MeanPeriod += RpPeakPos[i + 1] - RpPeakPos[i];
                RpPeriodNum++;
            }
            //去除一个最大值和一个最小值
            int MaxPeriod = (int)RpPeriod.Max();
            int MinPeriod = (int)RpPeriod.Min();

            int HeartRate = 0;
            if (RpPeriodNum >= 3)
            {
                HeartRate = (int)((MeanPeriod - MaxPeriod - MinPeriod) / (RpPeriodNum - 2));
            }
            else
            {
                HeartRate = (int)(MeanPeriod / RpPeriodNum);
            }

            return HeartRate;
        }
    }
    public class AcquireData
    {
        private int nMarkSelect;
        public int SelectWaveform(List<double> Data, int Sample_Rate)
        {
            int SelectWaveformBack = 0;
            double MaxValue;
            double MinValue;
            int nUpper;
            List<double> PulseData = new List<double>();
            MaxValue = Data.Max();
            MinValue = Data.Min();
            if (MaxValue < 600)
            {
                SelectWaveformBack = 0;
                return SelectWaveformBack;
            }
            //nLower = 0;
            nUpper = Data.Count - 1;
            PulseData.Clear();
            //把data（脉搏波）中的值存到pulsedata中
            for (int i = 0; i <= nUpper; i++)
            {
                PulseData.Add(Data[i]);
            }
            double[] TempData = new double[nUpper + 1];
            for (int i = 1; i <= 24; i++)
            {
                TempData[0] = (double)((Int64)(69 * PulseData[0]) + (Int64)(4 * PulseData[1]) - (Int64)(6 * PulseData[2]) + (Int64)(4 * PulseData[3]) - PulseData[4]) / 70;
                TempData[1] = (double)((Int64)(2 * PulseData[0]) + (Int64)(27 * PulseData[1]) + (Int64)(12 * PulseData[2]) - (Int64)(8 * PulseData[3]) + 2 * PulseData[4]) / 35;
                for (int j = 2; j <= nUpper - 2; j++)
                {
                    TempData[j] = (double)((-3 * PulseData[j - 2]) + (Int64)(12 * PulseData[j - 1]) + (Int64)(17 * PulseData[j]) + (Int64)(12 * PulseData[j + 1]) - (3 * PulseData[j + 2])) / 35;
                }
                TempData[nUpper - 1] = (double)((2 * PulseData[nUpper - 4]) - (Int64)(8 * PulseData[nUpper - 3]) + (Int64)(12 * PulseData[nUpper - 2]) + (Int64)(27 * PulseData[nUpper - 1]) + (2 * PulseData[nUpper])) / 35;
                TempData[nUpper] = (double)((-1 * PulseData[nUpper - 4]) + (Int64)(4 * PulseData[nUpper - 3]) - (Int64)(6 * PulseData[nUpper - 2]) + (4 * PulseData[nUpper - 1]) + (Int64)(69 * PulseData[nUpper])) / 70;
                for (int j = 0; j <= nUpper; j++)
                {
                    PulseData[j] = (double)TempData[j];
                }
            }//for i
            double[] Dif = new double[3] { 0, 0, 0 };
            double AvgDif = 0;
            List<double> DifSig = new List<double>();
            List<double> DifSigMaxPointPos = new List<double>();

            DifSig.Clear();
            DifSig.Add(0);
            DifSig.Add(0);
            for (int i = 2; i <= nUpper - 2; i++)
            {
                DifSig.Add(PulseData[i + 1] - PulseData[i - 1] + 2 * (PulseData[i + 2] - PulseData[i - 2]));
            }
            for (int i = 0; i <= 2; i++)
            {
                for (int j = (int)(i * (nUpper - 2) / 3); j <= (int)((i + 1) * (nUpper - 2) / 3); j++)
                {
                    if (DifSig[j] > Dif[i])
                    {
                        Dif[i] = DifSig[j];
                    }
                }
                AvgDif = AvgDif + Dif[i];
            }
            AvgDif = AvgDif / 3;
            int DifSigMaxPointPosNum = 0;
            DifSigMaxPointPos.Clear();
            for (int i = 1; i <= nUpper - 3; i++)
            {
                if ((DifSig[i] - DifSig[i - 1] > 0) && (DifSig[i] - DifSig[i + 1] > 0) && (DifSig[i] > 0.7 * AvgDif))
                {
                    if (DifSigMaxPointPosNum == 0)
                    {
                        DifSigMaxPointPos.Add(i);
                        DifSigMaxPointPosNum = DifSigMaxPointPosNum + 1;
                    }
                    else
                    {
                        if (i - DifSigMaxPointPos[DifSigMaxPointPosNum - 1] > 0.4 * Sample_Rate / 2)
                        {
                            DifSigMaxPointPos.Add(i);
                            DifSigMaxPointPosNum = DifSigMaxPointPosNum + 1;
                        }
                    }
                }
            }//for i
            if (DifSigMaxPointPosNum < 2)
            {
                SelectWaveformBack = 0;
                return SelectWaveformBack;
            }
            int NumOfPtoN = 0;
            for (int i = 1; i <= nUpper - 3; i++)
            {
                if ((DifSig[i - 1] > 0) && (DifSig[i] <= 0))
                {
                    NumOfPtoN = NumOfPtoN + 1;
                }
            }
            int NumOfError = 0;
            for (int i = 1; i <= DifSigMaxPointPosNum - 1; i++)
            {
                if ((DifSigMaxPointPos[i] - DifSigMaxPointPos[i - 1] > 1.425 * Sample_Rate / 2) || (DifSigMaxPointPos[i] - DifSigMaxPointPos[i - 1] < 0.4 * Sample_Rate / 2) || (NumOfError > 0))
                {
                    NumOfError = NumOfError + 1;
                }
            }
            double TempDataPeriod = 0;
            if (DifSigMaxPointPosNum >= 2)
            {
                TempDataPeriod = DifSigMaxPointPos[1] - DifSigMaxPointPos[0];
            }
            if (TempDataPeriod < 0.25 * Sample_Rate / 2)
            {
                SelectWaveformBack = 0;
                return SelectWaveformBack;
            }
            nMarkSelect = 0;
            if (NumOfError == 0)
            {
                if ((NumOfPtoN > 0) && (NumOfPtoN <= 20) && (TempDataPeriod > 1.425 * Sample_Rate / 2))
                {
                    nMarkSelect = 1;
                }
                if ((NumOfPtoN > 0) && (NumOfPtoN <= 25) && (TempDataPeriod > 50) && (TempDataPeriod <= 1.425 * Sample_Rate / 2))
                {
                    nMarkSelect = 1;
                }
            }
            if (nMarkSelect == 1)
            {
                SelectWaveformBack = 1;
            }
            else
            {
                SelectWaveformBack = 0;
            }
            return SelectWaveformBack;
        }
    }
    public class DiagnosisResult
    {
        #region 常量定义

        private const string SEPARATOR = "；";
        private const string NORMAL_DIAGNOSIS = "心脏供血能力未见异常；动脉弹性未见异常；";

        // 心率阈值
        private const int HR_BRADY_MIN = 1;
        private const int HR_BRADY_MAX = 54;
        private const int HR_TACHY_MIN = 100;
        private const int HR_TACHY_MAX = 200;

        // 血压阈值
        private const int BP_LOW_SBP = 90;
        private const int BP_LOW_DBP = 60;
        private const int BP_NORMAL_HIGH_SBP_MIN = 130;
        private const int BP_NORMAL_HIGH_SBP_MAX = 139;
        private const int BP_NORMAL_HIGH_DBP_MAX = 89;
        private const int BP_NORMAL_HIGH_DBP_MIN = 85;

        // 高血压阈值
        private const int HYPERTENSION_SBP = 140;
        private const int HYPERTENSION_DBP = 90;
        private const int HYPERTENSION_LEVEL1_SBP_MAX = 159;
        private const int HYPERTENSION_LEVEL1_DBP_MAX = 99;
        private const int HYPERTENSION_LEVEL2_SBP = 160;
        private const int HYPERTENSION_LEVEL2_DBP = 100;

        // 脉压阈值
        private const int PP_LOW_MAX = 27;
        private const int PP_HIGH_MIN = 55;
        private const int PP_MAX = 200;

        // 心肌缺血阈值
        private const double SEVR_SEVERE_MAX = 0.8;
        private const double SEVR_RISK_MAX = 0.9;
        private const double SEVR_NORMAL_MIN = 0.9;

        // 心脏时间阈值
        private const double ED_SYSTOLE_MAX = 0.2;
        private const double ED_DIASTOLE_MIN = 0.4;
        private const double ED_PCT_SYSTOLE_MAX = 0.3;
        private const double ED_PCT_DIASTOLE_MIN = 0.48;

        // AIx 阈值
        private const double AIX_ELASTICITY_THRESHOLD = 0.75;
        private const double AIX_ARTERIOSCLEROSIS_MIN = 0.8;
        private const double AIX_EARLY_MIN = 0.9;

        // 心脏功能正常范围
        private const int HR_NORMAL_MIN = 60;
        private const int HR_NORMAL_MAX = 99;
        private const int DPTI_NORMAL_MIN = 2300;
        private const int DPTI_NORMAL_MAX = 3500;
        private const int SPTI_NORMAL_MIN = 1800;
        private const int SPTI_NORMAL_MAX = 2500;
        private const double ED_PCT_NORMAL_MIN = 0.3;
        private const double ED_PCT_NORMAL_MAX = 0.45;

        // 血管功能正常范围
        private const int BP_NORMAL_SBP_MIN = 90;
        private const int BP_NORMAL_SBP_MAX = 130;
        private const int BP_NORMAL_DBP_MIN = 60;
        private const int BP_NORMAL_DBP_MAX = 90;
        private const int PP_NORMAL_MIN = 30;
        private const int PP_NORMAL_MAX = 45;
        private const int CAP_NORMAL_MIN = 85;
        private const int CAP_NORMAL_MAX = 110;

        #endregion

        #region 数据结构

        public struct CardiacIndex
        {
            public int Hr;
            public double Ed;
            public int SBp;
            public int DBp;
            public int Pp;
            public int Cap;
            public int Sbp2;
            public double AIx;
            public double EdPct;
            public int Spti;
            public int Dpti;
            public double Sevr;
        }

        public struct VascularIndex
        {
            public double Ed;
            public int Sbp;
            public int Dbp;
            public int Pp;
            public int Sbp2;
            public double AIx;
            public int Cap;
        }
        #endregion
        #region 公共字段
        public string strDiagnosisResult = "";
        public string strDiagnosisProposal = "";
        public string g_strABIAssess = "";
        public string g_strPWVAssess = "";
        public string g_strLefttoRightPWV = "";

        #endregion

        #region 私有字段

        private CardiacIndex _cardiacIndex;
        private VascularIndex _vascularIndex;

        #endregion

        #region 主入口

        public void AnalyseAIDiagnosisResult(int age, string sex, List<double> cardiacIndex, List<double> vascularIndex)
        {
            InitializeIndices(cardiacIndex, vascularIndex);
            var result = new DiagnosisResultBuilder(SEPARATOR);
            // 心率诊断
            result.Add(GetHeartRateDiagnosis());
            // 心肌耗氧量
            result.Add(GetMyocardialOxygenDiagnosis());
            // 中心动脉压
            result.Add(GetCentralArterialPressureDiagnosis());
            // 血压诊断
            var bpResult = GetBloodPressureDiagnosis();
            result.Add(bpResult.Diagnosis);
            result.AddProposal(bpResult.Proposal);
            // 脉压诊断
            result.Add(GetPulsePressureDiagnosis());
            // SEVR 诊断
            result.Add(GetSevrDiagnosis(sex));
            // AIx 诊断
            result.Add(GetAixDiagnosis(sex, age));
            // 补充正常诊断
            AddNormalDiagnosisIfNeeded(ref result, sex, age);
            // 指导建议
            strDiagnosisProposal = GetFinalProposal();
            strDiagnosisResult = result.Build() ?? NORMAL_DIAGNOSIS;
        }
        #endregion
        #region 初始化
        private void InitializeIndices(List<double> cardiac, List<double> vascular)
        {
            _cardiacIndex = new CardiacIndex
            {
                Hr = Convert.ToInt16(cardiac[0]),
                Ed = Convert.ToSingle(cardiac[1]),
                EdPct = Convert.ToSingle(cardiac[2]),
                Spti = Convert.ToInt16(cardiac[3]),
                Dpti = Convert.ToInt16(cardiac[4]),
                Sevr = cardiac[5]
            };
            _vascularIndex = new VascularIndex
            {
                Sbp = Convert.ToInt16(vascular[0]),
                Dbp = Convert.ToInt16(vascular[1]),
                Pp = Convert.ToInt16(vascular[2]),
                Cap = Convert.ToInt16(vascular[3]),
                AIx = vascular[4]
            };
        }
        #endregion
        #region 诊断方法
        private string GetHeartRateDiagnosis()
        {
            return _cardiacIndex.Hr switch
            {
                >= HR_BRADY_MIN and <= HR_BRADY_MAX => "心动过缓",
                >= HR_TACHY_MIN and <= HR_TACHY_MAX => "心动过快",
                _ => ""
            };
        }
        private string GetMyocardialOxygenDiagnosis()
        {
            return _cardiacIndex.Spti > 2750 ? "心肌耗氧量增加" : "";
        }

        private string GetCentralArterialPressureDiagnosis()
        {
            return _vascularIndex.Cap is >= 121 and <= 200 ? "中心动脉压较高" : "";
        }
        private (string Diagnosis, string Proposal) GetBloodPressureDiagnosis()
        {
            int sbp = _vascularIndex.Sbp;
            int dbp = _vascularIndex.Dbp;

            // 血压偏低
            if (sbp < BP_LOW_SBP || dbp < BP_LOW_DBP)
            {
                string diagnosis = "血压偏低";
                string proposal = "";

                if (_cardiacIndex.Spti < 1500)
                {
                    diagnosis += "；外周血氧供应不足可能增加";
                    proposal = "多食富含造血原料的食物及蔬菜水果";
                }
                return (diagnosis, proposal);
            }

            // 外周血压正常高值
            bool isNormalHigh = (sbp >= BP_NORMAL_HIGH_SBP_MIN && sbp < BP_NORMAL_HIGH_SBP_MAX && dbp < BP_NORMAL_HIGH_DBP_MAX) ||
                                (dbp > BP_NORMAL_HIGH_DBP_MIN && dbp < BP_NORMAL_HIGH_DBP_MAX && sbp < BP_NORMAL_HIGH_SBP_MAX);
            if (isNormalHigh)
                return ("外周血压正常高值", "请经常关注您的血压");
            // 高血压
            if (sbp >= HYPERTENSION_SBP || dbp >= HYPERTENSION_DBP)
                return GetHypertensionDiagnosis();
            return ("", "");
        }
        private (string Diagnosis, string Proposal) GetHypertensionDiagnosis()
        {
            int sbp = _vascularIndex.Sbp;
            int dbp = _vascularIndex.Dbp;
            string diagnosis = "";
            string proposal = "多食新鲜蔬菜；多食含钾、钙丰富而含钠低的食物";

            if ((sbp >= HYPERTENSION_SBP && sbp < HYPERTENSION_LEVEL1_SBP_MAX) ||
                (dbp >= HYPERTENSION_DBP && dbp < HYPERTENSION_LEVEL1_DBP_MAX))
            {
                diagnosis = "外周血压较高";
            }
            else if (sbp >= HYPERTENSION_LEVEL2_SBP || dbp >= HYPERTENSION_LEVEL2_DBP)
            {
                diagnosis = "外周血压很高";
                if (_cardiacIndex.Dpti > 4000)
                    diagnosis += "；代偿性心肌灌注增加";
            }
            return (diagnosis, proposal);
        }
        private string GetPulsePressureDiagnosis()
        {
            int pp = _vascularIndex.Pp;
            return pp switch
            {
                >= 0 and <= PP_LOW_MAX => "脉压减小",
                >= PP_HIGH_MIN and <= PP_MAX => "脉压增大",
                _ => ""
            };
        }
        private string GetSevrDiagnosis(string sex)
        {
            double sevr = _cardiacIndex.Sevr;
            string diagnosis = "";

            if (sevr <= SEVR_SEVERE_MAX)
                diagnosis = "可能存在心肌缺血";
            else if (sevr <= SEVR_RISK_MAX)
                diagnosis = "心肌缺血风险增加";
            else if (sevr < SEVR_NORMAL_MIN && sex == "男")
                diagnosis = "无明显心肌缺血";

            string cardiacTimeDiagnosis = GetCardiacTimeDiagnosis();
            return string.IsNullOrEmpty(cardiacTimeDiagnosis) ? diagnosis : $"{diagnosis}{SEPARATOR}{cardiacTimeDiagnosis}";
        }
        private string GetCardiacTimeDiagnosis()
        {
            double ed = _cardiacIndex.Ed;
            double edPct = _cardiacIndex.EdPct;

            if (ed < ED_SYSTOLE_MAX || edPct < ED_PCT_SYSTOLE_MAX)
                return "心脏收缩时间较短";

            if (ed > ED_DIASTOLE_MIN || edPct > ED_PCT_DIASTOLE_MIN)
                return "心脏舒张时间较短";

            return "";
        }
        private string GetAixDiagnosis(string sex, int age)
        {
            double aix = _vascularIndex.AIx;

            var thresholds = GetAixThresholds(sex, age);
            foreach (var (min, max, diagnosis) in thresholds)
            {
                if (aix > min && aix <= max)
                    return diagnosis;
            }
            return "";
        }
        private List<(double Min, double Max, string Diagnosis)> GetAixThresholds(string sex, int age)
        {
            // 男性 ≤ 40岁
            if (sex == "男" && age <= 40)
            {
                return new List<(double, double, string)>
            {
                (AIX_ELASTICITY_THRESHOLD, AIX_ARTERIOSCLEROSIS_MIN, "动脉弹性下降"),
                (AIX_ARTERIOSCLEROSIS_MIN, AIX_EARLY_MIN, "有动脉硬化迹象"),
                (AIX_EARLY_MIN, double.MaxValue, "有动脉硬化迹象")
            };
            }

            // 女性 ≤ 40岁 或 男性 40-60岁
            if ((sex == "女" && age <= 40) || (sex == "男" && age > 40 && age < 60))
            {
                return new List<(double, double, string)>
            {
                (AIX_ARTERIOSCLEROSIS_MIN, 0.85, "有动脉硬化迹象"),
                (0.85, AIX_EARLY_MIN, "有动脉硬化迹象"),
                (AIX_EARLY_MIN, double.MaxValue, "动脉硬化早期")
            };
            }

            // 女性 40-60岁
            if (sex == "女" && age > 40 && age < 60)
            {
                return new List<(double, double, string)>
            {
                (0.85, AIX_EARLY_MIN, "有动脉硬化迹象"),
                (AIX_EARLY_MIN, double.MaxValue, "动脉硬化早期")
            };
            }

            // 年龄 ≥ 60岁
            return new List<(double, double, string)>
        {
            (0, AIX_EARLY_MIN, "有动脉硬化迹象"),
            (AIX_EARLY_MIN, double.MaxValue, "动脉硬化早期")
        };
        }
        #endregion
        #region 正常诊断补充
        private void AddNormalDiagnosisIfNeeded(ref DiagnosisResultBuilder result, string sex, int age)
        {
            if (string.IsNullOrEmpty(strDiagnosisResult)) return;

            if (IsCardiacNormal())
                result.Add("心脏供血能力未见异常");

            if (IsVascularNormal(sex, age))
                result.Add("动脉弹性未见异常");
        }
        private bool IsCardiacNormal()
        {
            return _cardiacIndex.Hr >= HR_NORMAL_MIN && _cardiacIndex.Hr <= HR_NORMAL_MAX &&
                   _cardiacIndex.Sevr > 1 &&
                   _cardiacIndex.Dpti >= DPTI_NORMAL_MIN && _cardiacIndex.Dpti <= DPTI_NORMAL_MAX &&
                   _cardiacIndex.Spti >= SPTI_NORMAL_MIN && _cardiacIndex.Spti <= SPTI_NORMAL_MAX &&
                   _cardiacIndex.EdPct >= ED_PCT_NORMAL_MIN && _cardiacIndex.EdPct <= ED_PCT_NORMAL_MAX;
        }
        private bool IsVascularNormal(string sex, int age)
        {
            bool bpNormal = _vascularIndex.Sbp >= BP_NORMAL_SBP_MIN && _vascularIndex.Sbp <= BP_NORMAL_SBP_MAX &&
                            _vascularIndex.Dbp >= BP_NORMAL_DBP_MIN && _vascularIndex.Dbp <= BP_NORMAL_DBP_MAX;

            bool ppNormal = _vascularIndex.Pp >= PP_NORMAL_MIN && _vascularIndex.Pp <= PP_NORMAL_MAX;
            bool capNormal = _vascularIndex.Cap >= CAP_NORMAL_MIN && _vascularIndex.Cap <= CAP_NORMAL_MAX;

            return bpNormal && ppNormal && capNormal && IsAixNormal(sex, age);
        }
        private bool IsAixNormal(string sex, int age)
        {
            double aix = _vascularIndex.AIx;

            return (sex == "男" && age <= 40 && aix <= AIX_ELASTICITY_THRESHOLD) ||
                   (sex == "女" && age <= 40 && aix <= AIX_ARTERIOSCLEROSIS_MIN) ||
                   (sex == "男" && age > 40 && age < 60 && aix <= AIX_ARTERIOSCLEROSIS_MIN) ||
                   (sex == "女" && age > 40 && age < 60 && aix <= 0.85);
        }
        #endregion
        #region 指导建议
        private string GetFinalProposal()
        {
            if (_cardiacIndex.Hr > HR_TACHY_MIN || _cardiacIndex.Hr <= HR_BRADY_MAX)
                return "请静息后重测或进行心电图检测";

            return string.IsNullOrEmpty(strDiagnosisProposal)
                ? "多进行有氧运动（如慢跑等）有益您的心血管健康，具体运动量请听从医师指导。"
                : strDiagnosisProposal;
        }
        #endregion
        #region 辅助类
        private class DiagnosisResultBuilder
        {
            private readonly List<string> _diagnoses = new();
            private readonly string _separator;
            public string Proposal { get; set; } = "";

            public DiagnosisResultBuilder(string separator)
            {
                _separator = separator;
            }
            public void Add(string diagnosis)
            {
                if (!string.IsNullOrEmpty(diagnosis))
                    _diagnoses.Add(diagnosis);
            }
            public void AddProposal(string proposal)
            {
                if (!string.IsNullOrEmpty(proposal))
                    Proposal = string.IsNullOrEmpty(Proposal) ? proposal : $"{Proposal}{_separator}{proposal}";
            }
            public string? Build()
            {
                return _diagnoses.Count == 0 ? null : string.Join(_separator, _diagnoses);
            }
        }
        #endregion
    }
}
