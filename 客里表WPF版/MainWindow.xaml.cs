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
        }

        ClassDatabase classDB = null;

        private void textBox输入框_TextChanged(object sender, TextChangedEventArgs e)
        {
            var inputText = textBox输入框.Text.ToUpper();
            var resultStation = classDB.ListStationName.Where(x => x.拼音码.Contains(inputText) || x.电报码.Contains(inputText) || x.站名.Contains(inputText) || x.车站代码.Contains(inputText));


            if (resultStation.Count() > 0)
            {
                listBox车站列表.ItemsSource = resultStation.Take(100);
            }

            var resultRoute = classDB.ListRouteName.Where(x => x.线路名.Contains(inputText));
            if (resultRoute.Count() > 0)
            {
                listBox线路列表.ItemsSource = resultRoute;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var selectedRoute = classDB.ListRouteMileage.Where(x => x.线路名.Equals((listBox线路列表.SelectedItem as Class线路名).线路名));
                if (selectedRoute.Count() > 0)
                {
                    listViewTable.ItemsSource = selectedRoute.ToArray();
                }
                else
                {
                    Console.WriteLine("目标为空，找不到线路名{0}", (listBox线路列表.SelectedItem as Class线路名).线路名);
                }
            }
        }
    }
}
