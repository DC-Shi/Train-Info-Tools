using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using 客里表Library.Database;

namespace 客里表WPF版.Converter
{
    class Class计算站间距Converter : IMultiValueConverter
    {
        /// <summary>
        /// 将输入数据转换为距离差
        /// </summary>
        /// <param name="values">输入距离，应当为2个数值</param>
        /// <param name="targetType">距离差，double, int, string之一</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values[0] is Class线路里程 && values[1] is Class线路里程)
            {
                if((values[0] as Class线路里程).线路名== (values[1] as Class线路里程).线路名)
                {
                    var ret = Math.Abs((values[0] as Class线路里程).距起始站里程 - (values[1] as Class线路里程).距起始站里程);
                    return ret.ToString();
                }
            }
            return values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
