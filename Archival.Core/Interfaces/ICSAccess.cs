using Archival.Application.Services.Responses;
using Archival.Core.Model;
using Archival.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface ICSAccess<T>
    {
        Task<NodeResponse> GetNode(long id);
        Task<NodeResponse> GetUser(long id);
        Task<CaseResponse> GetCaseCategory(long id);
        Task<DocResponse> GetDocCategory(long id);
        Task UpdateDocCategory(long id);
        Task<DownloadResponse> DownloadRecord(T record); 
        Task<string> UploadFile(T record); 
        Task AddVersion(T record);
        Task HideRecord(bool hide, long convertedID);

    }
}
