using Cardio.CustomRule;
using HandyControl.Tools.Extension;
using System;

namespace Cardio.Model
{
    public partial class UserViewModel : BaseViewModel, IDialogResultable<string>, IValidationExceptionHandler
    {
        #region 基础信息
        private string username = "";
        public string UserName
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string userid = "";
        public string UserID
        {
            get => userid;
            set => SetProperty(ref userid, value);
        }

        private double userweight = 60;
        public double UserWeight
        {
            get => userweight;
            set => SetProperty(ref userweight, value);
        }

        private int userheight = 160;
        public int UserHeight
        {
            get => userheight;
            set => SetProperty(ref userheight, value);
        }

        private string usersex = "男";
        public string UserSex
        {
            get => usersex;
            set => SetProperty(ref usersex, value);
        }

        private string birthday = DateTime.Now.AddYears(0).ToString("d");
        public string BirthDay
        {
            get => birthday;
            set => SetProperty(ref birthday, value);
        }

        public string Result { get; set; } = "";

        public Action CloseAction { get; set; }

        private int _RemoveAndAdd = 0;
        public int RemoveAndAdd
        {
            get => _RemoveAndAdd;
            set => SetProperty(ref _RemoveAndAdd, value);
        }

        private string _Message = "";
        public string Message
        {
            get => _Message;
            set => SetProperty(ref _Message, value);
        }

        #endregion
    }
}
