using Archival.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface ICSVReport<T>
    {
        Task LogNASRecord(T record);
        Task LogNASFolder(Summary summary);
        Task LogPDF(T record);
        Task LogDaily(T record);
    }
}
