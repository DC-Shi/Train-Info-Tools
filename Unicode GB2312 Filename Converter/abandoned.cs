using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Unicode_GB2312_Filename_Converter
{
    class abandoned
    {
        /// <summary>
        /// 将UTF8编码的汉字转换为URL ENCODING
        /// </summary>
        /// <param name="filepath">包含车站名称的文件的路径</param>
        static void OutputToURLString(string filepath)
        {
            /// “站”字UTF8编码"%E7%AB%99"
            /// 

            foreach (var line in File.ReadAllLines(filepath))
            {
                Console.WriteLine("%{0}%E7%AB%99",
                    BitConverter.ToString(Encoding.UTF8.GetBytes(line)).Replace('-', '%'));
            }
        }

        /// <summary>
        /// 将文件名从GB2312/18030编码转换为UTF8编码。
        /// 此方法不应该使用，因为Linux下面获取的桂林北会在转换后变为桂林�_
        /// 正确的桂林北：E6-A1-82-E6-9E-97-E5-8C-97
        /// 错误的桂林�_：E6-A1-82-E6-9E-97-E5-8C-5F
        /// </summary>
        static void NameFromGb2312ToUtf8()
        {

            Console.OutputEncoding = Encoding.UTF8;
            var filenames = Directory.EnumerateFiles("stations");
            foreach (var path in filenames)
            {
                try
                {
                    var filename = Path.GetFileName(path);
                    var utf8name = Encoding.UTF8.GetString(Encoding.GetEncoding("GB18030").GetBytes(filename));

                    Console.WriteLine("{0}", BitConverter.ToString(Encoding.GetEncoding("GB18030").GetBytes(filename)));
                    foreach (var c in Encoding.GetEncoding("GB18030").GetBytes(filename))
                    {
                        Console.Write("{0:X}", c);
                        Console.Write('\t');
                    }
                    //Console.WriteLine("{0}", BitConverter.ToString(Encoding.GetEncoding("GB18030").GetBytes("桂林北")));
                    Console.WriteLine("{0} is {1}", filename, utf8name);
                }
                catch (DecoderFallbackException dfe)
                {
                    Console.WriteLine("Fall back occured for {0}", path);
                }
                Console.WriteLine();
            }
        }


        /// <summary>
        /// 编码测试。
        /// 结论：
        /// GB2312.GetBytes("闇炴郸") == UTF8.GetBytes("霞浦")
        /// </summary>
        static void StringEncodingTest()
        {
            string[] location = new string[] { "闇炴郸", "霞浦" };
            foreach (string str in location)
            {
                Console.WriteLine(str);
                List<Encoding> testEncodings = new List<Encoding>()
                {
                    Encoding.ASCII,
                    Encoding.BigEndianUnicode,
                    Encoding.Default,
                    Encoding.Unicode,
                    Encoding.UTF32,
                    Encoding.UTF7,
                    Encoding.UTF8,
                    Encoding.GetEncoding("GB2312"),
                    Encoding.GetEncoding("GBK"),
                    Encoding.GetEncoding("GB18030"),
                    Encoding.GetEncoding("BIG5"),
                };
                foreach (var encoding in testEncodings)
                {
                    Console.WriteLine("Encoding:\t{0}\tHexString:\t{1}", encoding.EncodingName, BitConverter.ToString(encoding.GetBytes(str)));
                }
            }
        }
    }
}
