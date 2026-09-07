using System;
using System.Globalization;
using System.Windows.Controls;

namespace Cadio.CustomRule
{
    public class BirthDateRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ValidateBirthDate(value, cultureInfo, DateTime.Today, out _);
        }

        public static ValidationResult ValidateBirthDate(object? value, CultureInfo cultureInfo,
            DateTime today, out DateTime birthDate)
        {
            // 兼容 DatePicker 的区域日期格式及已有标准日期格式，只比较日期部分。
            string? text = value?.ToString();
            if (value is DateTime date)
                birthDate = date;
            else if (!DateTime.TryParseExact(text, new[] { "yyyy-MM-dd", "M/d/yyyy h:mm:ss tt" },
                         CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out birthDate) &&
                     !DateTime.TryParse(text, cultureInfo, DateTimeStyles.AllowWhiteSpaces, out birthDate))
                return new ValidationResult(false, "日期格式不正确，请输入正确格式，例如：2000-08-07");

            birthDate = birthDate.Date;
            DateTime minimum = today.Date.AddYears(-120);
            DateTime maximum = today.Date.AddYears(-8);
            if (birthDate < minimum || birthDate > maximum)
                return new ValidationResult(false,
                    string.Format("日期范围不对，请输入：{0:yyyy-MM-dd} 至 {1:yyyy-MM-dd}（含首尾日期）", minimum, maximum));

            return ValidationResult.ValidResult;
        }

        public static int CalculateAge(DateTime birthDate, DateTime today)
        {
            int age = today.Year - birthDate.Year;
            if (birthDate.Date.AddYears(age) > today.Date) age--;
            return age;
        }
    }
}