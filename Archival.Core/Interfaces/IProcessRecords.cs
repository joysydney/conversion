using Archival.Core.Model;
using Archival.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface IProcessRecords<T>
    {
        Task<ConversionResponse> Execute(List<T> records);
    }
}
