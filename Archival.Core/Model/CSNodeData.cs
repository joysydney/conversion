using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class CSNodeData
    {
        public long id { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
        public string conversionType { get; set; }
        public string folderRef { get; set; }
        public string modify_date { get; set; }
        public string modify_user_id { get; set; }
        public string createDate { get; set; }
        public string recordSecurityGrading { get; set; }
        public string authorName { get; set; }
        public string filedBy { get; set; }
        public string nodeExt { get; set; }
        public string keywordsRemarks { get; set; }
        public long parentID { get; set; }
        public ConvertedRecord convertedRecord { get; set; }
    }
}
