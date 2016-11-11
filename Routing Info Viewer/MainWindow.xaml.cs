using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Routing_Info_Viewer.Handler.Database;
using Routing_Info_Viewer.Handler;
using System.ComponentModel;

namespace Routing_Info_Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            classDB = new ClassDatabase("data.mdb");
            listRouteMileage = classDB.ListRouteMileage;
            listRouteName = classDB.ListRouteName;
            listStationName = classDB.ListStationName;
            listBox.Items.Add("Database loaded.");
        }

        List<Class线路里程> listRouteMileage = new List<Class线路里程>();
        List<Class线路名> listRouteName = new List<Class线路名>();
        List<Class站名> listStationName = new List<Class站名>();

        ClassDatabase classDB = null;

        private void buttonGetRoute_Click_old(object sender, RoutedEventArgs e)
        {
            List<ClassOneRoute> sortedPossibleRoutes = new List<ClassOneRoute>();
            List<ClassOneRoute> allPossibleRoutes = new List<ClassOneRoute>();
            var starts = listRouteMileage.Where(x => x.站名.Equals(textBoxFrom.Text));
            foreach (var start in starts)
            {
                ClassOneRoute copr = new ClassOneRoute();
                copr.AddBack(start);
                sortedPossibleRoutes.Add(copr);
            }


            var lastStation = new Class线路里程();
            var last2ndStation = new Class线路里程();

            while (sortedPossibleRoutes.Count > 0)
            {
                ClassOneRoute copr = sortedPossibleRoutes.First();
                sortedPossibleRoutes.RemoveAt(0);

                lastStation = copr.Stations.Last();
                if (copr.Stations.Count >= 2)
                    last2ndStation = copr.Stations[copr.Stations.Count - 2];
                else
                    last2ndStation = new Class线路里程();

                // if we encountered the endpoint, we simply add it to allPossibleRoutes
                if (lastStation.站名.Equals(textBoxTo.Text))
                {
                    allPossibleRoutes.Add(copr);
                    //DebugPrint(copr);
                    if (allPossibleRoutes.Count > 5) break;
                    continue;
                }

                // if the length reaches max, ignore it!
                if (copr.Length > 5000 || copr.PassedRoutes > 5) { continue; }
                // else, add all the other station in the same line
                // or in the same station.

                var allNextStations = listRouteMileage.Where(
                    // same station，并且是接算站
                    x => //(listStationName.Where(y => y.站名.Equals(x.站名)).Count() == 1) &&

                        x.站名.Equals(lastStation.站名) && !x.线路名.Equals(lastStation.线路名)
                        && (listStationName.Where(y => y.站名.Equals(x.站名)).FirstOrDefault()?.是否接算站).GetValueOrDefault(false)
                    // 并且不能是不办理客运的线路连接点站
                    // 考虑到杜家坎是可以走的但是不能调头，因此此处取消该条件。从而得到的结果是在线路连接点允许调头
                    //&& (!listStationName.Where(y => y.站名.Equals(x.站名)).FirstOrDefault()?.Property.Contains("△")).GetValueOrDefault(false)
                    //// or same route but it is a interchange station.
                    //(x.线路名.Equals(lastStation.线路名) &&
                    //    listInterchangeStation.Where(y => y.站名.Equals(x.站名)).Count() > 0) ||
                    //// or it is the endpoint
                    //x.站名.Equals(EndStation)                   
                    ).ToList();
                var sameRoute = listRouteMileage.Where(x => x.线路名.Equals(lastStation.线路名)).ToList();
                int idxOfLastStation = sameRoute.IndexOf(lastStation);
                if (idxOfLastStation < 0)
                    Console.WriteLine("Cannot find " + lastStation.ToString());
                else
                {
                    if ((idxOfLastStation > 0) && !allNextStations.Contains(sameRoute[idxOfLastStation - 1])) allNextStations.Add(sameRoute[idxOfLastStation - 1]);
                    if ((idxOfLastStation < sameRoute.Count - 1) && !allNextStations.Contains(sameRoute[idxOfLastStation + 1])) allNextStations.Add(sameRoute[idxOfLastStation + 1]);
                }

                //// if this train is not D/C/G, then 不进入高速线路
                //if (train_code.IndexOfAny(new char[] { 'D', 'C', 'G' }) < 0)  // normal train number
                //{
                //    allNextStations.RemoveAll(x => x.线路名.Contains("高速"));
                //    // K1278 好像是经过昌九城际线路
                //    //allNextStations.RemoveAll(x => x.线路名.Contains("城际"));
                //}
                //else // D/C/G车辆偏好高速和城际线路
                //{

                //}

                // use all possible stations.
                foreach (var nextStation in allNextStations)
                {
                    //try
                    //{
                    ClassOneRoute newCopr = new ClassOneRoute(copr);
                    var res = newCopr.AddBack(nextStation);
                    switch (res)
                    {
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.different_station_and_different_route:
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.Same_station_in_same_route:
                            continue;
                        //break;
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.Normal:
                            // if we switch multiple routes in the same station, we ignore it!
                            if (last2ndStation.站名.Equals(nextStation.站名) && lastStation.站名.Equals(nextStation.站名))
                            {

                            }
                            else
                            {
                                // if we encounter first two station has a same name, ignore!
                                if (last2ndStation.站名.Equals(string.Empty) && lastStation.站名.Equals(nextStation.站名)) { }
                                else
                                    // if some station appears 3 times or more, we ignore it!
                                    if (newCopr.Stations.Where(x => x.站名.Equals(nextStation.站名)).Count() <= 2)
                                    sortedPossibleRoutes.Add(newCopr);
                            }
                            break;
                        default:
                            throw new Exception("Critical error, we are not designed to go here");

                    }
                    //}
                    //catch (DuplicateWaitObjectException dwoe) { continue; }
                    //catch (FormatException fe) { Console.WriteLine("Critical error, we are not designed to go here"); }
                }
                sortedPossibleRoutes.Sort((x, y) => (x.PassedRoutes - y.PassedRoutes));
            }

            if (allPossibleRoutes.Count < 1) throw new KeyNotFoundException("Cannot find a proper path under current condition, max change route: 5");

            foreach(var route in allPossibleRoutes)
            {
                textBoxResult.Text += route.ToString() + "\r\n";
                Console.WriteLine(route);
            }
        }


        private void buttonGetRoute_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bwAddRoute = new BackgroundWorker();
            bwAddRoute.WorkerReportsProgress = true;
            bwAddRoute.ProgressChanged += BwAddRoute_ProgressChanged;
            bwAddRoute.DoWork += BwAddRoute_DoWork;
            bwAddRoute.RunWorkerCompleted += BwAddRoute_RunWorkerCompleted;
            bwAddRoute.RunWorkerAsync(new string[] { textBoxFrom.Text, textBoxTo.Text });
        }

        private void BwAddRoute_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBoxResult.Text += string.Format("Completed at {0}.\r\n", DateTime.Now.ToShortTimeString());
        }

        private void BwAddRoute_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] args = e.Argument as string[];
            string startStation = args[0];
            string endStation = args[1];
            int maxLength = 5000;
            int maxRoutes = 5;
            int reportedTimes = 0;

            /// 添加时间从而防止搜索一直持续
            DateTime startTime = DateTime.UtcNow;


            List<ClassOneRoute> sortedPossibleRoutes = new List<ClassOneRoute>();

            var starts = listRouteMileage.Where(x => x.站名.Equals(startStation));
            foreach (var start in starts)
            {
                ClassOneRoute copr = new ClassOneRoute();
                copr.AddBack(start);
                sortedPossibleRoutes.Add(copr);
            }


            var lastStation = new Class线路里程();
            var last2ndStation = new Class线路里程();

            while (sortedPossibleRoutes.Count > 0)
            {
                /// 超时则直接返回，报告已经完成。
                if ((DateTime.UtcNow - startTime).Minutes > 1) return;
                ClassOneRoute copr = sortedPossibleRoutes.First();
                sortedPossibleRoutes.RemoveAt(0);

                lastStation = copr.Stations.Last();
                if (copr.Stations.Count >= 2)
                    last2ndStation = copr.Stations[copr.Stations.Count - 2];
                else
                    last2ndStation = new Class线路里程();

                // if we encountered the endpoint, we simply add it to allPossibleRoutes
                if (lastStation.站名.Equals(endStation))
                {
                    reportedTimes++;
                    (sender as BackgroundWorker).ReportProgress(reportedTimes * 100 / maxRoutes, copr);
                    DebugPrint(copr);
                    if (reportedTimes > maxRoutes)
                    {
                        return;
                    }
                    continue;
                }

                // if the length reaches max, ignore it!
                if (copr.Length > maxLength || copr.PassedRoutes > MAX_ROUTES_BETWEEN_STATION) { continue; }
                // else, add all the other station in the same line
                // or in the same station.

                var allNextStations = listRouteMileage.Where(
                    // same station，并且是接算站
                    x => //(listStationName.Where(y => y.站名.Equals(x.站名)).Count() == 1) &&

                        x.站名.Equals(lastStation.站名) && !x.线路名.Equals(lastStation.线路名)
                        && (listStationName.Where(y => y.站名.Equals(x.站名)).FirstOrDefault()?.是否接算站).GetValueOrDefault(false)
                    // 并且不能是不办理客运的线路连接点站
                    // 考虑到杜家坎是可以走的但是不能调头，因此此处取消该条件。从而得到的结果是在线路连接点允许调头
                    //&& (!listStationName.Where(y => y.站名.Equals(x.站名)).FirstOrDefault()?.Property.Contains("△")).GetValueOrDefault(false)
                    //// or same route but it is a interchange station.
                    //(x.线路名.Equals(lastStation.线路名) &&
                    //    listInterchangeStation.Where(y => y.站名.Equals(x.站名)).Count() > 0) ||
                    //// or it is the endpoint
                    //x.站名.Equals(EndStation)                   
                    ).ToList();
                var sameRoute = listRouteMileage.Where(x => x.线路名.Equals(lastStation.线路名)).ToList();
                int idxOfLastStation = sameRoute.IndexOf(lastStation);
                if (idxOfLastStation < 0)
                    NormalPrint("Cannot find " + lastStation.ToString());
                else
                {
                    if ((idxOfLastStation > 0) && !allNextStations.Contains(sameRoute[idxOfLastStation - 1])) allNextStations.Add(sameRoute[idxOfLastStation - 1]);
                    if ((idxOfLastStation < sameRoute.Count - 1) && !allNextStations.Contains(sameRoute[idxOfLastStation + 1])) allNextStations.Add(sameRoute[idxOfLastStation + 1]);
                }

                // use all possible stations.
                foreach (var nextStation in allNextStations)
                {
                    //try
                    //{
                    ClassOneRoute newCopr = new ClassOneRoute(copr);
                    var res = newCopr.AddBack(nextStation);
                    switch (res)
                    {
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.different_station_and_different_route:
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.Same_station_in_same_route:
                            continue;
                        //break;
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.Normal:
                            // if we switch multiple routes in the same station, we ignore it!
                            if (last2ndStation.站名.Equals(nextStation.站名) && lastStation.站名.Equals(nextStation.站名))
                            {

                            }
                            else
                            {
                                // if we encounter first two station has a same name, ignore!
                                if (last2ndStation.站名.Equals(string.Empty) && lastStation.站名.Equals(nextStation.站名)) { }
                                else
                                    // if some station appears 3 times or more, we ignore it!
                                    if (newCopr.Stations.Where(x => x.站名.Equals(nextStation.站名)).Count() <= 2)
                                    sortedPossibleRoutes.Add(newCopr);
                            }
                            break;
                        default:
                            throw new Exception("Critical error, we are not designed to go here");

                    }
                    //}
                    //catch (DuplicateWaitObjectException dwoe) { continue; }
                    //catch (FormatException fe) { Console.WriteLine("Critical error, we are not designed to go here"); }
                }
                sortedPossibleRoutes.Sort((x, y) => (x.PassedRoutes - y.PassedRoutes));
            }

        }

        private void BwAddRoute_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            listBox.Items.Add(new TextBox() { Text = e.UserState.ToString() });
        }

        List<ClassOneRoute> GetRouteByTwoStation(string startStation, string endStation, double maxLength, string train_code, int maxRoutes)
        {

            List<ClassOneRoute> sortedPossibleRoutes = new List<ClassOneRoute>();
            List<ClassOneRoute> allPossibleRoutes = new List<ClassOneRoute>();
            var starts = listRouteMileage.Where(x => x.站名.Equals(startStation));
            foreach (var start in starts)
            {
                ClassOneRoute copr = new ClassOneRoute();
                copr.AddBack(start);
                sortedPossibleRoutes.Add(copr);
            }


            var lastStation = new Class线路里程();
            var last2ndStation = new Class线路里程();

            while (sortedPossibleRoutes.Count > 0)
            {
                ClassOneRoute copr = sortedPossibleRoutes.First();
                sortedPossibleRoutes.RemoveAt(0);

                lastStation = copr.Stations.Last();
                if (copr.Stations.Count >= 2)
                    last2ndStation = copr.Stations[copr.Stations.Count - 2];
                else
                    last2ndStation = new Class线路里程();

                // if we encountered the endpoint, we simply add it to allPossibleRoutes
                if (lastStation.站名.Equals(endStation))
                {
                    allPossibleRoutes.Add(copr);
                    DebugPrint(copr);
                    if (allPossibleRoutes.Count > maxRoutes) return allPossibleRoutes;
                    continue;
                }

                // if the length reaches max, ignore it!
                if (copr.Length > maxLength || copr.PassedRoutes > MAX_ROUTES_BETWEEN_STATION) { continue; }
                // else, add all the other station in the same line
                // or in the same station.

                var allNextStations = listRouteMileage.Where(
                    // same station，并且是接算站
                    x => //(listStationName.Where(y => y.站名.Equals(x.站名)).Count() == 1) &&

                        x.站名.Equals(lastStation.站名) && !x.线路名.Equals(lastStation.线路名)
                        && (listStationName.Where(y => y.站名.Equals(x.站名)).FirstOrDefault()?.是否接算站).GetValueOrDefault(false)
                    // 并且不能是不办理客运的线路连接点站
                    // 考虑到杜家坎是可以走的但是不能调头，因此此处取消该条件。从而得到的结果是在线路连接点允许调头
                    //&& (!listStationName.Where(y => y.站名.Equals(x.站名)).FirstOrDefault()?.Property.Contains("△")).GetValueOrDefault(false)
                    //// or same route but it is a interchange station.
                    //(x.线路名.Equals(lastStation.线路名) &&
                    //    listInterchangeStation.Where(y => y.站名.Equals(x.站名)).Count() > 0) ||
                    //// or it is the endpoint
                    //x.站名.Equals(EndStation)                   
                    ).ToList();
                var sameRoute = listRouteMileage.Where(x => x.线路名.Equals(lastStation.线路名)).ToList();
                int idxOfLastStation = sameRoute.IndexOf(lastStation);
                if (idxOfLastStation < 0)
                    NormalPrint("Cannot find " + lastStation.ToString());
                else
                {
                    if ((idxOfLastStation > 0) && !allNextStations.Contains(sameRoute[idxOfLastStation - 1])) allNextStations.Add(sameRoute[idxOfLastStation - 1]);
                    if ((idxOfLastStation < sameRoute.Count - 1) && !allNextStations.Contains(sameRoute[idxOfLastStation + 1])) allNextStations.Add(sameRoute[idxOfLastStation + 1]);
                }

                // if this train is not D/C/G, then 不进入高速线路
                if (train_code.IndexOfAny(new char[] { 'D', 'C', 'G' }) < 0)  // normal train number
                {
                    allNextStations.RemoveAll(x => x.线路名.Contains("高速"));
                    // K1278 好像是经过昌九城际线路
                    //allNextStations.RemoveAll(x => x.线路名.Contains("城际"));
                }
                else // D/C/G车辆偏好高速和城际线路
                {

                }

                // use all possible stations.
                foreach (var nextStation in allNextStations)
                {
                    //try
                    //{
                    ClassOneRoute newCopr = new ClassOneRoute(copr);
                    var res = newCopr.AddBack(nextStation);
                    switch (res)
                    {
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.different_station_and_different_route:
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.Same_station_in_same_route:
                            continue;
                        //break;
                        case ClassOneRoute.ClassOnePossibleRouteReturnStatus.Normal:
                            // if we switch multiple routes in the same station, we ignore it!
                            if (last2ndStation.站名.Equals(nextStation.站名) && lastStation.站名.Equals(nextStation.站名))
                            {

                            }
                            else
                            {
                                // if we encounter first two station has a same name, ignore!
                                if (last2ndStation.站名.Equals(string.Empty) && lastStation.站名.Equals(nextStation.站名)) { }
                                else
                                    // if some station appears 3 times or more, we ignore it!
                                    if (newCopr.Stations.Where(x => x.站名.Equals(nextStation.站名)).Count() <= 2)
                                    sortedPossibleRoutes.Add(newCopr);
                            }
                            break;
                        default:
                            throw new Exception("Critical error, we are not designed to go here");

                    }
                    //}
                    //catch (DuplicateWaitObjectException dwoe) { continue; }
                    //catch (FormatException fe) { Console.WriteLine("Critical error, we are not designed to go here"); }
                }
                sortedPossibleRoutes.Sort((x, y) => (x.PassedRoutes - y.PassedRoutes));
            }

            if (allPossibleRoutes.Count < 1) throw new KeyNotFoundException("Cannot find a proper path under current condition, max change route:" + MAX_ROUTES_BETWEEN_STATION.ToString());
            return allPossibleRoutes;
        }

        private void buttonReload_Click(object sender, RoutedEventArgs e)
        {
            comboBoxFrom.ItemsSource = listStationName;
        }
        


        List<ClassOneRoute> 创建经由(string[] 经过站点)
        {
            if (经过站点.Length < 2) return null;

            List<ClassOneRoute> ret = new List<ClassOneRoute>();
            if (经过站点.Length == 2)
            {
                var starts = listRouteMileage.Where(x => x.站名.Equals(经过站点[0]));
                var ends = listRouteMileage.Where(x => x.站名.Equals(经过站点[1]));

                foreach(var start in starts)
                {
                    foreach(var end in ends)
                    {
                        if (start.线路名 == end.线路名)
                            ret.Add(new ClassOneRoute()
                            {
                                Length = Math.Abs(end.距起始站里程 - start.距起始站里程),
                                Stations = new List<Class线路里程>() { start, end }
                            });
                    }
                }

                if (ret.Count > 0)
                    return ret; // we can have direct routes from start to end.
            }


            return null;
        }


        const int MAX_ROUTES_BETWEEN_STATION = 4;
        bool DEBUG = true;
        void DebugPrint(object str)
        {
            if (DEBUG) { NormalPrint("[DEBUG] " + str); }
        }

        void NormalPrint(object obj)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + obj);
        }
    }
}
