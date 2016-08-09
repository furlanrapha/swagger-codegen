using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerOperationObject
    {
        public List<string> tags { get; set; }
        public string summary { get; set; }
        public SwaggerExternalDocumentationObject externalDocs { get; set; }
        public string operationId { get; set; }
        public List<string> consumes { get; set; }
        public List<string> produces { get; set; }
        public List<SwaggerParameterObject> parameters { get; set; }
        public Dictionary<string, SwaggerResponseObject> responses { get; set; }
        public List<string> schemes { get; set; }
        public bool deprecated { get; set; }
        public SwaggerSecurityRequirementObject security { get; set; }
    }
}