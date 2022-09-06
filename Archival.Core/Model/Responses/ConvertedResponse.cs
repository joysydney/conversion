using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model.Responses
{ 
    public class ConvertedResponse : BaseResponse<bool>
    {
        public ConvertedResponse(bool status) : base(status) { }
        public ConvertedResponse(string message) : base(message) { }
    }
}
