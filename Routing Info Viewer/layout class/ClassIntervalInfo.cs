using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routing_Info_Viewer.Handler.Database;

namespace Routing_Info_Viewer.layout_class
{
    class ClassIntervalInfo
    {
        Class线路里程 intervalStartStation;
        Class线路里程 intervalEndStation;

        /// <summary>
        /// The start station of this interval.
        /// </summary>
        public Class线路里程 IntervalStartStation
        {
            get
            {
                return intervalStartStation;
            }

            set
            {
                intervalStartStation = value;
            }
        }

        /// <summary>
        /// The end station of this interval.
        /// </summary>
        public Class线路里程 IntervalEndStation
        {
            get
            {
                return intervalEndStation;
            }

            set
            {
                intervalEndStation = value;
            }
        }

        /// <summary>
        /// The name of this line.
        /// </summary>
        public string LineName
        {
            get
            {
                return IntervalStartStation.线路名;
            }
        }

        /// <summary>
        /// The distance between this interval start and end.
        /// </summary>
        public double IntervalDistance
        {
            get
            {
                return Math.Abs(IntervalStartStation.距起始站里程 - IntervalEndStation.距起始站里程);
            }
        }
    }
}
