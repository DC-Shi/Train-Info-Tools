using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 客里表Library;
using 客里表Library.Database;

namespace 客里表WPF版.Class
{
    class ViewModel客里表数据
    {
        #region Properties

        /// <summary>
        /// 后端数据库
        /// </summary>
        ClassDatabase classDB { get; }

        /// <summary>
        /// 所有站名，每项包括站名及附加信息
        /// </summary>
        public List<Class站名加> 所有站名 { get; set; } = new List<Class站名加>();

        /// <summary>
        /// 所有线路，只读列表
        /// </summary>
        public List<Class线路名> 所有线路
        {
            get
            {
                return classDB.ListRouteName;
            }
        }

        /// <summary>
        /// 所有线路里程，只读列表
        /// </summary>
        public List<Class线路里程> 所有线路里程
        {
            get
            {
                return classDB.ListRouteMileage;
            }
        }

        #endregion

        /// <summary>
        /// 构造函数，初始化数据库，并给站名添加附加信息
        /// </summary>
        public ViewModel客里表数据()
        {
            classDB = new ClassDatabase("data.mdb");
            for(int i = 0; i < classDB.ListStationName.Count; i++)
            {
                var this站名加 = new Class站名加(classDB.ListStationName[i], classDB.ListRouteMileage);
                所有站名.Add(this站名加);
            }
        }

    }
}
