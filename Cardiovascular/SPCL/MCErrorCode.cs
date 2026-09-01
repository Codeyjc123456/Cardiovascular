using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.SPCL
{
    public enum MCErrorCode
    {
        NoError,
        TimeLimited,
        OpenSerialFail,
        BindSuccess,
        BindFail
    }
}
