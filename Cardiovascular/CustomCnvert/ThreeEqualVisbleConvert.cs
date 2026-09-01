using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cardio.CustomConvert
{
    public class ThreeEqualVisbleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 3)//新增
            {
                return "Visible";
            }
            return "Collapsed";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 3)//新增
            {
                return "Visible";
            }
            return "Collapsed";
        }
    }
}
