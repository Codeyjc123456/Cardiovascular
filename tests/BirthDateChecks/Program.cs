using Cadio.CustomRule;
using System.Globalization;

DateTime today = new(2026, 9, 7, 10, 23, 13);
var culture = CultureInfo.GetCultureInfo("zh-CN");
int checks = 0;

void Check(object? input, bool expected, DateTime? reference = null, CultureInfo? format = null)
{
    var result = BirthDateRule.ValidateBirthDate(input, format ?? culture, reference ?? today, out _);
    if (result.IsValid != expected)
        throw new Exception($"校验结果不符：{input}，预期 {expected}，实际 {result.ErrorContent}");
    checks++;
}

Check("1906-10-01", true);
Check("1906-09-07", true);
Check("1906-09-06", false);
Check("2018-09-07", true);
Check("2018-09-08", false);
Check(new DateTime(2018, 9, 7, 23, 59, 59), true);
Check("1906/10/1", true);
Check("10/1/1906 12:00:00 AM", true);
Check("01.10.1906", true, format: CultureInfo.GetCultureInfo("de-DE"));
Check("2000-02-29", true);
Check("1906-02-29", false);
Check("2026-09-08", false);
Check(null, false);
Check("", false);
Check("不是日期", false);
Check("1908-02-29", true, new DateTime(2028, 2, 29));
Check("1908-02-28", false, new DateTime(2028, 2, 29));
Check("2020-02-29", true, new DateTime(2028, 2, 29));
Check("2020-03-01", false, new DateTime(2028, 2, 29));

if (BirthDateRule.CalculateAge(new DateTime(1906, 10, 1), today) != 119 ||
    BirthDateRule.CalculateAge(new DateTime(2018, 9, 7), today) != 8 ||
    BirthDateRule.CalculateAge(new DateTime(2018, 9, 8), today) != 7)
    throw new Exception("周岁计算未正确判断生日是否已到");

var invalid = BirthDateRule.ValidateBirthDate("1906-09-06", culture, today, out _);
if (!Equals(invalid.ErrorContent, "日期范围不对，请输入：1906-09-07 至 2018-09-07（含首尾日期）"))
    throw new Exception("错误提示范围或日期格式不正确");

Console.WriteLine($"PASS: {checks} 项日期校验、3 项周岁计算及范围提示检查。");
