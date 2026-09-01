using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    [SugarTable("systemconfig")]
    public class SystemconfigEntity : BaseEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]//数据库是自增才配自增
        public new int Id { get; set; }

        //管理员医师的账号和密码
        public string APP_DoctorName { get; set; }
        public string APP_DoctorPWD { get; set; }

        //用于直插数据库方式 数据交互
        public string APP_DbIp {  get; set; }
        public string APP_DbPort {  get; set; }
        public string APP_DbUser {  get; set; }
        public string APP_DbPsw { get; set; }
        public string APP_DbName { get; set; }
        public string APP_ConctionString {  get; set; }

        //用于直插数据库方式 报告上传
        public string APP_FtpIp { get; set; }
        public string APP_FtpPort {  get; set; }
        public string APP_FtpUser { get; set; }
        public string APP_FtpPsw { get; set; }
        public string APP_IsFtp {  get; set; }

        //单机版 打印版设置 用于原先VB
        public string APP_OnlineOrLocal {  get; set; }

        //客户自定义设置（暂时未用到）
        public string APP_dataType {  get; set; }
        public string APP_checkNo {  get; set; }
        public string APP_checkDepartment {  get; set; }
        public string APP_checkDoctor {  get; set; }
        public string APP_checkTime { get; set; }
        public string APP_checkResult {  get; set; }
        public string APP_checkResultText {  get; set; }
        public string APP_manufacturer {  get; set; }
        public string APP_deviceModel {  get; set; }

        //系统设置界面的密码
        public string APP_PWD {  get; set; }

        public string APP_Network { get; set; }//单机 网络

        //网络版本 上传和获取用户接口
        public string APP_ApiUrlLogin { get; set; }
        public string APP_ApiUrlData { get; set; }
        public string APP_AutoUpload { get; set; }

        //软件版本号
        public string APP_Version {  get; set; }
        //设备型号
        public string APP_Type { get; set; }
        public string APP_OwnerSet {  get; set; }

        //报告界面头部显示
        public string APP_CompaneTitle { get; set; }

        //调试密码
        public string APP_debugPwd { get; set; }

        //用于通信的COM口
        public string APP_BPPort {  get; set; }
        //打印 
        public string APP_PrinterDialog { get; set; }
        //默认打印机名称
        public string APP_Printer {  get; set; }
        public string APP_COMRate { get; set; }

        public string APP_CorSbp { get; set; }
        public string APP_CorDbp { get; set; }
        public string APP_CorMap { get; set; }
        public string APP_CorHr { get; set; }
    }
}
