using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Configuration
{
    public class EmailConfiguration
    {
        public const string Email = "Email";
        public string email_host { get; set; }
        public string email_from { get; set; }
        public string email_to { get; set; }
        public string email_cc { get; set; }
        public string subject_header { get; set; }
    }
}
