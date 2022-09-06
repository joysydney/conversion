using Archival.Core.Model;
using Archival.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Application.Services
{
    public interface IPDFService<T>
    {
        Task<ConvertedResponse> ConvertToPDF(DownloadedRecord record); 
        Task<UploadedResponse> UploadPDFToBlazon(T record);
        Task<UploadedResponse> UploadCSVToBlazon(string sourcePath, string fileName, string conversionType);
    }
}
