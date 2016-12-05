using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 客里表Library.Database
{
    /// <summary>
    /// 单个车站信息
    /// </summary>
    public class Class站名
    {
        #region Properties

        /// <summary>
        /// 车站名称
        /// </summary>
        public string 站名 { get; set; }

        /// <summary>
        /// 拼音码
        /// </summary>
        public string 拼音码 { get; set; }

        /// <summary>
        /// 电报码
        /// </summary>
        public string 电报码 { get; set; }

        /// <summary>
        /// 车站代码
        /// </summary>
        public string 车站代码 { get; set; }

        /// <summary>
        /// 所属路局或公司
        /// </summary>
        public string 所属局 { get; set; }

        /// <summary>
        /// 是否为结算站
        /// </summary>
        public bool 是否接算站 { get; set; }

        /// <summary>
        /// 所属省份
        /// </summary>
        public string 所属行政区域 { get; set; }

        /// <summary>
        /// 营业办理限制
        /// </summary>
        public string 营业办理限制 { get; set; }

        #endregion


        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Class站名()
        {
            是否接算站 = false;
        }

        /// <summary>
        /// Construct from DataRow
        /// </summary>
        /// <param name="dr">One data record of 站名</param>
        public Class站名(DataRow dr)
        {
            站名 = dr["站名"].ToString();
            拼音码 = dr["拼音码"].ToString();
            电报码 = dr["电报码"].ToString();
            车站代码 = dr["车站代码"].ToString();
            所属局 = dr["所属局"].ToString();
            是否接算站 = bool.Parse(dr["是否接算站"].ToString());
            所属行政区域 = dr["所属行政区域"].ToString();
            营业办理限制 = dr["营业办理限制"].ToString();
        }
        #endregion


        #region Override functions.

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", 站名, 拼音码, 电报码);
        }

        #endregion
    }
}
