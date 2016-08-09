using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerTagObject
    {
        public string name { get; set; }
        public string description { get; set; }
        public SwaggerExternalDocumentationObject externalDocs { get; set; }
    }
}