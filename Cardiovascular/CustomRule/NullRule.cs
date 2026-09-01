using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Cadio.CustomRule
{
    public class NullRule:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string val = (string)value;
            try
            {
                if (val == "")
                {
                    return new ValidationResult(false, "不能为空!");
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "数值不合法，" + e.Message);
            }
            return ValidationResult.ValidResult;
        }
    }
}
