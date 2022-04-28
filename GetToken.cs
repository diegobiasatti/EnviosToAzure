using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace enviosShell
{
    public class GetToken
    {
        [JsonProperty("token_type")]
        public string token_type { get; set; }

        [JsonProperty("alive_from")]
        public string alive_from { get; set; }

        [JsonProperty("expires_on")]
        public string expires_on { get; set; }

        [JsonProperty("access_token")]
        public string access_token { get; set; }


    }
}
