using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerSecuritySchemeObject
    {
        public string type { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        [JsonProperty("in")]
        public string _in { get; set; }
        public string flow { get; set; }
        public string authorizationUrl { get; set; }
        public string tokenUrl { get; set; }
        public Dictionary<string, string> scopes { get; set; }
    }
}
