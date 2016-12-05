using Routing_Info_Viewer.Handler;
using Routing_Info_Viewer.layout_class;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Routing_Info_Viewer
{
    class ClassSimplifyRoutingConverter : IValueConverter
    {
        /// <summary>
        /// Simplify one routing.
        /// </summary>
        /// <param name="onePossibleRoute">Original route that contains full station list.</param>
        /// <param name="typeTarget">Return type, this should be a list of Class线路里程.</param>
        /// <param name="param">Not used.</param>
        /// <param name="culture">Not used.</param>
        public object Convert(object onePossibleRoute, Type typeTarget,
                              object param, CultureInfo culture)
        {
            if (onePossibleRoute is ClassOnePossibleRoute)
            {
                var curRoute = onePossibleRoute as ClassOnePossibleRoute;
                List<object> ret = new List<object>();
                /// Add first station.
                ret.Add(curRoute.Stations[0]);

                int lastStationIdx = 0;
                for (int i = 0; i < curRoute.Stations.Count; i++)
                {
                    /// If current station is not the same route with previous one, we should add it.
                    if (curRoute.Stations[lastStationIdx].线路名 != curRoute.Stations[i].线路名)
                    {
                        /// Add the interval
                        ret.Add(new ClassIntervalInfo()
                        {
                            IntervalStartStation = curRoute.Stations[lastStationIdx],
                            IntervalEndStation = curRoute.Stations[i - 1]
                        });
                        /// And add the station
                        ret.Add(curRoute.Stations[i]);
                        lastStationIdx = i;
                    }
                }

                /// Add the last station
                /// Add the interval
                ret.Add(new ClassIntervalInfo()
                {
                    IntervalStartStation = curRoute.Stations[lastStationIdx],
                    IntervalEndStation = curRoute.Stations.Last()
                });
                /// And add the station
                ret.Add(curRoute.Stations.Last());


                return ret;
            }
            throw new InvalidCastException("Cannot convert the value to ClassOnePossibleRoute");
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
