using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model.Responses
{
    public class UploadedResponse : BaseResponse<bool>
    {
        public UploadedResponse(bool status) : base(status) { }
        public UploadedResponse(string message) : base(message) { }
    } 
}
