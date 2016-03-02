namespace Parse12306_CSharp
{
    public class ClassTrainSchedule
    {
        /// <summary>
        /// 站序
        /// </summary>
        public string station_no
        {
            get; set;
        }
        /// <summary>
        /// ？车站车次代码
        /// </summary>
        public string station_train_code
        {
            get; set;
        }
        /// <summary>
        /// ？车次等级
        /// </summary>
        public string train_class_name
        {
            get; set;
        }
        /// <summary>
        /// 站名
        /// </summary>
        public string station_name
        {
            get; set;
        }
        /// <summary>
        /// 到站时间
        /// </summary>
        public string arrive_time
        {
            get; set;
        }
        /// <summary>
        /// 出发时间
        /// </summary>
        public string start_time
        {
            get; set;
        }
        /// <summary>
        /// 停留时间
        /// </summary>
        public string stopover_time
        {
            get; set;
        }
    }
}
