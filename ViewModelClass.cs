using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerCodeGenerator
{
    public class ViewModelClass
    {
        public ViewModelClass()
        {
            this.Attributes = new Dictionary<string, string>();
        }

        public string NameOfClass { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
                
            result.AppendLine("> CLASS NAME - " + NameOfClass);

            foreach (var item in Attributes)
            {
                result.AppendLine("+ " + item.Value + " : " + item.Key);
            }

            return result.ToString();
        }
    }
}
