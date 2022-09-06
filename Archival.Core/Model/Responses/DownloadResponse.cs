using Archival.Core.Model;
using Archival.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Application.Services.Responses
{
    public class DownloadResponse : BaseResponse<DownloadedRecord>
    {
        public DownloadResponse(DownloadedRecord record) : base(record) { }
        public DownloadResponse(string message) : base(message) { }
    }
}
