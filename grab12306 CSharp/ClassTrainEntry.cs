namespace Parse12306_CSharp
{
    public class ClassTrainEntry
    {
        /// <summary>
        /// 车次代码，自动解析其中的车次、始发站、到达站
        /// <para>例：
        /// <example>{"station_train_code":"D1(北京-沈阳)","train_no":"24000000D10R"}</example>
        /// </para>
        /// </summary>
        public string station_train_code
        {
            set
            {                
                _station_train_code = value;
                string[] parts = value.Split(new char[] { '(', ')', '-' });
                _车次 = parts[0];
                _始发站名称 = parts[1];
                _终到站名称 = parts[2];
            }
            get { return _station_train_code; }
        }
        private string _station_train_code;

        /// <summary>
        /// 始发站名称<para />
        /// 只能通过station_train_code修改
        /// </summary>
        public string start_station_name
        {
            get { return _始发站名称; }
        }
        private string _始发站名称 = string.Empty;

        /// <summary>
        /// 终到站名称<para />
        /// 只能通过station_train_code修改
        /// </summary>
        public string end_station_name
        {
            get { return _终到站名称; }
        }
        private string _终到站名称 = string.Empty;

        /// <summary>
        /// 车次<para />
        /// 只能通过station_train_code修改
        /// </summary>
        public string train_code
        {
            get { return _车次; }
        }
        private string _车次 = string.Empty;

        /// <summary>
        /// Train_No，车次特殊编号<para />
        /// 获取车次停站信息时需要
        /// </summary>
        public string train_no { get; set; }

        public string TrainCategory { get; set; }

        public override string ToString()
        {
            return string.Format("{1}({0}-{2}), {3}", _始发站名称, _车次, _终到站名称, train_no);
        }
    }
}
