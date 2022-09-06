using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Configuration
{
    public class BaseConfiguration
    {
        public const string APIConnection = "APIConnection"; 
        public string Username { get; set; }
        public string Password { get; set; }
        public string Destination_URL { get; set; }
        public string CaseCategoryID { get; set; } 
        public string CaseCategory_CaseID { get; set; } 
        public string CaseCategory_CaseTitle { get; set; } 
        public string CaseCategory_OpenDate { get; set; } 
        public string CaseCategory_CloseDate { get; set; } 
        public string CaseCategory_Author { get; set; } 
        public string DocumentCategory_PrevDocNo { get; set; }
        public string DocumentCategory_AuthNumber { get; set; }
        public string DocumentCategory_SecurityClassif { get; set; }
        public string DocumentCategory_DocDesc { get; set; }
        public string DocumentCategoryID { get; set; }
        public bool UseSecureCredentials { get; set; }
        public Secure secureInfo { get; set; }
        public DocmanSetting docmanSetting { get; set; }
        public AuthSetting authSetting { get; set; }
    }

    public class Secure
    {
        public string Path { get; set; }
        public FileName fileName { get; set; }
    }

    public class FileName
    {
        public string AESKey { get; set; }
        public string CSUsername { get; set; }
        public string CSPassword { get; set; }
        public string CSDBUsername { get; set; }
        public string CSDBPassword { get; set; }
    }

    public class DocmanSetting
    {
        public string Endpoint { get; set; }
    }
    public class AuthSetting
    {
        public string Endpoint { get; set; }
    }
}
