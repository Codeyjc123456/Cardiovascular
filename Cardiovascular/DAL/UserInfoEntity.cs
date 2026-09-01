using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    [SugarTable("userinfolocal")]
    public class UserInfoEntity : BaseEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
        public new int Id { get; set; }
        public string UserId { get; set; }
        public string IsServerId { get; set; }
        public string UserName { get; set; }
        public string UserSex { get; set; }
        public int UserAge { get; set; }
        public string UserBirthday { get; set; }
        public double UserHeight { get; set; }
        public double UserWeight { get; set; }
        public double UserDistance { get; set; }
        public string UserIdCard { get; set; }
        public string UserAddress { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserFillTime { get; set; }
        public string UserModifyTime { get; set; }
        public string OperatingDoctor { get; set; }
        public string UserNote { get; set; }
        public int RiskSmoking { get; set; }
        public int RiskHypertension { get; set; }
        public int RiskDyslipidemia { get; set; }
        public int CoronaryDiease { get; set; }
        public int Stroke { get; set; }
        public int KidneyDiease { get; set; }
        public int HeartFailure { get; set; }
        public int MvocardialInfarction { get; set; }
        public int Angina { get; set; }
        public string OtherDisease { get; set; }
        public int IsSurveryed  { get; set; }
        public string Department { get; set; }
        public string HospitalizationNum { get; set; }
        public int Waistline { get; set; }
        public string CreateTime { get; set; }


    }
}
