using System;
using System.Collections.Generic;

namespace ZL.CommandCore.Data
{
    public partial class SvrAppInfo
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Memo { get; set; }
        public int? Status { get; set; }
    }
}
