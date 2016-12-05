using 客里表Library.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

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
        }

        #endregion
    }
}
