using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.CustomRule
{
    public interface IValidationExceptionHandler
    {
        int RemoveAndAdd { get; set; }

        /// <summary>
        /// 异常提示
        /// </summary>
        string Message { get; set; }
    }
}
