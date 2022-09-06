using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Configuration
{
    public class LogConfiguration
    {
        public const string Logger = "Logger";
        public string LogDirectory { get; set; }
        public string LogFileName_Main { get; set; }
        public string LogFileLevel { get; set; }
        public string LogConsoleLevel { get; set; }
    }
}
