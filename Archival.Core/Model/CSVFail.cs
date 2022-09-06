using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class CSVFail
    {
        public CSVFail() { }
        public CSVFail(string path, string errorCode, string recAction)
        {
            this.path = path;
            this.errorCode = errorCode;
            this.recAction = recAction;
        }
         
        [CsvHelper.Configuration.Attributes.Name("DateTime")]
        public string date { get; set; } = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        [CsvHelper.Configuration.Attributes.Name("Path")]
        public string path { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Error Code")]
        public string errorCode { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Recommended Action")]
        public string recAction { get; set; }
    }
}
