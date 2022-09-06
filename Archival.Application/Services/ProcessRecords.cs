using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archival.Application.Services;
using Archival.Core.Interfaces;
using Archival.Core.Model;
using Archival.Core.Model.Responses;

namespace Application.Services
{
    public class ProcessRecords<T>: IProcessRecords<T> where T : IRecord
    {

        private readonly ICSAccess<T> _csAccess;
        private readonly IEmail _email;
        private readonly ICSVReport<T> _cSV;
        private readonly IPDFService<T> _pdfService;
        private readonly IUtil<T> _util;
        private readonly IRepository<T> _repository;
        public ProcessRecords(ICSAccess<T> csAccess, IUtil<T> util, IRepository<T> repository, IPDFService<T> pdfService, IEmail email, ICSVReport<T> cSV)
        {
            _csAccess = csAccess;
            _util = util;
            _pdfService = pdfService;
            _email = email;
            _cSV = cSV;
            _repository = repository;
        }

        public async Task<ConversionResponse> Execute(List<T> records) 
        {
            _util.EnsureFilesAreCleanedUp(_util.GetBufferPath(records[0], cleanUp : true));
            Summary summary = new Summary();
            summary.TotalRecords = records.Count();
            foreach (T record in records)
            { 
                var recordInfo = await _csAccess.GetNode(record.dataID);
                CSNode cSNode = new CSNode();
                if (recordInfo.Success)
                {
                    cSNode = recordInfo.Response;
                }

                var parentRecordInfo = await _csAccess.GetNode(record.parentID);
                CSNode parentCSNode = new CSNode();
                if (parentRecordInfo.Success)
                {
                    parentCSNode = parentRecordInfo.Response;
                }

                var processedRecord = await ProcessRecord(record);
                summary.recordReportPath = _util.GetReportPath(record);
                if (processedRecord.Success)
                {
                    if (record.convertedRecord == null || (record.convertedRecord != null && record.convertedRecord.dataID != record.dataID))
                    {
                        record.conversionStatus = "Success";
                        summary.processedRecord(record.conversionType);
                        await logRecord(record, summary);
                    }
                }
                else
                {
                    record.conversionStatus = processedRecord.Message;
                    summary.processedFailedRecord(processedRecord.Message, record.name, record.recordLocation, record.conversionType);
                    await logRecord(record, summary);
                }
            }
            await logFolder(records, summary); 
            summary.folderReportPath = _util.GetFolderReportPath(summary);
            _email.sendEmailNotification(summary);
            if (summary.failedRecords.Count() > 0) _email.sendFailedEmailNotification(summary);
            await uploadCSVFile(summary);
            return new ConversionResponse("");
        }

        private async Task uploadCSVFile(Summary summary)
        {
            summary.csvFile.Add(summary.recordReportPath);
            summary.csvFile.Add(summary.folderReportPath);
            foreach (var path in summary.csvFile)
            {
                if (!string.IsNullOrEmpty(path))
                    await _pdfService.UploadCSVToBlazon(path, Path.GetFileName(path), summary.conversionType);
            }
        }

        private async Task logFolder(List<T> records, Summary summary)
        { 
            if (records[0].conversionType == "MAIN_NAS")
            {
                CSVFolderNAS cSVFolderNAS = new CSVFolderNAS();
                foreach (KeyValuePair<long, int> item in summary.totalRecordInFolder)
                {
                    var res = await _csAccess.GetCaseCategory(item.Key);
                    var doc = await _csAccess.GetDocCategory(item.Key); 
                    summary.parentFolderFromDate = res.Response.CaseCategory_OpenDate;
                    summary.parentFolderToDate = res.Response.CaseCategory_CloseDate;
                    summary.parentKeywordsRemarks = doc.Response.DocumentCategory_DocDesc;
                    summary.parentAuthorityNumber = doc.Response.DocumentCategory_AuthNumber;
                    summary.parentSerialNumber++;

                    _cSV.LogNASFolder(summary);
                }
            }
        }
        private async Task logRecord(T record, Summary summary)
        {
            if(record.conversionType == "MAIN_NAS")
            {
                if (summary.totalConvertedInFolder.ContainsKey(record.parentID))
                    summary.totalConvertedInFolder[record.parentID] = summary.totalConvertedInFolder[record.parentID] + 1;
                else
                    summary.totalConvertedInFolder[record.parentID] = 1;

                if (summary.totalRecordInFolder.ContainsKey(record.parentID)) 
                    summary.totalRecordInFolder[record.parentID] = summary.totalRecordInFolder[record.parentID] + 1; 
                else 
                    summary.totalRecordInFolder[record.parentID] = 1;

                var node = await _csAccess.GetNode(record.dataID);
                var user = await _csAccess.GetUser(record.userID);
                var caseCategory = await _csAccess.GetCaseCategory(record.dataID);
                var docCategory = await _csAccess.GetDocCategory(record.dataID); 
                record.folderRef = caseCategory.Response.CaseCategory_CaseID; 
                record.authorName = caseCategory.Response.CaseCategory_Author;
                record.recordDate = node.Response.data.modify_date;
                record.filedBy = user.Response.data.name;
                record.nodeExt = Path.GetExtension(record.name);

                record.keywordsRemarks = docCategory.Response.DocumentCategory_DocDesc; 
                record.recordSecurityGrading = docCategory.Response.DocumentCategory_SecurityClassif; 
                await _cSV.LogNASRecord(record);
            }
            else if(record.conversionType == "MAIN_A")
            {
                await _cSV.LogPDF(record);
            }
            else if(record.conversionType == "DAILYCONVERSION")
            {
                await _cSV.LogDaily(record);
            }
        }

        private async Task<ConversionResponse> ProcessRecord(T record) 
        { 
            _util.GetBufferPath(record); 
            var downloadedRecord = await _csAccess.DownloadRecord(record);
            if (downloadedRecord.Success)
            {
                if (_util.IsPasswordProtected(downloadedRecord.Response.sourcePath))
                {
                    return new ConversionResponse("File is password protected.");
                }
                if (_util.IsInvalidFormat(downloadedRecord.Response.sourcePath))
                {
                    return new ConversionResponse("File has invalid format.");
                }
                else if (record.convertedRecord == null || (record.convertedRecord != null && record.convertedRecord.dataID != record.dataID))
                {
                    var converted = await _pdfService.ConvertToPDF(downloadedRecord.Response);
                    if (converted.Success)
                    {
                        if (record.convertedRecord != null)
                        {
                            await _csAccess.AddVersion(record);
                            await _pdfService.UploadPDFToBlazon(record);
                        }
                        else
                        {
                            await _csAccess.UploadFile(record);
                            await _pdfService.UploadPDFToBlazon(record);
                        }
                        return new ConversionResponse(converted.Response);
                    }
                    else
                    {
                        return new ConversionResponse(converted.Message);
                    }
                }
                else
                    return new ConversionResponse(true);
            }

            return new ConversionResponse(downloadedRecord.Message);
           
        } 
    }
}
