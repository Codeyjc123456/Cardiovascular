using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Cadio.CustomRule
{
    public class DataValueRule:ValidationRule
    {
        public bool IsRequired { get; set; } = false;
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string RegexPattern { get; set; } = "";
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            //必填校验
            string? input = value?.ToString()?.Trim();
            if (IsRequired && string.IsNullOrEmpty(input))
            {
                return new ValidationResult(false, "该字段不能为空");
            }
            //范围校验
            if (Min.HasValue || Max.HasValue) //判断是否为空
            {
                if (double.TryParse(input, out double num))
                {
                    if (num < Min || num > Max)
                        return new ValidationResult(false, "请输入正常范围的数值：" + Min + "-" + Max);
                }
                else
                    return new ValidationResult(false, "数值不合理，请输入如：" + RegexPattern + "。");//数值不合理，请输入如“
            }
            return ValidationResult.ValidResult;
        }
    }

}
