using Cardio.CustomRule;
using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Model
{
    class LoginViewModel : BaseViewModel, IDialogResultable<string>, IValidationExceptionHandler
    {
        public string Result { get; set; }

        private string userId = "";
        public string UserID
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        private string userPWD = "";
        public string UserPWD
        {
            get => userPWD;
            set => SetProperty(ref userPWD, value);
        }

        private int _RemoveAndAdd = 0;
        public int RemoveAndAdd
        {
            get => _RemoveAndAdd;
            set => SetProperty(ref _RemoveAndAdd, value);
        }

        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        private string isRunning = "Hidden";
        public string IsRunning
        {
            get => isRunning;
            set => SetProperty(ref isRunning, value);
        }

        public Action CloseAction { get; set; }

        private string _Message;
        public string Message
        {
            get => _Message;
            set => SetProperty(ref _Message, value);
        }
    }
}