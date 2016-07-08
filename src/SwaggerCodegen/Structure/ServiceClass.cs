using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerCodegen.Structure
{
    public class ServiceClass
    {
        public ServiceClass()
        {
            this.Methods = new List<ServiceMethod>();
        }

        public string NameOfClass { get; set; }

        public List<ServiceMethod> Methods { get; set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
                
            result.AppendLine("> SERVICE NAME - " + NameOfClass);

            foreach (var item in Methods)
            {
                string parameters = string.Join(", ", item.Parameters.Select(x => x.Value + " " + x.Key));

                result.AppendLine(item.HttpVerb + " " + item.RouteUrl);
                result.AppendLine("+ " + item.Name + "(" + parameters + ")");
            }

            return result.ToString();
        }
    }
}
