using Archival.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface IRecord
    {
        public long dataID { get; set; }
        public long userID { get; set; }
        public long parentID { get; set; }
        public int serialNumber { get; set; }
        public string name { get; set; }
        public string conversionType { get; set; }
        public string recordLocation { get; set; }
        public string createdBy { get; set; } 
        public string localFilePath { get; set; } 
        public string conversionStatus { get; set; }
        public string error_MSG { get; set; }
        public ConvertedRecord convertedRecord { get; set; }
        public string folderRef { get; set; }
        public string FileSysID { get; set; }
        public string recordDate { get; set; }
        public string recordSecurityGrading { get; set; }
        public string authorName { get; set; }
        public string filedBy { get; set; }
        public string createDate { get; set; }
        public string keywordsRemarks { get; set; }
        public string nodeExt { get; set; } 
        public string convertedPath { get; set; }
        public string recordReportPath { get; set; } 
    }
}
