using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cardio.CustomConvert
{
    public class ChufangConvert1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "MC201")//新增 或者 编辑
            {
                return "Hidden";
            }
            return "Visible";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "MC201")//新增 或者 编辑
            {
                return "Hidden";
            }
            return "Visible";
        }
    }
}
