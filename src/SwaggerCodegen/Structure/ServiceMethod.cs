using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen.Structure
{
    public class ServiceMethod
    {
        public ServiceMethod()
        {
            this.Parameters = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public string HttpVerb { get; set; }
        public string RouteUrl { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
