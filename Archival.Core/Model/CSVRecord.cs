using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class CSVRecord
    {
        public int SerialNumber { get; set; }
        public string BatchNumber { get; set; }
        public string FolderRef { get; set; }
        public long RecordID { get; set; }
        public string RecordTitle { get; set; }
        public string RecordDate { get; set; }
        public string RecordSecurityGrading { get; set; }
        public string AuthorName { get; set; }
        public string AuthorDesignation { get; set; }
        public string FiledBy { get; set; }
        public string FiledDesignation { get; set; }
        public string FiledDateTime { get; set; }
        public string KeywordsRemarks { get; set; }
        public string FileType { get; set; }
        public long FileSysID { get; set; }
        public string FileName { get; set; }
        public string ApplicationID { get; set; }
        public string ApplicationExt { get; set; }
        public string FileOrder { get; set; }
        public string ChecksumMD5 { get; set; }
        public string TimestampChecksumMD5 { get; set; }
        public string ChecksumSHA256 { get; set; }
        public string TimestampChecksumSHA256 { get; set; }
        public string FilePath { get; set; }
    }
}
