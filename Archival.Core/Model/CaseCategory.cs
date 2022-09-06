using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public class CaseCategory
    { 
        public string CaseCategory_CaseID { get; set; }
        public string CaseCategory_CaseTitle { get; set; }
        public string CaseCategory_OpenDate { get; set; }
        public string CaseCategory_CloseDate { get; set; } 
        public string CaseCategory_Author { get; set; } 
    }
}
