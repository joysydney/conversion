using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Model
{
    public partial class Permissions
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("data")]
        public ResultData Data { get; set; }
    }

    public partial class ResultData
    {
        [JsonProperty("permissions")]
        public PermissionsData Permissions { get; set; }
    }

    public partial class PermissionsData
    {
        [JsonProperty("right_id")]
        public long? RightId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}

