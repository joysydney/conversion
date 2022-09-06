using Archival.Core.Configuration;
using Archival.Core.Interfaces;
using Archival.Core.Model;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using System; 
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.File
{
    public class CSVReport<T> : ICSVReport<T> where T : IRecord
    {
        private int serialNumber = 0;
        private readonly CSVConfiguration _config;
        private readonly ILogManagerCustom _log;
        private readonly IUtil<T> _util;
        public CSVReport(IOptions<CSVConfiguration> config, ILogManagerCustom log, IUtil<T> util)
        {
            _config = config.Value;
            _log = log;
            _util = util;
        }
        public async Task LogNASRecord(T data)
        {
            try
            {
                var csvRows = new List<CSVRecord>
                {
                    new CSVRecord
                    {
                        SerialNumber = ++serialNumber,
                        BatchNumber = "URA/"+ DateTime.Now.ToString("ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture),
                        FolderRef = data.folderRef,
                        RecordID = data.dataID,
                        RecordTitle = data.name,
                        RecordDate = data.recordDate,
                        RecordSecurityGrading = data.recordSecurityGrading,
                        AuthorName = data.authorName,
                        AuthorDesignation = "",
                        FiledBy = data.filedBy,
                        FiledDesignation = "",
                        FiledDateTime = data.createDate,
                        KeywordsRemarks = data.keywordsRemarks,
                        FileType = "application/pdf",
                        FileSysID = data.parentID,
                        FileName = data.name,
                        ApplicationID = "",
                        ApplicationExt = data.nodeExt?? String.Empty,
                        FileOrder = "1",
                        ChecksumMD5 = "",
                        TimestampChecksumMD5 = "",
                        ChecksumSHA256 = "",
                        TimestampChecksumSHA256 = "",
                        FilePath = "\\Export\\"+DateTime.Now.ToString("ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture)

                    }
                };
                CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                { 
                    HasHeaderRecord = !System.IO.File.Exists(_util.GetReportPath(data))
                };
                using (var writer = new StreamWriter(_util.GetReportPath(data), true))
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    await csv.WriteRecordsAsync(csvRows);
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
        }
        public async Task LogNASFolder(Summary summary)
        {
            try
            {
                var csvRows = new List<CSVFolderNAS>
                {
                    new CSVFolderNAS
                    {
                        SerialNumber = summary.parentSerialNumber,
                        AuthorityNumber = summary.parentAuthorityNumber,
                        ReviewDate = summary.parentReviewDate,
                        AppraisedSystemID = "",
                        FolderRef = summary.parentFolderRef,
                        FolderTitle = summary.parentFolderTitle,
                        FolderSecurityGrading = summary.parentSecurityGrading,
                        FolderTransferFormat = "Digital",
                        FolderFromDate = summary.parentFolderFromDate,
                        FolderToDate = summary.parentFolderToDate,
                        KeywordsRemarks = summary.parentKeywordsRemarks,
                        RecordNumber = summary.totalRecordInFolderCount,
                        NativeFileNumber = summary.totalRecordInFolderCount,
                        ConvertedFileNumber = summary.totalConvertedInFolderCount,
                        MasterDigitisedFileNumber = "NA",
                        AccessFileNumber = "NA",
                        RedactedFileNumber = "NA",
                    }
                };
                CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = !System.IO.File.Exists(_util.GetFolderReportPath(summary))
                };
                using (var writer = new StreamWriter(_util.GetFolderReportPath(summary), true))
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    await csv.WriteRecordsAsync(csvRows);
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
        }
        public async Task LogPDF(T record)
        {
            try
            {
                var csvRows = new List<CSVRecord_A>
                {
                    new CSVRecord_A
                    {
                        DataId = record.dataID,
                        DocName = record.name,
                        ParentId = record.parentID,
                        ConversionStatus = record.conversionStatus,
                        ErrorMessage = record.error_MSG
                    }
                };
                CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = !System.IO.File.Exists(_util.GetReportPath(record))
                };
                using (var writer = new StreamWriter(_util.GetReportPath(record), true))
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    await csv.WriteRecordsAsync(csvRows);
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
        }
        public async Task LogDaily(T record)
        {
            try
            {
                var csvRows = new List<CSVRecordDaily>
                {
                    new CSVRecordDaily
                    {
                        DataId = record.dataID,
                        DocName = record.name,
                        RecLocationCS = record.recordLocation,
                        ConversionStatus = record.conversionStatus,
                        ErrorMessage = record.error_MSG,
                        Uploader = record.createdBy
                    }
                };
                string path = _util.GetReportPath(record);
                CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = !System.IO.File.Exists(path)
                };
                using (var writer = new StreamWriter(path, true))
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    await csv.WriteRecordsAsync(csvRows);
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
        }
    }
}
