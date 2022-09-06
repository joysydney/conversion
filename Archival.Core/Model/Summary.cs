using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class Summary
    {
        public Summary()
        {
            totalRecordInFolder = new Dictionary<long, int>();
            totalConvertedInFolder = new Dictionary<long, int>();
            failedRecords = new List<FailedRecord>();
            csvFile = new List<String>();
        }
        public int TotalRecords { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalFailed { get; set; }
        public string conversionType { get; set; }
        public string recordReportPath { get; set; }
        public string folderReportPath { get; set; }
        public string parentAuthorityNumber { get; set; }
        public string parentFolderRef { get; set; }
        public string parentReviewDate { get; set; }
        public string parentFolderTitle { get; set; }
        public string parentSecurityGrading { get; set; }
        public string parentKeywordsRemarks { get; set; }
        public string parentFolderFromDate { get; set; }
        public string parentFolderToDate { get; set; }
        public string convertedPath { get; set; }
        public int totalRecordInFolderCount { get; set; }
        public int totalConvertedInFolderCount { get; set; }
        public List<FailedRecord> failedRecords {get;set;}
        public Dictionary<long, int> totalRecordInFolder { get; set; }
        public Dictionary<long, int> totalConvertedInFolder { get; set; }
        public int parentSerialNumber { get; set; }
        public List<String> csvFile { get; set; } 
        public void processedRecord(string conversionType)
        {  
            this.TotalProcessed++; 
            this.TotalSuccess++;
            this.conversionType = conversionType;
        }
        public void processedFailedRecord(string errorMsg, string name, string path, string conversionType)
        {
            FailedRecord failedRecord = new FailedRecord();
            failedRecord.recLocation = path;
            failedRecord.name = name;
            failedRecord.error_msg = errorMsg;
            failedRecords.Add(failedRecord);

            this.TotalProcessed++; 
            this.TotalFailed++;
            this.conversionType = conversionType;
        }
    }

    public class FailedRecord
    {
        public string name { get; set; }
        public string recLocation { get; set; }
        public string error_msg { get; set; }
    }
}
