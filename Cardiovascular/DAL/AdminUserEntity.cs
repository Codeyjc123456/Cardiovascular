using SqlSugar;

namespace Cardio.DAL
{
    [SugarTable("cardioadminuser")]
    public class AdminUserEntity : BaseEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
        public new int Id { get; set; }
        public string AdminName { get; set; }
        public string AdminPassword { get; set; }
    }
}
