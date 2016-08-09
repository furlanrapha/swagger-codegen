using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerPathItemObject
    {
        [JsonProperty("$ref")]
        public string _ref { get; set; }
        public SwaggerOperationObject get { get; set; }
        public SwaggerOperationObject put { get; set; }
        public SwaggerOperationObject post { get; set; }
        public SwaggerOperationObject delete { get; set; }
        public SwaggerOperationObject options { get; set; }
        public SwaggerOperationObject head { get; set; }
        public SwaggerOperationObject patch { get; set; }
        public List<SwaggerParameterObject> parameters { get; set; }
    }
}