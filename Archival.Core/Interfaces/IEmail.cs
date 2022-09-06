using Archival.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface IEmail 
    {
        void sendEmailNotification(Summary summary);
        void sendFailedEmailNotification(Summary summary);
    }
}
