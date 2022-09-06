using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archival.Core.Configuration;
using Archival.Core.Interfaces;
using Archival.Core.Model;
using Archival.Core.Model.Responses;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Archival.Application.Services
{
    public class PDFService<T> : IPDFService<T> where T : IRecord
    {
        private readonly ILogManagerCustom _log;
        private readonly ConversionConfiguration _config;
        public PDFService(IOptions<ConversionConfiguration> config, ILogManagerCustom log)
        { 
            _log = log;
            _config = config.Value;
        }
        public async Task<ConvertedResponse> ConvertToPDF(DownloadedRecord record)
        {
            var client = new RestClient(_config.BlazonService);
            var request = new RestRequest("PDFConverter", Method.Post);
            request.AddFile("file", record.sourcePath);
            request.AddParameter("filename", record.fileName);
            request.AddParameter("action", record.conversionType);
            var response = await client.ExecuteAsync(request);
            if (response == null)
            {
                string msg = "The PDF conversion response from server is null, for file: " + record.sourcePath;
                _log.debug(msg);
                return new ConvertedResponse(msg);
            }
            else
            {
                if (response.RawBytes == null)
                {
                    string msg = "The PDF conversion response bytes from server is null, for file: " + record.sourcePath;
                    _log.debug(msg);
                    return new ConvertedResponse(msg);
                }
                else
                {
                    if (response.RawBytes.Length == 0)
                    {
                        string msg = "The PDF conversion response bytes length from server is 0, for file: " + record.sourcePath;
                        _log.debug(msg);
                        return new ConvertedResponse(msg);
                    }
                    else
                    {
                        await System.IO.File.WriteAllBytesAsync(record.destPath, response.RawBytes); // create the converted pdf file to the destination folder
                        return new ConvertedResponse(true);
                    }
                }
            }
        }
        public async Task<UploadedResponse> UploadPDFToBlazon(T record)
        {
            var client = new RestClient(_config.BlazonService);
            var request = new RestRequest("NASFiles", Method.Post);
            request.AddFile("file", record.convertedPath);
            request.AddParameter("filename", record.name);
            request.AddParameter("action", record.conversionType);
            var response = await client.ExecuteAsync(request);
            if (response == null)
            {
                string msg = "The PDF conversion response from server is null, for file: " + record.convertedPath;
                _log.debug(msg);
                return new UploadedResponse(msg);
            }
            else
            {
                return new UploadedResponse(true);
            }
        }
        public async Task<UploadedResponse> UploadCSVToBlazon(string sourcePath, string fileName, string conversionType)
        {
            var client = new RestClient(_config.BlazonService);
            var request = new RestRequest("CSVFiles", Method.Post);
            request.AddFile("file", sourcePath);
            request.AddParameter("filename", fileName);
            request.AddParameter("action", conversionType);
            var response = await client.ExecuteAsync(request);
            if (response == null)
            {
                string msg = "The PDF conversion response from server is null, for file: " + sourcePath;
                _log.debug(msg);
                return new UploadedResponse(msg);
            }
            else
            {
                return new UploadedResponse(true);
            }
        }
    }
}
