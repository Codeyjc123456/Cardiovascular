using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Model
{
    internal class DoctorViewModel : BaseViewModel, IDialogResultable<string>
    {
        string IDialogResultable<string>.Result { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action IDialogResultable<string>.CloseAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    }
}
