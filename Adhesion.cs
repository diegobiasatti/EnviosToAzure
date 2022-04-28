using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace enviosShell
{
    public class Adhesion
    {
        [JsonProperty("idEstacion")]
        public string idEstacion { get; set; }

        [JsonProperty("terminos")]
        public string terminos { get; set; }

        [JsonProperty("token")]
        public string token { get; set; }

        [JsonProperty("result")]
        public string result { get; set; }

    }
}
