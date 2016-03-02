using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Parse12306_CSharp
{
    public class ClassParse12306
    {
        /// <summary>
        /// Initialize the class, skip the ssl check.
        /// <para>This might be vulnerable since we allowed every connection to untrusted sites.</para>
        /// </summary>
        public ClassParse12306()
        {
            // Since 12306 use SRCA certificate(which is not trusted if you have not install it) to protect the website,
            // consider the computers that have not installed the cert, we have to ignore the security check.
            ServicePointManager
                .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// Internal value that stores all station list.
        /// </summary>
        List<ClassStation> _allStations;
        /// <summary>
        /// Get or set the all station list.
        /// <para>It will load from station_name.js method when getting the value at the first time.</para>
        /// </summary>
        public List<ClassStation> allStations
        {
            get
            {
                if (_allStations == null || _allStations.Count == 0) // _allStation is empty
                {
                    ClassStation cs = new ClassStation();
                    _allStations = cs.ParseStationsByFile(ClassStation.Filename_station_name_js);
                }
                return _allStations;
            }
            set
            {
                _allStations = value;
            }
        }

        /// <summary>
        /// Train list URL <para/>
        /// https://kyfw.12306.cn/otn/resources/js/query/train_list.js
        /// </summary>
        public string URL_train_list_js
        {
            get
            {
                return "https://kyfw.12306.cn/otn/resources/js/query/train_list.js";
            }
        }
        /// <summary>
        /// Default saving path of train_list.js
        /// </summary>
        public string Filename_train_list_js
        {
            get; set;
        } = "train_list.js";

        #region Download Handling
        /// <summary>
        /// Download the specific <c>URI</c> to <c>outputFile</c>.
        /// </summary>
        /// <remarks>This method is not async method, it just use download async method, and can show the downloading progress.</remarks>
        /// <param name="uri">URI to download.</param>
        /// <param name="outputFile">The name of the file to be placed on the local computer.</param>
        /// <param name="forceUpdate">Whether the file should be force updated.</param>
        public void DownloadUriAsync(Uri uri, string outputFile, bool forceUpdate = false)
        {
            if (!forceUpdate && File.Exists(outputFile))    // If there is the file, and not forcely update, ignore this request.
                return;
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileTaskAsync(uri, outputFile).Wait();
            }
        }

        /// <summary>
        /// Using this to control the output of the progress.
        /// </summary>
        private DateTime _DownloadProgressUpdateTimes = DateTime.Now;
        private WebClient _DownloadURL = new WebClient();
        /// <summary>
        /// Download progress changed event handler, print the progress.
        /// </summary>
        /// <param name="sender">Event sender, we do not use this.</param>
        /// <param name="e">The event args.</param>
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            WebClient wc = sender as WebClient;
            if (wc != null)
            {
                if(!wc.Equals(_DownloadURL))    // If we started a new download, reset the update state.
                {
                    _DownloadURL = wc;
                    _DownloadProgressUpdateTimes = DateTime.Now;
                }
            }
            if ((DateTime.Now - _DownloadProgressUpdateTimes).Seconds > 1)
            {
                Console.WriteLine("Downloaded: {0}%", e.ProgressPercentage);
                _DownloadProgressUpdateTimes = DateTime.Now;
            }
        }
        #endregion

        /// <summary>
        /// Split the train_list.js(around 38MB) to several small js files by date, each is around 600KB.
        /// </summary>
        /// <param name="filePath">The file path of train_list.js</param>
        /// <param name="forceUpdate">Whether the output file should be updated.</param>
        public void SplitWholeFileByDate(string filePath, bool forceUpdate = false)
        {
            /// Read all the contents, around 38MB.
            string _allContents = File.ReadAllText(filePath);
            /// Find the first '{', and the substring is from this point.
            StringBuilder sb = new StringBuilder(_allContents);
            sb = sb.Remove(0, _allContents.IndexOf('{'));
            //allContents = allContents.Substring(allContents.IndexOf('{'));

            while (sb.Length > 0)
            {
                int braceCounter = 0;
                int idx = 1;
                if (sb.Length < 3) break;
                while (sb[idx] != '{') idx++;
                int idxStart = 1;
                braceCounter = 0;
                DateTime date = DateTime.Parse(sb.ToString().Substring(2, 10));
                /// Find the first and the last bracket
                do
                {
                    if (sb[idx] == '{') braceCounter++;
                    if (sb[idx] == '}') braceCounter--;
                    idx++;
                } while (braceCounter != 0 && idx < sb.Length);

                /// Save this part and continue to next.
                string curSection = sb.ToString().Substring(idxStart, idx - idxStart);
                sb = sb.Remove(0, idx);
                string dateFilename = "Date/" + date.ToString("yyyy-MM-dd") + ".js";
                if (!Directory.Exists("Date")) Directory.CreateDirectory("Date");

                if (!forceUpdate && File.Exists(dateFilename))
                    continue;
                File.WriteAllText(dateFilename, curSection.Replace("}", "}\r\n").Replace(":{", ":\r\n{"));
            }
            return;
        }

        /// <summary>
        /// Download the train info.
        /// </summary>
        /// <param name="departDate">Departure date.</param>
        /// <param name="te">Train information.</param>
        public void DownloadTrainInfo(DateTime departDate, ClassTrainEntry te)
        {
            /// 获取起点站和终点站的信息，因为要获取telecode。去除空格是必须的。
            var startStation = allStations.FirstOrDefault(x => x.车站名 == te.start_station_name.Replace(" ", ""));
            var endStation = allStations.FirstOrDefault(x => x.车站名 == te.end_station_name.Replace(" ", ""));
            if (startStation == null || endStation == null) return;  // 金山卫找不到，也不需要记录

            string queryURL = "https://kyfw.12306.cn/otn/czxx/queryByTrainNo?"
                + "train_no=" + te.train_no
                + "&from_station_telecode=" + startStation.telecode  // 三亚——三  亚 ，重点多了两个空格，"海  口东"也是这情况
                + "&to_station_telecode=" + endStation.telecode
                + "&depart_date=" + departDate.ToString("yyyy-MM-dd");

            string trainContent = string.Empty;

            if (!Directory.Exists("TrainCode")) Directory.CreateDirectory("TrainCode");
            if (!Directory.Exists("TrainCode/" + departDate.ToString("yyyy-MM-dd"))) Directory.CreateDirectory("TrainCode/" + departDate.ToString("yyyy-MM-dd"));

            string trainFile = "TrainCode/" + departDate.ToString("yyyy-MM-dd") + "/" + te.train_code + ".json";
            
            /// If the file not exists, or the file content not have httpstatus 200, we have to download it.
            if (!File.Exists(trainFile) ||
                (
                    JsonConvert.DeserializeObject(File.ReadAllText(trainFile)) != null && 
                    !((JObject)JsonConvert.DeserializeObject(File.ReadAllText(trainFile)))["httpstatus"].ToString().Equals("200")
                )
               )
            {
                DownloadUriAsync(new Uri(queryURL), trainFile);
                Console.WriteLine(te.train_code);
            }
        }

        /// <summary>
        /// Give the train list from specific file.
        /// </summary>
        /// <param name="filePath">The file that contains train list.</param>
        /// <returns>List of train informations.</returns>
        public List<ClassTrainEntry> ParseTrainList(string filePath)
        {
            List<ClassTrainEntry> allTrains = new List<ClassTrainEntry>();

            string allContents = File.ReadAllText(filePath);
            allContents = allContents.Insert(0, "{").Insert(allContents.Length, "}");
            
            // Parse the contents as json object. Do not directly use the raw train_list.js file, it will consume over 600MB memory.
            var jobj = JObject.Parse(allContents);

            dynamic deserializedDateTrainInfo = JsonConvert.DeserializeObject(allContents);

            foreach (var eachDateTrainInfo in deserializedDateTrainInfo)
            {
                Console.WriteLine(eachDateTrainInfo.Name);  // 2016-03-02, 2016-02-29, etc...
                foreach (var eachTrainType in eachDateTrainInfo.Value)
                {
                    // 输出车次类别信息
                    //Console.WriteLine(eachTrainType.Name);  // K,T,G,D,C,Z,etc...
                    // 输出每趟车的信息
                    foreach (var eachTrain in eachTrainType.Value)
                    {
                        ClassTrainEntry curTrain = JsonConvert.DeserializeObject<ClassTrainEntry>(eachTrain.ToString());
                        //Console.WriteLine(curTrain);  // G1, G2, D1, D2, 1201, C2003, etc...
                        allTrains.Add(curTrain);
                    }
                }
            }

            return allTrains;
        }



    }
}
