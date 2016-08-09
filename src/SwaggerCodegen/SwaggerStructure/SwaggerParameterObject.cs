using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerParameterObject
    {
        public string name { get; set; }
        [JsonProperty("in")]
        public string _in { get; set; }
        public string description { get; set; }
        public bool required { get; set; }
        [JsonProperty("schema")]
        public SwaggerSchemaObject schema { get; set; }
        public string type { get; set; }
        public string format { get; set; }
        public bool allowEmptyValue { get; set; }
        public SwaggerSchemaObject items { get; set; }
        public string collectionFormat { get; set; }
        [JsonProperty("default")]
        public string _default { get; set; }
        public float maximum { get; set; }
        public bool exclusiveMaximum { get; set; }
        public float minimum { get; set; }
        public bool exclusiveMinimum { get; set; }
        public int maxLength { get; set; }
        public int minLength { get; set; }
        public string pattern { get; set; }
        public int maxItems { get; set; }
        public int minItems { get; set; }
        public bool uniqueItems { get; set; }
        [JsonProperty("enum")]
        public List<string> _enum { get; set; }
        public float multipleOf { get; set; }
    }
}