using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model.Responses
{
    public class NodeResponse : BaseResponse<CSNode>
    {
        public NodeResponse(CSNode cSNode) : base(cSNode) { }
        public NodeResponse(string message) : base(message) { }
    }
}
