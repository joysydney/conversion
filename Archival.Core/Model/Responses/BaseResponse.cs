using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model.Responses
{
    public abstract class BaseResponse<T>
    {
        public string Message { get; private set; }
        public bool Success { get; private set; }
        public T Response { get; private set; }

        protected BaseResponse(T response)
        {
            Success = true;
            Response = response;
            Message = string.Empty;
        }

        protected BaseResponse(string message)
        {
            Success = false;
            Message = message;
        }
    }
}
