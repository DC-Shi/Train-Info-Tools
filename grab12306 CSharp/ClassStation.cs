using System;
using System.Collections.Generic;
using System.IO;

namespace Parse12306_CSharp
{
    public class ClassStation
    {
        /// <summary>
        /// 车站的字母码（三位）
        /// <list type="table">
        /// <listheader><para>
        /// <term>车站名称汉字数量</term>
        /// <description>字母码组成方式</description>
        /// </para></listheader>
        /// <item><para>
        /// <term>一个字</term>
        /// <description>拼音前三个字母. </description>
        /// </para></item>
        /// <item><para>
        /// <term>两个字</term>
        /// <description>第一个字的拼音首字母+第二个字的拼音前两个字母.</description>
        /// </para></item>
        /// <item><para>
        /// <term>三个字</term>
        /// <description>拼音首字母.</description>
        /// </para></item>
        /// <item><para>
        /// <term>多于三个</term>
        /// <description>前两个字的拼音首字母+最后一个字的拼音首字母.</description>
        /// </para></item>
        /// </list>
        /// </summary>
        public string 字母码 { get; set; }
        /// <summary>
        /// 车站汉字名称
        /// </summary>
        public string 车站名 { get; set; }
        /// <summary>
        /// 电报码（三位）
        /// </summary>
        public string telecode { get; set; }
        /// <summary>
        /// 车站拼音，全拼
        /// </summary>
        public string 拼音 { get; set; }
        /// <summary>
        /// 车站拼音首字母
        /// </summary>
        public string 拼音首字母 { get; set; }
        /// <summary>
        /// 车站id
        /// <remarks>车站的id并不是连续的，三亚站和海口站均跳了一个，说明为了保证三亚——三亚和海口东——海口东的数据，每个站分配的两个id，但是只能下载到较小的那个id</remarks>
        /// </summary>
        public string id { get; set; }

        public ClassStation()
        {
            字母码 = string.Empty;
            车站名 = string.Empty;
            telecode = string.Empty;
            拼音 = string.Empty;
            拼音首字母 = string.Empty;
            id = string.Empty;
        }

        /// <summary>
        /// 类Constructor
        /// </summary>
        /// <param name="stationStr">原始字符串，
        /// 如：<example>bjb|北京北|VAP|beijingbei|bjb|0</example>
        /// </param>
        /// <exception cref="System.ArgumentException">输入的字符串不符合上述标准</exception>
        public ClassStation(string stationStr)
        {
            string[] parts = stationStr.Split('|');
            if (parts.Length == 6)
            {
                字母码 = parts[0];
                车站名 = parts[1];
                telecode = parts[2];
                拼音 = parts[3];
                拼音首字母 = parts[4];
                id = parts[5];
            }
            else
                throw new ArgumentException("Input stationStr is not in correct format.");
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", 字母码, 车站名, telecode, 拼音, 拼音首字母, id);
        }

        /// <summary>
        /// Load station file content.
        /// </summary>
        /// <param name="inputFile">Path to the file.</param>
        /// <returns>The file content.</returns>
        string _loadStationFile(string inputFile)
        {
            string ret = File.ReadAllText(inputFile);

            return ret;
        }
        /// <summary>
        /// Parse the content to List&lt;ClassStation&gt;
        /// </summary>
        /// <param name="stationString">The content that contains stations info.</param>
        /// <returns>Parsed list.</returns>
        List<ClassStation> _parseStations(string stationString)
        {
            List<ClassStation> ret = new List<ClassStation>();

            string[] splitStations = stationString.Split(new char[] { '@', '\'', ';' });

            foreach (string sta in splitStations)
            {
                try
                {
                    var parsedStation = new ClassStation(sta);
                    ret.Add(parsedStation);
                }
                catch (ArgumentException) { }
            }

            return ret;
        }

        /// <summary>
        /// URL to station_name.js
        /// </summary>
        public static string Url_station_name_js
        {
            get
            {
                return "https://kyfw.12306.cn/otn/resources/js/framework/station_name.js";
            }
        }

        /// <summary>
        /// Default saving path of station_name.js
        /// </summary>
        public static string Filename_station_name_js
        {
            get; set;
        } = "station_name.js";

        /// <summary>
        /// Parse the file to List&lt;ClassStation&gt;
        /// </summary>
        /// <param name="filePath">Path to the file that contains station info.</param>
        /// <returns>Parsed list.</returns>
        public List<ClassStation> ParseStationsByFile(string filePath)
        {

            string stationContent = _loadStationFile(filePath);

            var parsedSta = _parseStations(stationContent);

            return parsedSta;
        }
    }
}
