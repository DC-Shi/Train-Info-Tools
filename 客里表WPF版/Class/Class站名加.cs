using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 客里表Library.Database;

namespace 客里表WPF版.Class
{
    /// <summary>
    /// 单个车站信息，包含了程序显示所需要的附加信息。
    /// </summary>
    class Class站名加 : Class站名
    {
        #region Properties

        /// <summary>
        /// 车站所属路线
        /// </summary>
        public IEnumerable<Class线路里程> 所属路线 { get; set; }

        public bool 是否允许办理G车 { get; set; }
        
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Class站名加()
        {

        }

        /// <summary>
        /// Construct from Class站名
        /// </summary>
        /// <param name="cz">父类实例</param>
        Class站名加(Class站名 cz)
        {
            this.所属局 = cz.所属局;
            this.所属行政区域 = cz.所属行政区域;
            this.拼音码 = cz.拼音码;
            this.是否接算站 = cz.是否接算站;
            this.电报码 = cz.电报码;
            this.站名 = cz.站名;
            this.营业办理限制 = cz.营业办理限制;
            this.车站代码 = cz.车站代码;
        }

        /// <summary>
        /// 利用父类实例构建本类，并初始化所属路线
        /// </summary>
        /// <param name="cz">Class站名</param>
        /// <param name="cxllc">包含该站点的线路里程列表，方便起见可以传入整个列表</param>
        public Class站名加(Class站名 cz, List<Class线路里程> cxllc) : this(cz)
        {
            所属路线 = cxllc.Where(x => x.站名 == 站名);
            是否允许办理G车 = 营业办理限制.Contains("G");
        }
        #endregion
    }
}
