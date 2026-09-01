using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;

namespace Cardio.Model
{
    public class DataListViewModel<T> : BaseViewModel, IDialogResultable<string>
    {
        private int pageIndex = 1;
        public int PageIndex
        {
            get => pageIndex;
            set => SetProperty(ref pageIndex, value);
        }
        private int recordTotal = 0;
        public int RecordTotal
        {
            get => recordTotal;
            set => SetProperty(ref recordTotal, value);
        }
        private List<T> dataList;
        public List<T> DataList
        {
            get => dataList;
            set => SetProperty(ref dataList, value);
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

        private List<T> doctorLsit;
        public List<T> DoctorLsit
        {
            get => doctorLsit;
            set => SetProperty(ref doctorLsit, value);
        }


        private int _RemoveAndAdd = 0;
        public int RemoveAndAdd
        {
            get => _RemoveAndAdd;
            set => SetProperty(ref _RemoveAndAdd, value);
        }

        private string _Message;
        public string Message
        {
            get => _Message;
            set => SetProperty(ref _Message, value);
        }
        public Action CloseAction { get; set; }
        public string Result { get; set; }
    }

}



