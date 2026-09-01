using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    public class UploadTestDataEntity
    {//38

        public string userSn { get; set; }
        public string userName { get; set; }
        public string userSex { get; set; }
        public string userAge { get; set; }
        public string userHeight { get; set; }
        public string userWeight { get; set; }
        public string TestDateTime { get; set; }
        //public int abiNum { get; set; }
        public string lbSbp { get; set; }
        public string lbDbp { get; set; }
        public string lbMap { get; set; }
        public string laSbp { get; set; }
        public string laDbp { get; set; }
        public string laMap { get; set; }
        public string rbSbp { get; set; }
        public string rbDbp { get; set; }
        public string rbMap { get; set; }
        public string raSbp { get; set; }
        public string raDbp { get; set; }
        public string raMap { get; set; }
        public string leftABI { get; set; }
        public string rightABI { get; set; }
        public string lbPwv { get; set; }
        public string rbPwv { get; set; }
        public string diagnosisResult { get; set; }
        public string diagnosisProposal { get; set; }

        //心血管指标
        //public int aiNum { get; set; }
        public string SBP { get; set; }
        public string DBP { get; set; }
        public string ED { get; set; }
        public string SPTI { get; set; }
        public string DPTI { get; set; }
        public string CAP { get; set; }
        public string SEVR { get; set; }
        //public double AIX { get; set; }AIDiagnosisResult
        public string aiAssess { get; set; }
        public string AIDiagnosisResult { get; set; }
        public string AIDiagnosisProposal { get; set; }
        public string HR { get; set; }
    }


}
