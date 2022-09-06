using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Configuration
{
    public class DBConfig
    {
        public const string ConnectionStrings = "ConnectionStrings";
        public string DBConnection { get; set; }
    }
}
