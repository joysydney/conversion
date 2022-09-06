using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface IFolder
    { 
        public string AuthorityNumber { get; set; }
        public string ReviewDate { get; set; }
        public string FolderTitle { get; set; }
    }
}
