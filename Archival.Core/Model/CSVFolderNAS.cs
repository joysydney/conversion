using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class CSVFolderNAS
    {
        public int SerialNumber { get; set; }
        public string AuthorityNumber { get; set; }
        public string ReviewDate { get; set; }
        public string AppraisedSystemID { get; set; }
        public string FolderRef { get; set; }
        public string FolderTitle { get; set; }
        public string FolderSecurityGrading { get; set; }
        public string FolderTransferFormat { get; set; }

        private string _FolderFromDate;
        public string FolderFromDate
        {
            get
            {
                return _FolderFromDate == null ? "" : _FolderFromDate;
            }
            set
            {
                _FolderFromDate = value;
            }
        }
        private string _FolderToDate;
        public string FolderToDate
        {
            get
            {
                return _FolderToDate == null ? "" : _FolderToDate;
            }
            set
            {
                _FolderToDate = value;
            }
        }
        public string KeywordsRemarks { get; set; }
        public int RecordNumber { get; set; }
        public int NativeFileNumber { get; set; }
        public int ConvertedFileNumber { get; set; }
        public string MasterDigitisedFileNumber { get; set; }
        public string AccessFileNumber { get; set; }
        public string RedactedFileNumber { get; set; }
    }
}
