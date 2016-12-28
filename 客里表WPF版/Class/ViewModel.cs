using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 客里表Library;
using 客里表Library.Database;

namespace 客里表WPF版.Class
{
    class ViewModel
    {
        #region Properties

        ClassDatabase classDB { get; }

        public List<Class站名加> 所有站名 { get; set; } = new List<Class站名加>();

        public List<Class线路名> 所有线路
        {
            get
            {
                return classDB.ListRouteName;
            }
        }

        public List<Class线路里程> 所有线路里程
        {
            get
            {
                return classDB.ListRouteMileage;
            }
        }

        #endregion

        public ViewModel()
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
