using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SqlSugar;

namespace Cardio.DAL
{
    [SugarTable("pulsedatalocal")]
    public class PulseDataLocalEntity : BaseEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
        public new int Id { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string userSex { get; set; }
        public int userAge { get; set; }
        public string userBirthday { get; set; }
        public double userHeight { get; set; }
        public double userWeight { get; set; }
        public string TestDateTime { get; set; }
        [JsonIgnore]
        public int ABI_num { get; set; }
        public int Sbp { get; set; }
        public int Dbp { get; set; }
        public int Pp { get; set; }
        public int Map { get; set; }
        public int Hr { get; set; }
        public int BpHr { get; set; }
        public double Distance { get; set; }
        [JsonIgnore]
        public int AI_num { get; set; }
        public double Ed { get; set; }
        public int Spti { get; set; }
        public int Dpti { get; set; }
        public double Sevr { get; set; }
        public double AIx { get; set; }
        public string RpRawData { get; set; }
        public string AIAssess { get; set; }
        public string AIDiagnosisResult { get; set; }
        public string AIDiagnosisProposal { get; set; }
        [JsonIgnore]
        public string DoctorDiagnosis { get; set; }
        [JsonIgnore]
        public string OperationgDoctor { get; set; }
        [JsonIgnore]
        public string IsReportPrinted { get; set; }
        [JsonIgnore]
        public string FristBraBP { get; set; }
        [JsonIgnore]
        public string SecondBraBP { get; set; }
        public int Sbp2 { get; set; }
        public double EdPct { get; set; }
        public string Report_Name { get; set; }
        [JsonIgnore]
        public string IsPDFReportPrinted { get; set; }
        [JsonIgnore]
        public string IsRepertPrinted { get; set; }
        [JsonIgnore]
        public string CardiovascularFactors { get; set; }
        [JsonIgnore]
        public string CardiovascularDis { get; set; }
        [JsonIgnore]
        public string SignatureName { get; set; }
        [JsonIgnore]
        public string DiagnosisTime { get; set; }
        [JsonIgnore]
        public string HaveUpload { get; set; }
    }
}
