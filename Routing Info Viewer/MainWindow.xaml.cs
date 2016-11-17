using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Routing_Info_Viewer.Handler.Database;
using Routing_Info_Viewer.Handler;
using System.ComponentModel;

namespace Routing_Info_Viewer
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
            textBlockResult.Text += ("Database loaded.");
        }
        

        ClassDatabase classDB = null;
        private void buttonReload_Click(object sender, RoutedEventArgs e)
        {
            comboBoxFrom.ItemsSource = classDB.ListStationName;
        }
        




        const int MAX_ROUTES_BETWEEN_STATION = 4;
        bool DEBUG = true;
        void DebugPrint(object str)
        {
            if (DEBUG) { NormalPrint("[DEBUG] " + str); }
        }

        void NormalPrint(object obj)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + obj);
        }
        
        private void buttonNewMethod_Click(object sender, RoutedEventArgs e)
        {
            ClassMultipleRoutes cmr = new ClassMultipleRoutes();
            if(listBox.DataContext is ClassMultipleRoutes)
            {
                (listBox.DataContext as ClassMultipleRoutes).Timeout = 0;
            }
            listBox.DataContext = cmr;
            try {
                cmr.StartStation = textBoxFrom.Text;
                cmr.EndStation = textBoxTo.Text;
                cmr.MaxCount = int.Parse(textBoxMaxCount.Text);
                cmr.MaxLength = double.Parse(textBoxMaxLength.Text);
                cmr.MaxTransfers = int.Parse(textBoxMaxTransfers.Text);
                cmr.Timeout = double.Parse(textBoxTimeout.Text);
                cmr.FindInBackground(classDB);
            }
            catch(FormatException fe)
            {
                textBlockResult.Text = fe.Message;
            }
        }
    }
}
