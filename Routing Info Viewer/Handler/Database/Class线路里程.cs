using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Info_Viewer.Handler.Database
{
    class Class线路里程
    {
        #region Properties

        /// <summary>
        /// 线路名称
        /// </summary>
        public string 线路名 { get; set; }

        /// <summary>
        /// 车站名称
        /// </summary>
        public string 站名 { get; set; }

        /// <summary>
        /// 从起始站到本站的里程
        /// </summary>
        public double 距起始站里程 { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Class线路里程()
        {
            线路名 = string.Empty;
            站名 = string.Empty;
            距起始站里程 = 0;
        }

        /// <summary>
        /// Construct from DataRow
        /// </summary>
        /// <param name="dr">One data record of 线路里程</param>
        public Class线路里程(DataRow dr)
        {
            线路名 = dr["线路名"].ToString();
            站名 = dr["站名"].ToString();
            距起始站里程 = double.Parse(dr["距起始站里程"].ToString());
        }
        #endregion

        #region Override functions.

        public override string ToString()
        {
            return string.Format("{0}({1}@{2}km)", 站名, 线路名, 距起始站里程);
        }
        public override bool Equals(object obj)
        {
            return 线路名.Equals(((Class线路里程)obj).线路名)
                && 站名.Equals(((Class线路里程)obj).站名)
                && 距起始站里程.Equals(((Class线路里程)obj).距起始站里程);
        }
        public override int GetHashCode()
        {
            return 线路名.GetHashCode() ^ 站名.GetHashCode() ^ 距起始站里程.GetHashCode();
        }

        #endregion
    }
}
