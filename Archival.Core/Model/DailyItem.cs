using Archival.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class DailyItem : IFilter
    {
        public long dataID { get; set; }
        public long userID { get; set; }
        public long parentID { get; set; } 
        public string modifiedBy { get; set; }
        public string name { get; set; }
        public bool subtype { get; set; }
        public string recordLocation { get; set; }
        public string createdBy { get; set; }
        public bool isEmail { get; set; }
        public long noPDFId { get; set; }
        public ConvertedRecord convertedRecord { get; set; }
    }
}
