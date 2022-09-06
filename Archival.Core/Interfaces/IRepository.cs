using Archival.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface IRepository<T>
    {
        Task<List<T>> GetRecords();
        Task<List<DailyItem>> GetDailyRecords();
        Task<List<PDFItem>> GetPDFRecords();
        Task<List<DailyItem>> GetDailyRecordsUnderCompounds();
        Task<List<FullPath>> GetRecordFullPath(long id);
        Task<int> GetRecord(T nas);
        Task<List<ConvertedRecord>> GetConverted_FirstCol(long id);
        Task<int> GetConverted_SecCol(T nas);
        Task<int> GetEkrisRelated(T nas);
        Task<bool> isEmailWithAttachment(long id);
        Task<long> GetCompoundLocation(long id); 
        Task<int> AddEkrisRelated(long id, long convertedID);
        Task<List<(String, String)>> GetRecordLocation(T nas);
    }
}
