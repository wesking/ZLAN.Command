using System;
using System.Collections.Generic;

namespace ZLAN.Command.Data
{
    public partial class SvrAppAuthorization
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string AuthorizationCode { get; set; }
        public int? Status { get; set; }
    }
}
