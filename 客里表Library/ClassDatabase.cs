using 客里表Library.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Linq;

namespace 客里表Library
{
    public class ClassDatabase
    {
        #region Private members

        List<Class线路里程> listRouteMileage = new List<Class线路里程>();
        List<Class线路名> listRouteName = new List<Class线路名>();
        List<Class站名> listStationName = new List<Class站名>();

        #endregion

        #region Public members

        /// <summary>
        /// List of Class线路里程.
        /// </summary>
        public List<Class线路里程> ListRouteMileage
        {
            get
            {
                return listRouteMileage;
            }

            set
            {
                listRouteMileage = value;
            }
        }

        /// <summary>
        /// List of Class线路名
        /// </summary>
        public List<Class线路名> ListRouteName
        {
            get
            {
                return listRouteName;
            }

            set
            {
                listRouteName = value;
            }
        }

        /// <summary>
        /// List of Class站名
        /// </summary>
        public List<Class站名> ListStationName
        {
            get
            {
                return listStationName;
            }

            set
            {
                listStationName = value;
            }
        }

        #endregion

        /// <summary>
        /// Construct from database
        /// </summary>
        /// <param name="str">Database file name, should end with .mdb</param>
        public ClassDatabase(string str)
        {
            LoadMDB(str);
        }

        #region Load database methods.
        
        /// <summary>
        /// Setup OLE to Access database asynchronously.
        /// </summary>
        /// <param name="DBPath">Database file path.</param>
        void LoadMDB(string DBPath)
        {
            /// Variables to access the database.
            OleDbConnection conn = null;
            OleDbDataAdapter adapter;
            //DataTable dtMain;

            try
            {
                /// Connect to DB
                conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath);
                conn.Open();

                /// Get all tables from DB
                using (DataTable dt = conn.GetSchema("Tables"))
                {
                    /// For each table
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        /// There are many tables, we only need "TABLE", not "SYSTEM TABLE" or other elements.
                        if (dt.Rows[i].ItemArray[dt.Columns.IndexOf("TABLE_TYPE")].ToString() == "TABLE")
                        {
                            /// Get current table name.
                            string table_name = dt.Rows[i].ItemArray[dt.Columns.IndexOf("TABLE_NAME")].ToString();

                            /// Select current table command.
                            adapter = new OleDbDataAdapter("SELECT * FROM [" + table_name + "]", conn);
                            /// Execute the command.
                            new OleDbCommandBuilder(adapter);

                            /// Create a new data table,
                            DataTable dtMain = new DataTable();
                            /// and fill it.
                            adapter.Fill(dtMain);

                            /// Create different variables depends on their table name.
                            switch (table_name)
                            {
                                case "线路里程":
                                    foreach (DataRow dr in dtMain.Rows)
                                    {
                                        ListRouteMileage.Add(new Class线路里程(dr));
                                    }
                                    break;
                                case "线路名":
                                    foreach (DataRow dr in dtMain.Rows)
                                    {
                                        ListRouteName.Add(new Class线路名(dr));
                                    }
                                    break;
                                case "站名":
                                    foreach (DataRow dr in dtMain.Rows)
                                    {
                                        ListStationName.Add(new Class站名(dr));
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    /// Sort the list by 距起始站里程
                    ListRouteMileage.Sort((x, y) => { return (int)(x.距起始站里程 - y.距起始站里程); });
                }
            }
            catch (OleDbException ode)
            {
                Console.WriteLine(ode.ToString());
            }
            finally
            {
                conn?.Close();
            }


            /// 查找剩余没有在线路列表当中的线路
            /// 
            foreach(var stationMile in ListRouteMileage)
            {
                if (ListRouteName.FindIndex(route => route.线路名 == stationMile.线路名) < 0)
                {
                    /// 先查找该站点所在线路的所有站点
                    /// 
                    var allStations = ListRouteMileage.Where(lrm => lrm.线路名 == stationMile.线路名);

                    /// 如果当前线路恰好两站，那么一个为头，一个为尾；否则不添加该线路
                    if (allStations.Count() == 2)
                    {
                        var sortedStations = allStations.ToList();
                        sortedStations.Sort((x, y) => (int)(x.距起始站里程 - y.距起始站里程));
                        /// 有两条线路没有在所有线路当中出现，因此手动添加
                        /// 鉴于最大序号246，用248作为最大序号
                        /// 
                        Class线路名 新线路 = new Class线路名();
                        新线路.备注 = "该线路为程序自动补充";
                        新线路.序号 = "248";
                        新线路.是否窄轨 = false;
                        新线路.线路名 = stationMile.线路名;
                        新线路.终止端站名 = sortedStations[1].站名;
                        新线路.起始端站名 = sortedStations[0].站名;
                        新线路.里程 = sortedStations[1].距起始站里程 - sortedStations[0].距起始站里程;

                        ListRouteName.Add(新线路);
                    }
                }
            }

            /// 查找线路列表当中哪些线路没有车站
            /// 只有郑荥线是没有在任何车站当中出现过的
            //foreach (var route in ListRouteName)
            //{
            //    if (ListRouteMileage.FindIndex(x => x.线路名 == route.线路名) < 0)
            //    {
            //        Console.WriteLine(route.线路名);
            //    }
            //}

        }

        #endregion
    }
}
