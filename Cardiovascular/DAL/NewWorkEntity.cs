using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.DAL
{
    public class NewWorkEntity:BaseEntity
    {
        public string ServerUserName { get; set; }
        public string ServerPassword { get; set; }
        public string ServerDatabase { get; set; }
        public string ServerPort { get; set; }
        public string ServerIPAddr1 { get; set; }
        public string ServerIPAddr2 { get; set; }
        public string ServerIPAddr3 { get; set; }
        public string ServerIPAddr4 { get; set; }
        public string FTPIPAddr1 { get; set; }
        public string FTPIPAddr2 { get; set; }
        public string FTPIPAddr3 { get; set; }
        public string FTPIPAddr4 { get; set; }
        public string FTPIPAddr5 { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
    }
}
