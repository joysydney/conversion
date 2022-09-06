using Archival.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface IUtil<T>
    {
        string GetValidFileName(string fileName);
        string GetBufferPath(T record, bool cleanUp = false, bool isPDF = false);
        string GetReportPath(T record);
        string GetFolderReportPath(Summary summary);
        bool IsPasswordProtected(string filePath);
        bool IsInvalidFormat(string filePath);
        void EnsureFilesAreCleanedUp(string dirPath);
    }
}
