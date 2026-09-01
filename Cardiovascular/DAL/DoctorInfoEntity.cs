using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    [SugarTable("cardiodoctorinfo")]
    public class DoctorInfoEntity : BaseEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
        public new int Id { get; set; }
        public string DoctorName { get; set; }
        public string DoctorPwd { get; set; }
    }
}
