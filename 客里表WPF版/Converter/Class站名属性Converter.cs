using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using 客里表Library;
using 客里表Library.Database;
using 客里表WPF版.Class;

namespace 客里表WPF版.Converter
{
    class Class站名属性Converter : IValueConverter
    {
        static ViewModel客里表数据 classVM = new ViewModel客里表数据();
        /// <summary>
        /// 把路名转换为本站其他属性的Text
        /// </summary>
        /// <param name="stationName">站名</param>
        /// <param name="typeTarget">Return type, this should be some chars or string.</param>
        /// <param name="param">Not used.</param>
        /// <param name="culture">Not used.</param>
        public object Convert(object stationName, Type typeTarget,
                              object param, CultureInfo culture)
        {
            var findStation = classVM.所有站名.Where(x => x.站名.Equals((stationName as Class线路里程)?.站名));
            if (findStation.Count() > 0)
            {
                /// Station found, show the first one.
                var first = findStation.First();

                /// Depending on the param, show the value text.
                switch (param?.ToString())
                {
                    case "限制ToolTip":
                        var 营业办理限制 = first.营业办理限制;
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
                        if (营业办理限制.Contains("■") || 营业办理限制.Contains("Ａ") || 营业办理限制.Contains("Ｚ"))
                        {
                            sb.AppendLine("■、Ａ、Ｚ	这图例是内部测试用");
                        }
                        return sb.ToString().TrimEnd(null);

                    case "限制":
                        return first.营业办理限制;
                    case "结算站":
                        return first.是否接算站 ? "是" : string.Empty;
                    case "接续线路":
                        var count = first.所属路线.Count();
                        return count > 1 ? count.ToString() : string.Empty;
                    case "高速动车限制":
                        return first.是否允许办理G车 ? "G可通过此处" : string.Empty;
                }
                
            }
            /// 如果没遇到可识别的param，那么直接返回，不转换
            return stationName;
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
