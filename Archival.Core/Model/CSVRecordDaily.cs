using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class CSVRecordDaily
    {
        public long DataId { get; set; }
        public string DocName { get; set; }
        public string RecLocationCS { get; set; }
        public string Uploader { get; set; }
        public string ErrorMessage { get; set; }
        public string ConversionStatus { get; set; }
    }
}
