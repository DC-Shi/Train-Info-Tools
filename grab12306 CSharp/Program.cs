using System;
using System.IO;

namespace Parse12306_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ClassParse12306 cp12306 = new ClassParse12306();

            cp12306.DownloadUriAsync(new Uri(ClassStation.Url_station_name_js), ClassStation.Filename_station_name_js, false);
            cp12306.DownloadUriAsync(new Uri(cp12306.URL_train_list_js), cp12306.Filename_train_list_js, false);
            
            cp12306.SplitWholeFileByDate(cp12306.Filename_train_list_js);
            foreach (string oneDateFile in Directory.EnumerateFiles("Date", "*.js", SearchOption.TopDirectoryOnly))
            {
                string currentDate = Path.GetFileNameWithoutExtension(oneDateFile);

                // Print currentDate:
                Console.WriteLine("Found train data in " + currentDate);

                // Only capture tomorrow
                if (!currentDate.Equals(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"))) continue;
                var allTrain = cp12306.ParseTrainList(oneDateFile);
                foreach (var train in allTrain)
                {
                    cp12306.DownloadTrainInfo(DateTime.Parse(currentDate), train);
                }
                //break;
            }
            
            
            Console.WriteLine("Finished. Press any key to exit.");
            Console.ReadKey();
        }

        static void test()
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd"));
        }
    }
}
