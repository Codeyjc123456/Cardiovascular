using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    public class OnlineConfigEntity : BaseEntity
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DbIp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DbPort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DbUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DbPsw { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApiUrlLogin { get; set; }
        public string ApiUrlData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FtpIp { get; set; }
        public string FtpPort { get; set; }
        public string FtpUser { get; set; }
        public string FtpPsw { get; set; }
        public string ApiOrDb { get; set; }
        public string IsFtp { get; set; }
        public string DbName { get; set; }


        public string password { get; set; }
        public string  OnlineOrLocal { get; set; }//单机版还是网络版 0是单机版，1是网络版
        public string LoginIPPort { get; set; }
        public string UploadDataPort  { get; set; }


}
}
