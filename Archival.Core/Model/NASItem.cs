using Archival.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class NASItem : IFilter
    {
        public NASItem()
        {
        }
        public long dataID { get; set; }
        public long userID { get; set; }
        public long parentID { get; set; }
        public DateTime QDATE { get; set; }
        public string RSI { get; set; }
        public string name { get; set; } 
        public bool isPDFExist { get; set; }
        public string parentSubtype { get; set; }
        public string recordLocation { get; set; }
        public string createdBy { get; set; }
        public bool isEmail { get; set; }
        public long noPDFId { get; set; }
        public ConvertedRecord convertedRecord { get; set; }
    }
}
