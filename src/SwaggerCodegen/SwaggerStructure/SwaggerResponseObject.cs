using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerResponseObject
    {
        public string description { get; set; }
        [JsonProperty("schema")]
        public SwaggerSchemaObject schema { get; set; }
        public Dictionary<string, SwaggerHeaderObject> headers { get; set; }
        public Dictionary<string, object> examples { get; set; } // TODO: Create the SwaggerExampleObject
    }
}