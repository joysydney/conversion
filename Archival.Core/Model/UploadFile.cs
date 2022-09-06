using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class UploadFile
    {
        public string sourcePath { get; set; } 
        public string fileName { get; set; }
        public string conversionType { get; set; }
    }
}
