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


            /// 加载完成数据之后，计算G车允许办理的情况：
            /// 如果相邻两个G之间的所有车站包括了：
            /// 叉：说明该站不办理货运，应该是乘降所或者动车站
            /// 三角：线路链接所
            /// 接续线路超过1个：说明这是线路连接点，有可能是动车枢纽站（普速高速均停车的）
            /// 这三类的话，
            /// 那么这两个G之间的所有车站都是允许G车运行的车站。
            /// 
            /// 如果该线路一段为G标志，其他不包含G标志，且只有这三类的话，那么该线路可以办理G车（或者C车）
            /// 
            Queue<Class站名加> queue待查站名 = new Queue<Class站名加>(所有站名.Where(x => x.是否允许办理G车));

            while (queue待查站名.Count > 0)
            {
                var cur站 = queue待查站名.Dequeue();
                /// 检查当前站的所有路线
                foreach (var cur线 in cur站.所属路线)
                {
                    /// 获取当前线路的里程站点信息
                    var cur线路里程 = 所有线路里程.Where(x => x.线路名 == cur线.线路名).ToList();
                    /// 获取当前站点在这条线上的哪个位置
                    int curIdx = cur线路里程.IndexOf(cur线);

                    /// 获取上一个与下一个可以通行G车的站点
                    int previousIdx = curIdx - 1;
                    int nextIdx = curIdx + 1;

                    /// 往前查找符合条件的站
                    Class站名加 previous站 = null;
                    while (previousIdx >= 0)
                    {
                        /// 白鹿塘站不在所有站名当中
                        /// 那个客里表程序是显示了空白
                        var possibleprevious站 = 所有站名.Where(x => x.站名 == cur线路里程[previousIdx].站名);
                        if (possibleprevious站.Count() == 0) break;
                        
                        previous站 = possibleprevious站.First();
                        /// 如果当前车站已经是可以办理G车的，那么就可以跳出来了
                        if (previous站.是否允许办理G车)
                            break;
                        /// 如果当前车站是符合那三类条件的，那么继续往前或往后
                        if (previous站.营业办理限制.Contains("×") ||
                            previous站.营业办理限制.Contains("△") ||
                            previous站.所属路线.Count() > 1)
                            previousIdx--;
                        /// 当前车站不是上述3类，没必要继续往前搜索
                        else
                            break;
                    }
                    /// 如果找到的站允许办理G车，或者previousIdx已经小于0了（说明到头了也均可办理）
                    /// 那么说明previous到cur这一段均可办理G车，因此均包括进来
                    if (
                        previous站 != null &&
                        (previousIdx < 0 || previous站.是否允许办理G车)
                        )
                    {
                        /// 那么从previous到cur之前的车站均可视为允许G车的
                        for (int i = previousIdx + 1; i < curIdx; i++)
                        {
                            previous站 = 所有站名.Where(x => x.站名 == cur线路里程[i].站名).First();
                            if (!previous站.是否允许办理G车)
                            {
                                /// 我们将一个站点的属性改变了，因此其他站点也有可能受影响，因此加入到队列当中
                                /// 这样做就能让霸徐线变为允许G通过的线路了。
                                queue待查站名.Enqueue(previous站);
                            }
                            previous站.是否允许办理G车 = true;
                        }
                    }


                    /// 往后查找符合条件的站
                    Class站名加 next站 = null;
                    while (nextIdx < cur线路里程.Count)
                    {
                        /// 白鹿塘站不在所有站名当中
                        /// 那个客里表程序是显示了空白
                        var possiblenext站 = 所有站名.Where(x => x.站名 == cur线路里程[nextIdx].站名);
                        if (possiblenext站.Count() == 0) break;

                        next站 = possiblenext站.First();
                        /// 如果当前车站已经是可以办理G车的，那么就可以跳出来了
                        if (next站.是否允许办理G车)
                            break;
                        /// 如果当前车站是符合那三类条件的，那么继续往前或往后
                        if (next站.营业办理限制.Contains("×") ||
                            next站.营业办理限制.Contains("△") ||
                            next站.所属路线.Count() > 1)
                            nextIdx++;
                        /// 当前车站不是上述3类，没必要继续往前搜索
                        else
                            break;
                    }
                    /// 如果找到的站允许办理G车，或者previousIdx已经小于0了（说明到头了也均可办理）
                    /// 那么说明previous到cur这一段均可办理G车，因此均包括进来
                    if (
                        next站 != null &&
                        (nextIdx >= cur线路里程.Count || next站.是否允许办理G车)
                        )
                    {
                        /// 那么从previous到cur之前的车站均可视为允许G车的
                        for (int i = curIdx + 1; i < nextIdx; i++)
                        {
                            next站 = 所有站名.Where(x => x.站名 == cur线路里程[i].站名).First();
                            if (!next站.是否允许办理G车)
                            {
                                /// 我们将一个站点的属性改变了，因此其他站点也有可能受影响，因此加入到队列当中
                                /// 这样做就能让霸徐线变为允许G通过的线路了。
                                queue待查站名.Enqueue(next站);
                            }
                            next站.是否允许办理G车 = true;
                        }
                    }
                }
            }
        }

    }
}
