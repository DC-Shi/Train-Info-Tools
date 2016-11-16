using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routing_Info_Viewer.Handler.Database;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Routing_Info_Viewer.Handler
{
    class ClassOnePossibleRoute : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation.

        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Properties

        ObservableCollection<Class线路里程> _stations;
        int _transferedRouteNum = 0;
        double _length = 0;

        /// <summary>
        /// All stations in this route.
        /// </summary>
        public ObservableCollection<Class线路里程> Stations
        {
            get
            {
                return _stations;
            }

            set
            {
                _stations = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Number of different 线路 in this route.
        /// </summary>
        public int TransferedRouteNum
        {
            get
            {
                return _transferedRouteNum;
            }

            set
            {
                _transferedRouteNum = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Total length of this route.
        /// </summary>
        public double Length
        {
            get
            {
                return _length;
            }

            set
            {
                _length = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public ClassOnePossibleRoute()
        {
            Stations = new ObservableCollection<Class线路里程>();
        }

        public ClassOnePossibleRoute(ClassOnePossibleRoute copr)
        {
            Stations = new ObservableCollection<Class线路里程>(copr.Stations);
            Length = copr.Length;
            TransferedRouteNum = copr.TransferedRouteNum;
        }

        #endregion

        public ClassOnePossibleRouteReturnStatus Add(Class线路里程 csn, int idx)
        {
            if (!(idx == 0) && !(idx == Stations.Count)) throw new NotImplementedException();
            // if this one is a new one, add it directly!
            if ((Stations == null) || (Stations.Count == 0))
            {
                Stations = new ObservableCollection<Class线路里程>();
                Length = 0;
                TransferedRouteNum = 1;
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
                TransferedRouteNum += 1;
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
                TransferedRouteNum = 1;
                Stations = new ObservableCollection<Class线路里程>();
                Stations.Add(csn);
                return ClassOnePossibleRouteReturnStatus.Normal;
            }
            return Add(csn, Stations.Count);
        }

        #region Override methods

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Class线路里程 lastCRM = new Class线路里程();
            sb.AppendFormat("全程{0}km, 途径{1}站, 走过{2}条线路, ", Length, Stations.Count, TransferedRouteNum);
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

        //public override bool Equals(object obj)
        //{
        //    /// DisconnectedItem would appear if you resize the window very quickly and item is out of view area.
        //    /// Which would lead to invalid cast.
        //    var tmp = obj as ClassOnePossibleRoute;
        //    //if (tmp == null) return false;
        //    return tmp.Length == Length
        //        && tmp.TransferedRouteNum == TransferedRouteNum
        //        && tmp.Stations.SequenceEqual(Stations);
        //}
        //public override int GetHashCode()
        //{
        //    int retHash = Length.GetHashCode() ^ TransferedRouteNum.GetHashCode();
        //    foreach (var sta in Stations) retHash ^= sta.GetHashCode();
        //    return retHash;
        //}

        #endregion
    }
}
