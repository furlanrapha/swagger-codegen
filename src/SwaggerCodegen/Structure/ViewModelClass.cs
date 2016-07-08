using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerCodegen.Structure
{
    public class ViewModelClass
    {
        public ViewModelClass()
        {
            this.Properties = new List<ViewModelProperty>();
        }

        public string NameOfClass { get; set; }

        public List<ViewModelProperty> Properties { get; set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
                
            result.AppendLine("> VIEW MODEL NAME - " + NameOfClass);

            foreach (var item in Properties)
            {
                result.AppendLine("+ " + item.Type + " : " + item.Name);
            }

            return result.ToString();
        }
    }
}
