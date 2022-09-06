using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class ConvertedPermission
    {
        public string type { get; set; }
        public long id { get; set; }
        public long convertedID { get; set; }
    }
}
