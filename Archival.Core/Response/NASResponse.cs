using Archival.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Response
{
    public class NASResponse : BaseResponse<NAS>
    {
        public NASResponse(NAS nas) : base(nas) { }
        public NASResponse(string message) : base(message) { }
    }
}
