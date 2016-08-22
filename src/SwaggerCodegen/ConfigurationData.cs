using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen
{
    public class ConfigurationData
    {
        public string APIUrl { get; set; }
        public string APISwaggerUrl { get; set; }
        public string FolderPath { get; set; }
        public string ClientNameSpace { get; set; }
        public string APINameSpace { get; set; }
    }
}
