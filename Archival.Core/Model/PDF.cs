using Archival.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class PDF : IRecord, IFilter
    {
        public long dataID { get; set; }
        public long userID { get; set; }
        public long parentID { get; set; }
        public int serialNumber { get; set; }
        public string name { get; set; }
        public string CSVFolderPath { get; set; }
        public string CSVRecordPath { get; set; }
        public bool isPDFExist { get; set; }
        public string parentSubtype { get; set; }
        public ConvertedRecord convertedRecord { get; set; }
        public string localFilePath { get; set; }
        public string conversionType { get; set; } = "MAIN_A";
        public string recordLocation { get; set; }
        public string createdBy { get; set; }
        public bool isEmail { get; set; }
        public string conversionStatus { get; set; }
        public string folderRef { get; set; }
        public string FileSysID { get; set; }
        public string recordDate { get; set; }
        public string recordSecurityGrading { get; set; }
        public string authorName { get; set; }
        public string filedBy { get; set; }
        public string createDate { get; set; }
        public string keywordsRemarks { get; set; }
        public string nodeExt { get; set; } 
        public string recordFullPath { get; set; }
        public string error_MSG { get; set; }
        public string convertedPath { get; set; }
        public long noPDFId { get; set; }
        public string recordReportPath { get; set; }
        public void recordProcessed()
        {
            this.serialNumber++;
            this.conversionStatus = "Success";
            this.error_MSG = String.Empty;
        }
        public void recordFailedProcessed(string error_msg)
        {
            this.serialNumber++;
            this.conversionStatus = "Fail";
            this.error_MSG = error_msg;
        }
    }
}
