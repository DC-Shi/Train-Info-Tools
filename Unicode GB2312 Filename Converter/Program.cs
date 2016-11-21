using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Data.OleDb;
using System.Data;

namespace Unicode_GB2312_Filename_Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    LoadMDB(args[0], "stations.list");
                    break;
                case 2:
                    LoadMDB(args[0], args[1]);
                    break;
                default:
                    Console.WriteLine("Usage:");
                    Console.WriteLine("args[0](required): mdb file path.");
                    Console.WriteLine("args[1](optional): txt output file path, this would would be OVERWRITTEN!");
                    Console.WriteLine("args[1] default value is 'stations.list'");
                    return;
            }
            Console.WriteLine("Finished.");
            Console.ReadLine();
        }

        static void LoadMDB(string mdbPath, string outputTextPath)
        {
            /// Variables to access the database.
            OleDbConnection conn = null;
            OleDbDataAdapter adapter;
            //DataTable dtMain;

            try
            {
                /// Connect to DB
                conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath);
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
                                case "站名":
                                    foreach (DataRow dr in dtMain.Rows)
                                    {
                                        /// 必须用\n，从而保证Linux下不出错。
                                        File.AppendAllText(outputTextPath, string.Format(
                                            "%{0}%E7%AB%99\n",
                                            BitConverter.ToString(Encoding.UTF8.GetBytes(dr["站名"].ToString())).Replace('-', '%')
                                            ));
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
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
    }
}
