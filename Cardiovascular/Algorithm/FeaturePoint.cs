
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Algorithm
{
    public class FeaturePoint
    {
        // 返回的参数
        public string[,] index = new string[2, 8];
        public object IndexValue;
        public object varFootPos;
        public object varPeakPos;
        public object varEPointPos;
        public object varFPointPos;
        public object varGPointPos;
        public object varCalibration;

        // 各项指标值
        public int Hr;           // 心率
        public double Ed;        // 射血时间
        public double EdPct;     // 射血时间占整个心动周期的百分比
        public int Spti;         // 左心负荷
        public int Dpti;         // 心肌灌注
        public double Sevr;      // 心内膜下心肌活力率
        public double Ai;        // 增长指数
        public int Cap;          // 中心动脉压

        // 特征点位置（波形下标）
        private List<int> _footPos = [];
        private List<int> _peakPos = [];
        private List<int> _ePointPos = [];
        private List<int> _fPointPos = [];
        private List<int> _gPointPos = [];

        // 特征点与起始点距离
        private List<int> _fPointToFootDis = [];

        // 波形数据
        private List<double> _pulseData = [];
        private List<double> _difSig = [];
        private List<double> _curvature = [];
        private List<double> _calibration = [];
        private List<double> _normalization = [];
        // 差分信号极大值点
        private List<int> _difSigMaxPointPos = [];
        // 周期
        private List<int> _period = [];
        // 配置参数
        private const int SEGMENT_NUM = 7;
        private const int SCALE1 = 25;
        private const int SCALE2 = 50;
        private const int SAMPLE_RATE = 500; // 默认采样率

        private int _getSbp;
        private int _getDbp;

        #region 主入口
        public int Identify(int sbp, int dbp, List<double> arrTemp)
        {
            _getSbp = sbp;
            _getDbp = dbp;
            if (sbp is < 40 or > 200 || dbp is < 40 or > 200 || arrTemp.Count < 1000)
                return 0;
            _pulseData = arrTemp;
            SmoothData();
            SearchDifSigMaxPointPos();
            SearchFootPointPos();
            if (_footPos.Count == 0) return 0;
            // 计算平均周期
            _period.Clear();
            for (int i = 1; i < _footPos.Count; i++)
                _period.Add(_footPos[i] - _footPos[i - 1]);

            float avgPeriod = (float)_period.Average();
            if (avgPeriod < 100) return 0;

            SearchPeakPointPos();
            CalculateUpstrokeTime();
            RemoveBaselineWanderAndCalibrateWaveform(_getSbp, _getDbp);
            SearchDicroticPointPos();//重搏波G点
            SearchDicroticNotchPointPos();//寻找重搏波切迹
            //F点到起始点的距离
            _fPointToFootDis = _fPointPos.Zip(_footPos, (f, foot) => f - foot).ToList();
            SearchReflectionPointPos();//寻找反射点E点
            CalculateIndexValue();
            CorrectAiByHeartRate();
            ClampIndexValues();
            BuildResultArray();

            return 1;
        }

        #endregion
        #region 滤波
        private void SmoothData()
        {
            int counter = 12 * SAMPLE_RATE - 13;
            var tempData = new List<double>();
            for (int iter = 1; iter <= 24; iter++)
            {
                tempData.Clear();
                tempData.Add((69 * _pulseData[0] + 4 * _pulseData[1] - 6 * _pulseData[2] + 4 * _pulseData[3] - _pulseData[4]) / 70);
                tempData.Add((2 * _pulseData[0] + 27 * _pulseData[1] + 12 * _pulseData[2] - 8 * _pulseData[3] + 2 * _pulseData[4]) / 35);

                for (int j = 2; j <= counter - 2; j++)
                {
                    tempData.Add((-3 * _pulseData[j - 2] + 12 * _pulseData[j - 1] + 17 * _pulseData[j] + 12 * _pulseData[j + 1] - 3 * _pulseData[j + 2]) / 35);
                }

                tempData.Add((2 * _pulseData[counter - 4] - 8 * _pulseData[counter - 3] + 12 * _pulseData[counter - 2] + 27 * _pulseData[counter - 1] + 2 * _pulseData[counter]) / 35);
                tempData.Add((-_pulseData[counter - 4] + 4 * _pulseData[counter - 3] - 6 * _pulseData[counter - 2] + 4 * _pulseData[counter - 1] + 69 * _pulseData[counter]) / 70);

                for (int j = 0; j <= counter; j++)
                    _pulseData[j] = tempData[j];
            }
        }
        #endregion
        #region 差分信号极大值点
        private void SearchDifSigMaxPointPos()
        {
            int counter = 12 * SAMPLE_RATE - 13;
            var firstDrt = new List<double>();
            var denominator = new List<double>();
            // 一阶导数
            for (int i = 1; i <= counter; i++)
            {
                firstDrt.Add(_pulseData[i] - _pulseData[i - 1]);
                double diff = _pulseData[i] - _pulseData[i - 1];
                denominator.Add(Math.Pow(diff * diff + 1, 1.5));
            }
            // 二阶导数（曲率分子）
            var numerator = Enumerable.Range(1, counter - 1).Select(i => Math.Abs(firstDrt[i] - firstDrt[i - 1])).ToList();
            // 曲率
            _curvature = Enumerable.Range(0, counter - 1).Select(i => numerator[i] / denominator[i]).ToList();
            // 差分信号
            _difSig = new List<double> { 0, 0 };
            for (int i = 2; i <= counter - 2; i++)
                _difSig.Add(_pulseData[i + 1] - _pulseData[i - 1] + 2 * (_pulseData[i - 2] - _pulseData[i - 2])); // 注意：原代码可能有误，保持原样
            _difSig.Add(0);
            _difSig.Add(0);
            // 分段计算阈值
            int dataLen = _difSig.Count;
            var segmentMax = new double[SEGMENT_NUM + 1];
            for (int i = 0; i <= SEGMENT_NUM; i++)
            {
                int start = i * dataLen / 8;
                int end = (i + 1) * dataLen / 8;
                segmentMax[i] = _difSig.Skip(start).Take(end - start).Max();
            }
            double threshold = segmentMax.Average() * 0.6;
            // 寻找极大值点
            _difSigMaxPointPos.Clear();
            for (int i = 5; i <= counter - 5; i++)
            {
                if (_difSig[i] > _difSig[i - 1] && _difSig[i] > _difSig[i + 1] && _difSig[i] > threshold)
                {
                    if (_difSigMaxPointPos.Count == 0 || i - _difSigMaxPointPos[^1] > 0.4 * SAMPLE_RATE)
                        _difSigMaxPointPos.Add(i);
                }
            }
        }
        #endregion
        #region 起始点
        private void SearchFootPointPos()
        {
            if (_difSigMaxPointPos.Count < 2) return;

            // 计算平均周期
            var tempPeriod = Enumerable.Range(0, _difSigMaxPointPos.Count - 1)
                .Select(i => _difSigMaxPointPos[i + 1] - _difSigMaxPointPos[i]).ToList();
            int avgPeriod = (int)tempPeriod.Average();

            _footPos.Clear();
            for (int i = 0; i < _difSigMaxPointPos.Count; i++)
            {
                int searchStart = i == 0 ? 1 : _difSigMaxPointPos[i] - (int)(0.1 * SAMPLE_RATE);
                if (searchStart < 1) continue;

                int foot = -1;
                for (int j = _difSigMaxPointPos[i]; j >= searchStart; j--)
                {
                    if (j + 1 < _difSig.Count && j - 1 >= 0 && _difSig[j + 1] > 0 && _difSig[j - 1] < 0)
                    {
                        foot = j;
                        break;
                    }
                }

                if (foot == -1 && _difSigMaxPointPos[i] > 0.077 * avgPeriod)
                    foot = _difSigMaxPointPos[i] - (int)(0.077 * avgPeriod);

                if (foot > 0) _footPos.Add(foot);
            }
            if (_footPos.Count < 2) return;
            // 处理最后一个波形
            int lastIdx = _footPos.Count - 1;
            int lastPeriod = _pulseData.Count - _footPos[lastIdx];
            int prevPeriod = _footPos[lastIdx] - _footPos[lastIdx - 1];

            if (lastPeriod > 1.05 * prevPeriod)
            {
                for (int i = _pulseData.Count - 2; i >= _footPos[lastIdx] - 1; i--)
                {
                    if (_difSig[i] >= 0 && _difSig[i - 1] < 0)
                    {
                        _footPos.Add(i);
                        break;
                    }
                }
            }
        }
        #endregion
        #region 峰值点
        private void SearchPeakPointPos()
        {
            _peakPos.Clear();
            for (int i = 0; i < _footPos.Count - 1; i++)
            {
                for (int j = _footPos[i]; j < _footPos[i + 1]; j++)
                {
                    if (j + 1 < _difSig.Count && _difSig[j] >= 0 && _difSig[j + 1] < 0)
                    {
                        int peak = FindMaxInRange(j, j + 38, _pulseData);
                        _peakPos.Add(peak);
                        break;
                    }
                }
            }
        }
        private static int FindMaxInRange(int start, int end, List<double> data)
        {
            int maxPos = start;
            double maxVal = data[start];
            for (int k = start + 1; k <= end && k < data.Count; k++)
            {
                if (data[k] > maxVal)
                {
                    maxVal = data[k];
                    maxPos = k;
                }
            }
            return maxPos;
        }
        private void CalculateUpstrokeTime()
        {
            if (_peakPos.Count == 0 || _footPos.Count == 0) return;

            var upstrokeTime = new List<int>();
            if (_peakPos[0] < _footPos[0])
            {
                for (int i = 0; i < _peakPos.Count - 1; i++)
                    upstrokeTime.Add(_peakPos[i + 1] - _footPos[i]);
            }
            else
            {
                for (int i = 0; i < _peakPos.Count; i++)
                    upstrokeTime.Add(_peakPos[i] - _footPos[i]);
            }
        }
        #endregion
        #region 重搏波点
        private void SearchDicroticPointPos()
        {
            _gPointPos.Clear();
            int sampleRate = SAMPLE_RATE;

            for (int i = 0; i < _footPos.Count - 1; i++)
            {
                int period = _period[i];
                var (startRatio, endRatio) = (double)period switch
                {
                    < 125 * SAMPLE_RATE / 200 => (0.43, 0.75),
                    > 185 * SAMPLE_RATE / 200 => (0.34, 0.58),
                    _ => (0.38, 0.62)
                };

                int start = _footPos[i] + (int)(startRatio * period);
                int end = _footPos[i] + (int)(endRatio * period);
                int gPoint = FindGPoint(start, end, i);

                if (gPoint > 0)
                    _gPointPos.Add(gPoint);
                else
                    _gPointPos.Add((int)(i > 0 ? _gPointPos[i - 1] + _period[i - 1] : _footPos[0] + 100 * sampleRate / 200));
            }
        }
        private int FindGPoint(int start, int end, int cycleIndex)
        {
            int maxCurvaturePos = 0;
            double maxCurvature = 0;

            for (int j = start; j <= end && j < _difSig.Count - 1; j++)
            {
                if (_difSig[j - 1] >= 0 && _difSig[j + 1] < 0)
                {
                    return FindMaxInRange(j, j + 38, _pulseData);
                }
                if (j < _curvature.Count && _curvature[j] > maxCurvature)
                {
                    maxCurvature = _curvature[j];
                    maxCurvaturePos = j;
                }
            }

            if (maxCurvaturePos > 0 && maxCurvaturePos > _footPos[cycleIndex] + (int)(0.58 * _period[cycleIndex]))
                return maxCurvaturePos;

            return _footPos[cycleIndex] + (int)(0.55 * _period[cycleIndex]);
        }
        private void SearchDicroticNotchPointPos()
        {
            _fPointPos.Clear();

            for (int i = 0; i < _footPos.Count - 1; i++)
            {
                int period = _period[i];
                var (startRatio, endRatio) = period switch
                {
                    < 125 * SAMPLE_RATE / 200 => (0.36, 0.52),
                    > 185 * SAMPLE_RATE / 200 => (0.28, 0.45),
                    _ => (0.32, 0.48)
                };

                int start = _footPos[i] + (int)(startRatio * period);
                int end = _footPos[i] + (int)(endRatio * period);
                int fPoint = FindFPoint(start, end, i);

                if (fPoint > 0)
                    _fPointPos.Add(fPoint);
                else
                    _fPointPos.Add(i > 0 ? _fPointPos[i - 1] + _period[i - 1] : _footPos[0] + 80 * SAMPLE_RATE / 200);
            }
        }
        private int FindFPoint(int start, int end, int cycleIndex)
        {
            int maxCurvaturePos = 0;
            double maxCurvature = 0;

            for (int j = start; j <= end && j < _difSig.Count - 1; j++)
            {
                if (_difSig[j - 1] < 0 && _difSig[j + 1] >= 0)
                {
                    return FindMinInRange(j - 8, j + 8, _pulseData);
                }
                if (j < _curvature.Count && _curvature[j] > maxCurvature)
                {
                    maxCurvature = _curvature[j];
                    maxCurvaturePos = j;
                }
            }

            return maxCurvaturePos;
        }
        private static int FindMinInRange(int start, int end, List<double> data)
        {
            start = Math.Max(0, start);
            end = Math.Min(end, data.Count - 1);
            int minPos = start;
            double minVal = data[start];
            for (int k = start + 1; k <= end; k++)
            {
                if (data[k] < minVal)
                {
                    minVal = data[k];
                    minPos = k;
                }
            }
            return minPos;
        }
        #endregion
        #region 反射点
        private void SearchReflectionPointPos()
        {
            // 简化：使用卷积找过零点
            int num = _footPos[^1] - _footPos[0];
            var scale1Conv = Convolve(_calibration.Skip(_footPos[0]).Take(num).ToList(), SCALE1);
            var scale2Conv = Convolve(scale1Conv, SCALE2);

            _ePointPos.Clear();
            int offset = _footPos[0] < _peakPos[0] ? 0 : 1;

            for (int i = offset; i < _footPos.Count - 1; i++)
            {
                int start = _peakPos[i];
                int end = _footPos[i + 1];

                for (int j = start; j < end && j < scale2Conv.Count; j++)
                {
                    if (j > 0 && j + 1 < scale2Conv.Count && scale2Conv[j - 1] < 0 && scale2Conv[j + 1] > 0)
                    {
                        _ePointPos.Add(j);
                        break;
                    }
                }
            }
        }

        private List<double> Convolve(List<double> signal, int scale)
        {
            int n = signal.Count;
            var result = new List<double>(new double[n]);

            for (int i = 0; i < n; i++)
            {
                double conv = 0;
                for (int j = 0; j < n; j++)
                {
                    int d = Math.Abs(i - j);
                    if (d <= scale / 2)
                        conv += signal[j] * (-8.0 / 3 * d / scale + 5.0 / 6);
                    else if (d <= scale)
                        conv += signal[j] * (7.0 / 6 * d / scale - 13.0 / 12);
                    else if (d <= 1.5 * scale)
                        conv += signal[j] * (-1.0 / 6 * d / scale + 0.25);
                }
                result[i] = conv;
            }
            return result;
        }

        #endregion

        #region 基线去除与标定

        private void RemoveBaselineWanderAndCalibrateWaveform(int sbp, int dbp)
        {
            _normalization.Clear();
            int lastIdx = _footPos.Count - 1;

            // 第一个波谷前
            for (int i = 0; i < _footPos[0]; i++)
                _normalization.Add(_pulseData[i] - _pulseData[_footPos[0]]);

            // 波谷之间
            var slopes = new List<double>();
            for (int i = 0; i < lastIdx; i++)
            {
                slopes.Add((_pulseData[_footPos[i + 1]] - _pulseData[_footPos[i]]) / _period[i]);
                for (int j = _footPos[i]; j < _footPos[i + 1]; j++)
                {
                    double drift = slopes[i] * (j - _footPos[i]);
                    _normalization.Add(_pulseData[j] - _pulseData[_footPos[i]] - drift);
                }
            }

            // 最后一个波谷后
            for (int i = _footPos[lastIdx]; i < _pulseData.Count; i++)
                _normalization.Add(_pulseData[i] - _pulseData[_footPos[lastIdx]]);

            // 标定
            _calibration.Clear();
            int nMap = (int)(dbp + 0.4 * (sbp - dbp));

            for (int i = 0; i < _footPos[1]; i++)
            {
                _calibration.Add(dbp + (nMap - dbp) * (_normalization[i] - _normalization[_footPos[0]]) / (GetAverageWave(0, 1) - _normalization[_footPos[0]]));
            }

            for (int i = 1; i < lastIdx; i++)
            {
                double avgWave = GetAverageWave(i, i + 1);
                for (int j = _footPos[i]; j < _footPos[i + 1]; j++)
                {
                    _calibration.Add(dbp + (nMap - dbp) * (_normalization[j] - _normalization[_footPos[i]]) / (avgWave - _normalization[_footPos[i]]));
                }
            }

            double lastAvgWave = GetAverageWave(lastIdx - 1, lastIdx);
            for (int i = _footPos[lastIdx - 1]; i < _normalization.Count; i++)
            {
                _calibration.Add(dbp + (nMap - dbp) * (_normalization[i] - _normalization[_footPos[lastIdx - 1]]) / (lastAvgWave - _normalization[_footPos[lastIdx - 1]]));
            }
        }

        private double GetAverageWave(int startIdx, int endIdx)
        {
            double sum = 0;
            for (int j = _footPos[startIdx]; j < _footPos[endIdx]; j++)
                sum += _normalization[j];
            return sum / (_footPos[endIdx] - _footPos[startIdx]);
        }
        #endregion
        #region 指标计算
        private void CalculateIndexValue()
        {
            var arrSpti = new List<double>();
            var arrDpti = new List<double>();
            var arrSevr = new List<double>();
            var arrAi = new List<double>();
            var arrEd = new List<double>();
            var arrHr = new List<double>();
            var arrEToPeak = new List<double>();
            double minNorm = _normalization.Min();
            for (int i = 0; i < _footPos.Count - 1; i++)
            {
                double ss = 0, sd = 0;
                for (int j = _footPos[i]; j < _footPos[i + 1]; j++)
                {
                    if (j < _fPointPos[i]) ss += _calibration[j + 1];
                    else sd += _calibration[j + 1];
                }
                arrSpti.Add(ss * 60 / _period[i]);
                arrDpti.Add(sd * 60 / _period[i]);
                arrSevr.Add(sd / ss);
                arrAi.Add(_normalization[_ePointPos[i]] / _normalization[_peakPos[i]]);
                arrHr.Add(60.0 * SAMPLE_RATE / _period[i]);
                arrEd.Add((double)_fPointToFootDis[i] / SAMPLE_RATE);
                arrEToPeak.Add((double)(_ePointPos[i] - _peakPos[i]) / SAMPLE_RATE);
            }
            // 剔除异常值后取平均
            Hr = (int)TrimmedAverage(arrHr);
            Spti = (int)TrimmedAverage(arrSpti);
            Dpti = (int)TrimmedAverage(arrDpti);
            Ai = TrimmedAverage(arrAi);
            Ed = TrimmedAverage(arrEd);
            Sevr = TrimmedAverage(arrSevr);
            EdPct = TrimmedAverage(arrEd.Select(e => e / (_period[0] / (double)SAMPLE_RATE)).ToList());
            double eToPeak = TrimmedAverage(arrEToPeak);
            Cap = (int)(_getDbp + Ai * (_getSbp - _getDbp));
        }
        private static double TrimmedAverage(List<double> values)
        {
            if (values.Count <= 2) return values.Count == 0 ? 0 : values.Average();
            values.Sort();
            return values.Skip(1).Take(values.Count - 2).Average();
        }

        private void CorrectAiByHeartRate()
        {
            Ai = Ai switch
            {
                < 1 => Ai - 0.04 * (Hr - 75) / 10,
                > 90 => Ai + 0.04 * (Hr - 75) / 10,
                _ => Ai
            };
        }

        private void ClampIndexValues()
        {
            Ai = Math.Clamp(Ai, 0.25, 0.99);
            Sevr = Math.Clamp(Sevr, 0.5, 2);
        }

        private void BuildResultArray()
        {
            index[0, 0] = "Hr:"; index[0, 1] = "EdPct:"; index[0, 2] = "Spti:";
            index[0, 3] = "Dpti:"; index[0, 4] = "Sevr:"; index[0, 5] = "Cap:";
            index[0, 6] = "Ai:"; index[0, 7] = "Ed:";

            index[1, 0] = Hr.ToString();
            index[1, 1] = EdPct.ToString("0.000");
            index[1, 2] = Spti.ToString();
            index[1, 3] = Dpti.ToString();
            index[1, 4] = Sevr.ToString("0.00");
            index[1, 5] = Cap.ToString();
            index[1, 6] = Ai.ToString("0.00");
            index[1, 7] = Ed.ToString("0.00");

            IndexValue = index;
            varFootPos = _footPos;
            varPeakPos = _peakPos;
            varEPointPos = _ePointPos;
            varFPointPos = _fPointPos;
            varCalibration = _calibration;
        }

        #endregion
    }
}

