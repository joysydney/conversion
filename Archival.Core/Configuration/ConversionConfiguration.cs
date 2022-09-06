using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Configuration
{
    public class ConversionConfiguration
    {
        public const string Conversion = "Conversion";
        public string BlazonService { get; set; }
        public string BufferFolder { get; set; }
    }
}
