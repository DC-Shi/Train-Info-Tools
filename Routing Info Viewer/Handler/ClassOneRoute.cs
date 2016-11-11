using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routing_Info_Viewer.Handler.Database;

namespace Routing_Info_Viewer.Handler
{
    class ClassOneRoute
    {
        public List<Class线路里程> Stations { get; set; }
        public double Length { get; set; }
        public int PassedRoutes { get { return _passedRoutes; } }

        private int _passedRoutes = 1;

        public ClassOneRoute() { }
        public ClassOneRoute(ClassOneRoute copr)
        {

            Length = copr.Length;
            Stations = copr.Stations.ToList();
            _passedRoutes = copr.PassedRoutes;
        }

        public enum ClassOnePossibleRouteReturnStatus
        {
            Normal,
            Same_station_in_same_route,
            different_station_and_different_route
        }
        public ClassOnePossibleRouteReturnStatus Add(Class线路里程 csn, int idx)
        {
            if (!(idx == 0) && !(idx == Stations.Count)) throw new NotImplementedException();
            // if this one is a new one, add it directly!
            if ((Stations == null) || (Stations.Count == 0))
            {
                Length = 0;
                _passedRoutes = 1;
                Stations = new List<Class线路里程>();
                Stations.Add(csn);
                return ClassOnePossibleRouteReturnStatus.Normal;
            }
            foreach (var s in Stations)
            {
                if (s.Equals(csn)) // we add one station in the same route that added before, throw new exception
                                   //throw new DuplicateWaitObjectException("Encounter same station in same route.");
                    return ClassOnePossibleRouteReturnStatus.Same_station_in_same_route;
                // 三亚——三亚的情况被排除。
            }

            // we can add it to the list now.
            var nearbyStation = Stations[idx == Stations.Count ? idx - 1 : idx];
            // first we have to check whether this is a interchange station, just change the route, but not add the length
            if (nearbyStation.站名.Equals(csn.站名) && !nearbyStation.线路名.Equals(csn.线路名))
            {
                Stations.Insert(idx, csn);
                _passedRoutes += 1;
                return ClassOnePossibleRouteReturnStatus.Normal;
            }
            // if we are in the same route, add the station and add the length
            if (!nearbyStation.站名.Equals(csn.站名) && nearbyStation.线路名.Equals(csn.线路名))
            {
                Stations.Insert(idx, csn);
                Length += Math.Abs(nearbyStation.距起始站里程 - csn.距起始站里程);
                return ClassOnePossibleRouteReturnStatus.Normal;
            }

            // other situations, we want to add a station which is not in the same route and not the same name, throw new not match exception
            //throw new FormatException("Not in same route, or not in same station.");
            return ClassOnePossibleRouteReturnStatus.different_station_and_different_route;
        }

        public ClassOnePossibleRouteReturnStatus AddBack(Class线路里程 csn)
        {
            if ((Stations == null) || (Stations.Count == 0))
            {
                Length = 0;
                _passedRoutes = 1;
                Stations = new List<Class线路里程>();
                Stations.Add(csn);
                return ClassOnePossibleRouteReturnStatus.Normal;
            }
            return Add(csn, Stations.Count);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Class线路里程 lastCRM = new Class线路里程();
            sb.AppendFormat("全程{0}km, 途径{1}站, 走过{2}条线路, ", Length, Stations.Count, _passedRoutes);
            for (int i = 0; i < Stations.Count; i++)
            {
                // if not match the previous route
                // this means we changed the route, we need add this station.
                if (lastCRM.线路名 != Stations[i].线路名)
                {
                    sb.AppendFormat("{0}--({1},{2})--", Stations[i].站名, Stations[i].线路名, Stations[i].距起始站里程);
                    lastCRM = Stations[i];
                }

            }
            sb.AppendFormat(Stations.Last().站名);
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            var tmp = (ClassOneRoute)obj;
            return tmp.Length == Length
                && tmp.PassedRoutes == PassedRoutes
                && tmp.Stations.SequenceEqual(Stations);
        }
        public override int GetHashCode()
        {
            int retHash = Length.GetHashCode() ^ PassedRoutes.GetHashCode();
            foreach (var sta in Stations) retHash ^= sta.GetHashCode();
            return retHash;
        }

    }
}
