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

        public bool Equals(Class线路里程 crm)
        {
            return 线路名.Equals(crm.线路名)
                && 站名.Equals(crm.站名)
                && 距起始站里程.Equals(crm.距起始站里程);
        }

        #region Override functions.

        public override string ToString()
        {
            return string.Format("{0}({1}@{2}km)", 站名, 线路名, 距起始站里程);
        }

        #endregion
    }
}
