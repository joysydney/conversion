using Archival.Core.Configuration;
using Archival.Core.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Application.Services
{
    public class SecureInfo : ISecureInfo
    {
        private readonly BaseConfiguration _config; 
        private readonly ICryptography _cryptography; 
        public SecureInfo(IOptions<BaseConfiguration> config, ICryptography cryptography)
        {
            _config = config.Value;
            _cryptography = cryptography; 
        }

        public object CryptographyCore { get; private set; }

        public string getSensitiveInfo(string secureFileName)
        {
            var fileName = _config.secureInfo.fileName;

            string credentialsDirectory = _config.secureInfo.Path;
            string AESKeyFilePath = Path.Combine(credentialsDirectory, _config.secureInfo.fileName.AESKey);
            string secureFilePath = Path.Combine(credentialsDirectory, secureFileName);

            return _cryptography.ReadSensitiveData(secureFilePath, AESKeyFilePath);
        }
    }
}
