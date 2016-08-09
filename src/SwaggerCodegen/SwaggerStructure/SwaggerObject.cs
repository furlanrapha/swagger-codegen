using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerObject
    {
        public string swagger { get; set; }
        public SwaggerInfoObject info { get; set; }
        public string host { get; set; }
        public string basePath { get; set; }
        public List<string> schemes { get; set; }
        public List<string> consumes { get; set; }
        public List<string> produces { get; set; }
        public SwaggerPathObject paths { get; set; }
        public SwaggerDefinitionObject definitions { get; set; }
        public SwaggerParameterObject parameters { get; set; }
        public SwaggerResponseObject responses { get; set; }
        public SwaggerSecurityDefinitionObject securityDefinitions { get; set; }
        public SwaggerSecurityRequirementObject security { get; set; }
        public List<SwaggerTagObject> tags { get; set; }
        public SwaggerExternalDocumentationObject externalDocs { get; set; }
    }
}