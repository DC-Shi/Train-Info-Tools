using 客里表Library.Database;
using 客里表Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Info_Viewer.Handler
{
    class ClassMultipleRoutes : INotifyPropertyChanged
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

        ObservableCollection<ClassOnePossibleRoute> _sortedPossibleRoutes = new ObservableCollection<ClassOnePossibleRoute>();
        string _startStation;
        string _endStation;
        double maxLength = 5000;
        int maxCount = 5;
        int maxTransfers = 4;
        double timeout = 0.5;

        private BackgroundWorker bwFind = null;

        /// <summary>
        /// Possible routes that from start to end.
        /// </summary>
        public ObservableCollection<ClassOnePossibleRoute> SortedPossibleRoutes
        {
            get
            {
                return _sortedPossibleRoutes;
            }

            set
            {
                _sortedPossibleRoutes = value;
            }
        }

        /// <summary>
        /// The station where the route starts.
        /// </summary>
        public string StartStation
        {
            get
            {
                return _startStation;
            }

            set
            {
                _startStation = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The station where the route ends.
        /// </summary>
        public string EndStation
        {
            get
            {
                return _endStation;
            }

            set
            {
                _endStation = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Max length of the route.
        /// The longest train services currently is Guangzhou - Lhasa Z264/Z265, 4976km.
        /// </summary>
        public double MaxLength
        {
            get
            {
                return maxLength;
            }

            set
            {
                maxLength = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Maximum number of routes allowed.
        /// </summary>
        public int MaxCount
        {
            get
            {
                return maxCount;
            }

            set
            {
                maxCount = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Maximum of transfers between lines.
        /// </summary>
        public int MaxTransfers
        {
            get
            {
                return maxTransfers;
            }

            set
            {
                maxTransfers = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Timeout of background searching.
        /// </summary>
        public double Timeout
        {
            get
            {
                return timeout;
            }

            set
            {
                timeout = value;
            }
        }

        #endregion

        public void FindInBackground(ClassDatabase db)
        {
            /// Initialize BackgroundWorker.
            if (bwFind == null)
            {
                bwFind = new BackgroundWorker();
                bwFind.DoWork += BwFind_DoWork;
                bwFind.WorkerReportsProgress = true;
                bwFind.ProgressChanged += BwFind_ProgressChanged;
                bwFind.WorkerSupportsCancellation = true;
                bwFind.RunWorkerCompleted += BwFind_RunWorkerCompleted;
            }

            /// TODO: support cancellation.
            if (bwFind.IsBusy)
            {
                Timeout = -1;
                bwFind.CancelAsync();
            }

            /// Start Finding...
            bwFind.RunWorkerAsync(db);
        }

        private void BwFind_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Loop: {0}", checkedTimes);
        }

        private void BwFind_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SortedPossibleRoutes.Add(e.UserState as ClassOnePossibleRoute);
        }

        private void BwFind_DoWork(object sender, DoWorkEventArgs e)
        {
            ClassDatabase db = e.Argument as ClassDatabase;
            if (db != null)
            {
                DateTime startTime = DateTime.UtcNow;

                /// Use list to store currently scanned routes.
                /// Sorted by minimum route transfers.
                List<ClassOnePossibleRoute> sortedPartialRoutes = new List<ClassOnePossibleRoute>();

                /// Find all routes that contains start station.
                var starts = db.ListRouteMileage.Where(x => x.站名.Equals(StartStation));
                foreach (var start in starts)
                {
                    ClassOnePossibleRoute copr = new ClassOnePossibleRoute();
                    copr.AddBack(start);
                    sortedPartialRoutes.Add(copr);
                }

                /// Remember last two stations so we assure only 1 route change in 1 station.
                var lastStation = new Class线路里程();
                var last2ndStation = new Class线路里程();

                /// Reset checking counter.
                checkedTimes = 0;

                /// From partial routes, use BFS to get target.
                while (sortedPartialRoutes.Count > 0)
                {
                    /// Timeout, return.
                    if ((DateTime.UtcNow - startTime).TotalMinutes > Timeout)
                    {
                        /// Output for debugging
                        /// 
                        Console.WriteLine("Timeout, function return.");
                        return;
                    }

                    /// Get the first partial route,
                    /// and removed from queue.
                    ClassOnePossibleRoute copr = sortedPartialRoutes.First();
                    sortedPartialRoutes.RemoveAt(0);

                    /// Get last two stations.
                    lastStation = copr.Stations.Last();
                    if (copr.Stations.Count >= 2)
                        last2ndStation = copr.Stations[copr.Stations.Count - 2];
                    else
                        last2ndStation = new Class线路里程();

                    /// if we encountered the end station, we simply add it.
                    if (lastStation.站名.Equals(EndStation))
                    {
                        (sender as BackgroundWorker).ReportProgress(100 * SortedPossibleRoutes.Count / MaxCount, copr);
                        //SortedPossibleRoutes.Add(copr);
                        //DebugPrint(copr);
                        /// Output for debugging
                        /// 
                        Console.WriteLine("耗时{0} | {1}", (DateTime.UtcNow - startTime).TotalSeconds, copr);

                        /// Do not search if we have enough possible routes.
                        if (SortedPossibleRoutes.Count > MaxCount)
                        {
                            /// Output for debugging
                            /// 
                            Console.WriteLine("SortedPossibleRoutes.Count > MaxCount, function return.");
                            return;
                        }
                        continue;
                    }

                    /// if the length reaches max, ignore it!
                    if (copr.Length > MaxLength || copr.TransferedRouteNum > MaxTransfers) { continue; }
                    /// else, add all the other station in the same line
                    /// or in the same station.

                    /// Same staions, but different route
                    var allNextStations = db.ListRouteMileage.Where(
                        x =>
                            x.站名.Equals(lastStation.站名) && !x.线路名.Equals(lastStation.线路名)
                        // 考虑到杜家坎是可以走的但是不能调头，因此此处取消该条件（“不能是不办理客运的线路连接点站”）。
                        // 从而得到的结果是在线路连接点允许调头
                        // 由于存在怀柔北——通州西区间的列车，要走行怀柔北线，怀柔北与怀柔均不是结算站，因此该条件取消（“当前站需要为结算站”）。
                        ).ToList();
                    /// Same route, but different station
                    var sameRoute = db.ListRouteMileage.Where(x => x.线路名.Equals(lastStation.线路名)).ToList();
                    int idxOfLastStation = sameRoute.IndexOf(lastStation);
                    if (idxOfLastStation < 0)
                    { }//NormalPrint("Cannot find " + lastStation.ToString());
                    else
                    {
                        /// If the last station of current checking route is not the first one on sameRoute,
                        /// add to checking list.
                        if ((idxOfLastStation > 0) && !allNextStations.Contains(sameRoute[idxOfLastStation - 1]))
                            /// Do not add duplicate station.
                            if (!copr.Stations.Contains(sameRoute[idxOfLastStation - 1]))
                                allNextStations.Add(sameRoute[idxOfLastStation - 1]);
                        /// If the last station of current checking route is not the last one on sameRoute,
                        /// add to checking list.
                        if ((idxOfLastStation < sameRoute.Count - 1) && !allNextStations.Contains(sameRoute[idxOfLastStation + 1]))
                            /// Do not add duplicate station.
                            if (!copr.Stations.Contains(sameRoute[idxOfLastStation + 1]))
                                allNextStations.Add(sameRoute[idxOfLastStation + 1]);
                    }

                    /// Check all possible stations.
                    foreach (var nextStation in allNextStations)
                    {
                        ClassOnePossibleRoute newCopr;
                        ClassOnePossibleRouteReturnStatus res;
                        if (allNextStations.Count == 1)
                        {
                            /// Contains only 1 next station, so directly add, no need to create new one.
                            res =  copr.AddBack(nextStation);
                            newCopr = copr;
                        }
                        else {
                            /// Create a possible route from current route.
                            newCopr = new ClassOnePossibleRoute(copr);
                            /// Add the current station to the back of the station list.
                            res = newCopr.AddBack(nextStation);
                        }
                        /// Checking the result.
                        switch (res)
                        {
                            /// Ignore if we encountered such error.
                            case ClassOnePossibleRouteReturnStatus.different_station_and_different_route:
                            case ClassOnePossibleRouteReturnStatus.Same_station_in_same_route:
                                continue;
                            /// If we added it successfully,
                            case ClassOnePossibleRouteReturnStatus.Normal:
                                /// if we switch multiple routes in the same station,
                                /// There is no need to switch routes more than 2 tiems in same station,
                                /// ignore it!
                                if (last2ndStation.站名.Equals(nextStation.站名) && lastStation.站名.Equals(nextStation.站名))
                                {

                                }
                                else
                                {
                                    /// if we encounter first two station has a same name,
                                    /// The start station is not needed to switch route,
                                    /// ignore!
                                    if (last2ndStation.站名.Equals(string.Empty) && lastStation.站名.Equals(nextStation.站名)) { }
                                    else
                                    /// if some station appears 3 times or more,
                                    /// means A-B-C-D-B, which is a loop,
                                    /// B in A-B & B-C & D-B,
                                    /// ignore it!
                                    /// Only add the station that did not in a loop.
                                    if (newCopr.Stations.Where(x => x.站名.Equals(nextStation.站名)).Count() <= 2)
                                        sortedPartialRoutes.Add(newCopr);
                                }
                                break;
                            default:
                                throw new Exception("Critical error, we are not designed to go here");

                        }
                    }
                    /// Sort the list by transfer number.
                    /// No need to sort each time, sort with low frequency.
                    if (++checkedTimes % 100 == 0)
                    {
                        sortedPartialRoutes.Sort((x, y) => (x.TransferedRouteNum - y.TransferedRouteNum));
                    }
                }


                /// Output for debugging
                /// 
                Console.WriteLine("Loop exit since we examined all possible routes.");
            }
        }

        int checkedTimes = 0;
    }
}
