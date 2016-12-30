using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using 客里表Library;
using 客里表Library.Database;
using 客里表WPF版.Class;

namespace 客里表WPF版
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// 数据库的ViewModel
        /// </summary>
        ViewModel客里表数据 vm = null;

        /// <summary>
        /// BackgroundWorker, 因为修改了ViewModel，因此加载数据变慢了，用后台加载
        /// </summary>
        BackgroundWorker bwLoadData = new BackgroundWorker();

        /// <summary>
        /// 构造函数，初始化可视化组件，初始化数据库ViewModel
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            textBlockStatus.Text = ("Database is loading.");

            bwLoadData.DoWork += BwLoadData_DoWork;
            bwLoadData.RunWorkerCompleted += BwLoadData_RunWorkerCompleted;
            bwLoadData.RunWorkerAsync();
        }

        private void BwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DataContext = vm;
            textBlockStatus.Text = ("Database loaded.");

            FocusF3(this, null);
        }

        private void BwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            vm = new ViewModel客里表数据();
        }

        /// <summary>
        /// http://stackoverflow.com/questions/3386206/wpf-assign-f-keys-to-buttons
        /// Focus the textbox when F3 pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FocusF3(object sender, RoutedEventArgs e)
        {
            textBox输入框.Focus();
            textBox输入框.SelectAll();
        }
        

        private void textBox输入框_TextChanged(object sender, TextChangedEventArgs e)
        {
            /// http://stackoverflow.com/questions/2008481/how-do-i-filter-listview-in-wpf
            /// 
            var view = CollectionViewSource.GetDefaultView(listBox车站列表.ItemsSource);
            var inputText = textBox输入框.Text.ToUpper();

            /// view might be null if no item presented.
            if (view == null) return;

            view.Filter = x =>
            {
                var czm = x as Class站名;
                return czm.拼音码.Contains(inputText)
                || czm.电报码.Contains(inputText)
                || czm.站名.Contains(inputText)
                || czm.车站代码.Contains(inputText);
            };

            view.Refresh();
        }
        

        private void textBox输入框_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (listBox车站列表.Items.Count > 0)
                {
                    listBox车站列表.Focus();
                    listBox车站列表.SelectedIndex = 0;
                }
                /// 如果当前只有一个结果，那么自动移到线路选择当中
                if (listBox车站列表.Items.Count == 1)
                {
                    listBox车站列表_KeyDown(sender, e);
                }
            }
        }

        private void listBox车站列表_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                /// http://stackoverflow.com/questions/2008481/how-do-i-filter-listview-in-wpf
                /// 
                var view = CollectionViewSource.GetDefaultView(listBox线路列表.ItemsSource);

                /// view might be null if no item presented.
                if (view == null) return;

                view.Filter = x =>
                {
                    var cxlm = x as Class线路名;
                    var ccz = listBox车站列表.SelectedItem as Class站名;
                    if (cxlm == null || ccz == null) return false;
                    if (vm.所有线路里程.Where(t => t.线路名 == cxlm.线路名 && t.站名 == ccz.站名).Count() > 0)
                        return true;
                    return false;
                };

                view.Refresh();


                if (listBox线路列表.Items.Count > 0)
                {
                    listBox线路列表.Focus();
                    listBox线路列表.SelectedIndex = 0;

                    /// 只有一个结果，直接切到table
                    if (listBox线路列表.Items.Count == 1)
                    {
                        listBox线路列表_KeyDown(sender, e);
                    }
                }
            }
        }
        

        private void listBox线路列表_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                /// 先找当前选择的线路所包含的站点
                var selectedRoute = vm.所有线路里程.Where(x => x.线路名.Equals((listBox线路列表.SelectedItem as Class线路名)?.线路名));
                if (selectedRoute.Count() > 0)
                {
                    /// 找到之后进行绑定
                    listViewTable.ItemsSource = selectedRoute;

                    var czm = listBox车站列表.SelectedItem as Class站名;

                    /// Fix bug that no station selected
                    int i = 0;
                    if (czm != null)
                    {
                        for (i = 0; i < listViewTable.Items.Count; i++)
                        {
                            if (listViewTable.Items[i] is Class线路里程)
                            {
                                /// 然后查找与所选内容一致的站点
                                if ((listViewTable.Items[i] as Class线路里程).站名 == czm.站名)
                                {
                                    break;
                                }
                            }
                        }
                        /// 如果没找到，那就默认第一个
                        if (i >= listViewTable.Items.Count) i = 0;

                    }
                    /// 设置线路名称
                    textBox线路名称.Text = (listViewTable.Items[i] as Class线路里程).线路名;

                    listViewTable.Focus();
                    listViewTable.ScrollIntoView(listViewTable.Items[i]);

                    /// http://stackoverflow.com/questions/7363777/arrow-keys-dont-work-after-programmatically-setting-listview-selecteditem/7364949#7364949
                    this.listViewTable.ItemContainerGenerator.StatusChanged += icg_StatusChanged;
                    this.listViewTable.SelectedItem = listViewTable.Items[i];
                    current线路里程 = listViewTable.SelectedItem as Class线路里程;
                }
                else
                {
                    Console.WriteLine("目标为空，找不到线路名{0}", (listBox线路列表.SelectedItem as Class线路名)?.线路名);
                }
            }
        }
        
        #region INotifyPropertyChanged implementation.

        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>
        /// 利用DependencyProperty来实现DataBinding
        /// http://stackoverflow.com/questions/13325617/i-cant-data-bind-to-a-local-variable-in-wpf-xaml
        /// </summary>
        public Class线路里程 current线路里程
        {
            get { return (Class线路里程)GetValue(current线路里程Property); }
            set { SetValue(current线路里程Property, value); }
        }

        // Using a DependencyProperty as the backing store for myText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty current线路里程Property =
            DependencyProperty.Register("current线路里程", typeof(Class线路里程), typeof(MainWindow), new PropertyMetadata(null));


        /// <summary>
        /// http://stackoverflow.com/questions/7363777/arrow-keys-dont-work-after-programmatically-setting-listview-selecteditem/7364949#7364949
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void icg_StatusChanged(object sender, EventArgs e)
        {
            if (this.listViewTable.ItemContainerGenerator.Status
                == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                this.listViewTable.ItemContainerGenerator.StatusChanged
                    -= icg_StatusChanged;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input,
                                       new Action(() => {
                                           var uielt = (UIElement)this.listViewTable.ItemContainerGenerator.ContainerFromItem(current线路里程);
                                           uielt.Focus();
                                       }));
            }
        }

        private void listViewTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var selectedClass线路里程 = listViewTable.SelectedItem as Class线路里程;
                if (selectedClass线路里程 != null)
                {
                    textBox输入框.Text = selectedClass线路里程.站名;

                    /// 仅显示当前车站
                    var view = CollectionViewSource.GetDefaultView(listBox车站列表.ItemsSource);

                    /// view might be null if no item presented.
                    if (view == null) return;

                    view.Filter = x =>
                    {
                        var czm = x as Class站名;
                        return czm.站名 == selectedClass线路里程.站名;
                    };

                    view.Refresh();

                    listBox车站列表.SelectedIndex = 0;
                    /// 然后模拟车站列表按回车
                    listBox车站列表_KeyDown(sender, e);
                }
            }
        }
    }
}
