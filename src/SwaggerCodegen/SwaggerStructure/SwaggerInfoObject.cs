using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.SwaggerStructure
{
    public class SwaggerInfoObject
    {
        public string title { get; set; }
        public string description { get; set; }
        public string termsOfService { get; set; }
        public SwaggerContactObject contact { get; set; }
        public SwaggerLicenseObject license { get; set; }
        public string version { get; set; }
    }
}