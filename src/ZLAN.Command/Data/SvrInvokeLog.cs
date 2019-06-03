using System;
using System.Collections.Generic;

namespace ZLAN.Command.Data
{
    public partial class SvrInvokeLog
    {
        public int Id { get; set; }
        public int? InvokeId { get; set; }
        public string AppId { get; set; }
        public string ServiceId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Api { get; set; }
        public string Url { get; set; }
        public string RequestBody { get; set; }
        public string RequestHeaders { get; set; }
        public double? ResponseTime { get; set; }
        public string ResponseData { get; set; }
    }
}
