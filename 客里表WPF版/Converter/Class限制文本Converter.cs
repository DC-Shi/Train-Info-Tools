using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace 客里表WPF版.Converter
{
    class Class限制文本Converter : IValueConverter
    {
        /// <summary>
        /// 把路名转换为本站办理限制条件的Text
        /// </summary>
        /// <param name="stationName">站名</param>
        /// <param name="typeTarget">Return type, this should be some chars or string.</param>
        /// <param name="param">Not used.</param>
        /// <param name="culture">Not used.</param>
        public object Convert(object stationName, Type typeTarget,
                              object param, CultureInfo culture)
        {
            
            return string.Empty;
        }


        /// <summary>
        /// Convert from target to source.
        /// Not implemented since we only need one way binding.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeTarget"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        public object ConvertBack(object value, Type typeTarget,
                                  object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
