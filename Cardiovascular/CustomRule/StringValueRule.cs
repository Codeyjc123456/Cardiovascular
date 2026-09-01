using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Cadio.CustomRule
{
    public class StringValueRule : ValidationRule
    {
        public string RegexPattern { get; set; } = "";
        public bool IsRequired { get; set; } = false;
        public int? Min { get; set; }
        public int? Max { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // 必填校验
            string input = value?.ToString()?.Trim() ?? "";
            if (IsRequired && string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, "该字段不能为空");
            }
            //长度校验
            if (input.Length < Min || input.Length > Max) return new ValidationResult(false, "字符长度不合理，应在 " + Min + "-" + Max + " 位！");
            // 正则校验
            if (!string.IsNullOrEmpty(RegexPattern))
            {
                if (!Regex.IsMatch(input ?? "", RegexPattern))
                    return new ValidationResult(false, "输入格式不正确");
            }
            return ValidationResult.ValidResult;
        }
    }
}
