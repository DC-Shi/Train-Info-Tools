using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using 客里表Library;

namespace 客里表WPF版.Converter
{
    class Class限制文本Converter : IValueConverter
    {
        static ClassDatabase classDB = new ClassDatabase("data.mdb");
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
            var findStation = classDB.ListStationName.Where(x => x.站名.Equals(stationName));
            if (findStation.Count() > 0)
            {
                var 营业办理限制 = findStation.First().营业办理限制;
                /// If we need tooltip text
                if (object.Equals(param, "ToolTip"))
                {
                    /// Tooltip was constructed when mouse hover the textblock.
                    /// 
                    StringBuilder sb = new StringBuilder();

                    if (营业办理限制.Contains("×"))
                    {
                        sb.AppendLine("×	不办理行李和包裹业务的车站");
                    }
                    if (营业办理限制.Contains("△"))
                    {
                        sb.AppendLine("△	不办理客运业务的线路连接点车站");
                    }
                    if (营业办理限制.Contains("※"))
                    {
                        sb.AppendLine("※	旅客乘降所，只办理旅客乘降业务");
                    }
                    if (营业办理限制.Contains("□"))
                    {
                        sb.AppendLine("□	：里程表未包含但有图定列车停靠的车站");
                    }
                    if (营业办理限制.Contains("▲"))
                    {
                        sb.AppendLine("▲	：里程表未包含的线路连接点");
                    }
                    if (营业办理限制.Contains("G"))
                    {
                        sb.AppendLine("G	：有动车在此站办理业务");
                    }
                    if (营业办理限制.Contains("■")||营业办理限制.Contains("Ａ") ||营业办理限制.Contains("Ｚ"))
                    {
                        sb.AppendLine("■、Ａ、Ｚ	这图例是内部测试用");
                    }
                    return sb.ToString().TrimEnd(null);
                }
                
                /// Station found, show the first one.
                return 营业办理限制;
            }
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
