using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Info_Viewer.Handler
{
    public enum ClassOnePossibleRouteReturnStatus
    {
        Normal,
        Same_station_in_same_route,
        different_station_and_different_route
    }
}
