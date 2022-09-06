using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface ICryptography
    {
        string ReadSensitiveData(string inputFilePath, string keyFilePath);
    }
}
