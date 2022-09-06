using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class DocCategory
    {
        public string DocumentCategory_PrevDocNo { get; set; }
        public string DocumentCategory_SecurityClassif { get; set; }
        public string DocumentCategory_DocDesc { get; set; }
        public string DocumentCategory_AuthNumber { get; set; }
    }
}
