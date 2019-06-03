using System;
using System.Collections.Generic;
using System.Text;

namespace ZLAN.Command
{
    public class CommandOptions
    {
        public string ServiceKey { get; set; }
        
        public string ConnectionString { get; set; }

        public bool InvokerHostEnable { get; set; } = false;
    }
}
