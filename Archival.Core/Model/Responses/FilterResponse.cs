using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model.Responses
{
    public class FilterResponse<T> : BaseResponse<List<T>>
    {
        public FilterResponse(List<T> record) : base(record) { }
        public FilterResponse(string message) : base(message) { }
    }
}
