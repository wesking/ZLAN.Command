using System;
using System.Collections.Generic;

namespace ZLAN.Command.Data
{
    public partial class SvrAppCache
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public int? CacheTime { get; set; }
        public string CacheVersion { get; set; }
    }
}
