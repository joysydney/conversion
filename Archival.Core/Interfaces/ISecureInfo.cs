using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface ISecureInfo
    {
        string getSensitiveInfo(string secureFileName);
    }
}
