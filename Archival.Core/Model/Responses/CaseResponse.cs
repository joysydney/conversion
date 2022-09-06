using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model.Responses
{ 
    public class CaseResponse : BaseResponse<CaseCategory>
    {
        public CaseResponse(CaseCategory caseCategory) : base(caseCategory) { }
        public CaseResponse(string message) : base(message) { }
    }
}
