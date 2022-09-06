using Archival.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface IFilterRecord<T>
    { 
        Task<FilterResponse<T>> Filter(List<T> allRecord);
    }
}
