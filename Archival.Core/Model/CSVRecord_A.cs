using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class CSVRecord_A
    {
        public long DataId { get; set; }
        public string DocName { get; set; }
        public long ParentId { get; set; } 
        public string ConversionStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}
