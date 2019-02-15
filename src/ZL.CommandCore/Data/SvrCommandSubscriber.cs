using System;
using System.Collections.Generic;

namespace ZL.CommandCore.Data
{
    public partial class SvrCommandSubscriber
    {
        public int Id { get; set; }
        public string CommandKey { get; set; }
        public string ExecutorType { get; set; }
        public string ExecutorConfig { get; set; }
        public int? Sort { get; set; }
        public int? Status { get; set; }
    }
}
