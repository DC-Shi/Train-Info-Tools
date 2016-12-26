using System;
using System.Collections.Generic;
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

namespace 客里表WPF版
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            classDB = new ClassDatabase("data.mdb");
            textBlockStatus.Text += ("Database loaded.");


            listBox线路列表.ItemsSource = classDB.ListRouteName;
            listBox车站列表.ItemsSource = classDB.ListStationName;
        }

        ClassDatabase classDB = null;

        private void textBox输入框_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var inputText = textBox输入框.Text.ToUpper();
            //var resultStation = classDB.ListStationName.Where(x => x.拼音码.Contains(inputText) || x.电报码.Contains(inputText) || x.站名.Contains(inputText) || x.车站代码.Contains(inputText));


            //listBox车站列表.ItemsSource = resultStation.Take(100);

            /// http://stackoverflow.com/questions/2008481/how-do-i-filter-listview-in-wpf
            /// 
            var view = CollectionViewSource.GetDefaultView(listBox车站列表.ItemsSource);
            var inputText = textBox输入框.Text.ToUpper();

            view.Filter = x =>
            {
                var czm = x as Class站名;
                return czm.拼音码.Contains(inputText)
                || czm.电报码.Contains(inputText)
                || czm.站名.Contains(inputText)
                || czm.车站代码.Contains(inputText);
            };

            view.Refresh();

            //var resultRoute = classDB.ListRouteName.Where(x => x.线路名.Contains(inputText));
            //if (resultRoute.Count() > 0)
            //{
            //    listBox线路列表.ItemsSource = resultRoute;
            //}
        }

        //private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ClickCount == 2)
        //    {
        //        var selectedRoute = classDB.ListRouteMileage.Where(x => x.线路名.Equals((listBox线路列表.SelectedItem as Class线路名).线路名));
        //        if (selectedRoute.Count() > 0)
        //        {
        //            listViewTable.ItemsSource = selectedRoute.ToArray();
        //        }
        //        else
        //        {
        //            Console.WriteLine("目标为空，找不到线路名{0}", (listBox线路列表.SelectedItem as Class线路名).线路名);
        //        }
        //    }
        //}

        private void textBox输入框_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (listBox车站列表.Items.Count > 0)
                {
                    listBox车站列表.Focus();
                    listBox车站列表.SelectedIndex = 0;
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


                view.Filter = x =>
                {
                    var cxlm = x as Class线路名;
                    var ccz = listBox车站列表.SelectedItem as Class站名;
                    if (cxlm == null || ccz == null) return false;
                    if (classDB.ListRouteMileage.Where(t => t.线路名 == cxlm.线路名 && t.站名 == ccz.站名).Count() > 0)
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
                var selectedRoute = classDB.ListRouteMileage.Where(x => x.线路名.Equals((listBox线路列表.SelectedItem as Class线路名).线路名));
                if (selectedRoute.Count() > 0)
                {
                    listViewTable.ItemsSource = selectedRoute;

                    var czm = listBox车站列表.SelectedItem as Class站名;
                    if (czm != null)
                    {
                        for (int i = 0; i < listViewTable.Items.Count; i++)
                        {
                            if (listViewTable.Items[i] is Class线路里程)
                            {
                                if ((listViewTable.Items[i] as Class线路里程).站名 == czm.站名)
                                {
                                    listViewTable.Focus();
                                    listViewTable.ScrollIntoView(listViewTable.Items[i]);
                                    listViewTable.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("目标为空，找不到线路名{0}", (listBox线路列表.SelectedItem as Class线路名).线路名);
                }
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
                    textBox输入框_KeyDown(sender, e);
                }
            }
        }
    }
}
