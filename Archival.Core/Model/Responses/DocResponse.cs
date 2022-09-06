using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model.Responses
{ 
    public class DocResponse : BaseResponse<DocCategory>
    {
        public DocResponse(DocCategory docCategory) : base(docCategory) { }
        public DocResponse(string message) : base(message) { }
    }
}
