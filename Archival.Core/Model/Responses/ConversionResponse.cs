using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model.Responses
{
    public class ConversionResponse : BaseResponse<bool>
    {
        public ConversionResponse(bool status) : base(status) { }
        public ConversionResponse(string message) : base(message) { }
    }
}
