using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 客里表Library.Database
{
    class Class线路名
    {
        #region Properties

        /// <summary>
        /// 线路名称
        /// </summary>
        public string 线路名 { get; set; }

        /// <summary>
        /// 线路全长
        /// </summary>
        public double 里程 { get; set; }

        /// <summary>
        /// 起始站名
        /// </summary>
        public string 起始端站名 { get; set; }

        /// <summary>
        /// 终止站名
        /// </summary>
        public string 终止端站名 { get; set; }

        /// <summary>
        /// 是否窄轨
        /// </summary>
        public bool 是否窄轨 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string 备注 { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public string 序号 { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Class线路名()
        {
            备注 = "NULL";
        }

        /// <summary>
        /// Construct from DataRow
        /// </summary>
        /// <param name="dr">One data record of 线路名</param>
        public Class线路名(DataRow dr)
        {
            线路名 = dr["线路名"].ToString();
            里程 = double.Parse(dr["里程"].ToString());
            起始端站名 = dr["起始端站名"].ToString();
            终止端站名 = dr["终止端站名"].ToString();
            是否窄轨 = bool.Parse(dr["是否窄轨"].ToString());
            备注 = dr["备注"].ToString();
            序号 = dr["序号"].ToString();
        }
        #endregion
    }
}
