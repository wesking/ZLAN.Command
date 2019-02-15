using System;
using System.Collections.Generic;

namespace ZL.CommandCore.Data
{
    public partial class SvrInvokeInfo
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string ServiceId { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? Interval { get; set; }
        public DateTime? NextInvokeTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Api { get; set; }
        public string Url { get; set; }
        public string RequestBody { get; set; }
        public string RequestHeaders { get; set; }
        public double? ResponseTime { get; set; }
        public string ResponseData { get; set; }
        public int? Status { get; set; }
    }
}
