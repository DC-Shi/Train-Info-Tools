using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Routing_Info_Viewer
{
    class ClassRouteNumberConverter : IValueConverter
    {
        /// <summary>
        /// Convert number to chars.
        /// </summary>
        /// <param name="num">The number of chars you want to show.</param>
        /// <param name="typeTarget">Return type, this should be some chars or string.</param>
        /// <param name="param">Char you want to repeat.</param>
        /// <param name="culture">Not used.</param>
        public object Convert(object num, Type typeTarget,
                              object param, CultureInfo culture)
        {
            int n;
            if(int.TryParse(num.ToString(), out n))
            {
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < n; i++)
                {
                    sb.Append(param);
                }
                return sb.ToString();
            }
            return param;
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
