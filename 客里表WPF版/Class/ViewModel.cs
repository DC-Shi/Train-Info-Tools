using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 客里表Library;

namespace 客里表WPF版.Class
{
    class ViewModel
    {
        public ClassDatabase classDB { get; set; }

        public ViewModel()
        {
            classDB = new ClassDatabase("data.mdb");
        }
    }
}
