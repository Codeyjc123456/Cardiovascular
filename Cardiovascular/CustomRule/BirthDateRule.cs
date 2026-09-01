using Cardio.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Cadio.CustomRule
{
    public class BirthDateRule:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateTime d;
            bool chValidity = DateTime.TryParseExact(value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d) ||
                DateTime.TryParseExact(value.ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
            if (!chValidity) return new ValidationResult(false, "日期格式不正确，请输入正确格式，例如：2023-08-07");
            try
            {
                d = DateTime.Now;
                DateTime t = DateTime.Parse(value.ToString() ?? "");
                TimeSpan diff = d - t;
                if (diff.TotalDays / 365 < 8 || diff.TotalDays / 365 > 120)
                    return new ValidationResult(false, "日期范围不对，请输入：" + d.AddYears(-120).ToString() + "-" + d.AddYears(-8).ToString());
            }
            catch (Exception ex)
            {
                LogUtil.Error("BirthDayaRule", ex.Message);
                return new ValidationResult(false, "日期范围不对，请输入正确出生日期，例如：2000-08-07");
            }
            return ValidationResult.ValidResult;
        }
    }
}
