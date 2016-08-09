using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerHeaderObject
    {
        public string description { get; set; }
        public string type { get; set; }
        public string format { get; set; }
        public string items { get; set; }
        public string collectionFormat { get; set; }
        [JsonProperty("default")]
        public string _default { get; set; }
    }
}