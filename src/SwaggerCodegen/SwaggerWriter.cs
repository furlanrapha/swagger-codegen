using Newtonsoft.Json.Linq;
using SwaggerCodegen.Structure;
using SwaggerCodegen.SwaggerStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen
{
    public class SwaggerWriter
    {
        public static void WriteServices(List<ServiceClass> serviceList, string apiNameSpace, string clientNameSpace, string folderPath)
        {
            foreach (var serviceClass in serviceList)
            {
                string className = serviceClass.NameOfClass + "ClientService";
                
                string directoryOfTheFile = folderPath + @"ClientService\";

                if (!Directory.Exists(directoryOfTheFile))
                {
                    Directory.CreateDirectory(directoryOfTheFile);
                }

                string nameOfTheFile = directoryOfTheFile + className + ".cs";

                using (var viewModelClassFileStream = File.CreateText(nameOfTheFile))
                {
                    viewModelClassFileStream.WriteLine("using System;");
                    viewModelClassFileStream.WriteLine("using System.Collections.Generic;");
                    viewModelClassFileStream.WriteLine("using System.Linq;");
                    viewModelClassFileStream.WriteLine("using " + clientNameSpace + ".ViewModel;");
                    viewModelClassFileStream.WriteLine();
                    
                    viewModelClassFileStream.WriteLine("namespace " + clientNameSpace + ".ClientService");

                    viewModelClassFileStream.WriteLine("{");

                    viewModelClassFileStream.WriteLine("    public class " + className);

                    viewModelClassFileStream.WriteLine("    {");

                    foreach (var method in serviceClass.Methods)
                    {
                        viewModelClassFileStream.WriteLine();

                        string methodParameters = string.Join(", ", method.Parameters.Select(x => GetNewNameFromClass(clientNameSpace, x.Value) + " " + x.Key));

                        viewModelClassFileStream.WriteLine("        public void " + method.Name + "(" + methodParameters + ")");
                        viewModelClassFileStream.WriteLine("        {");

                        // TODO: MAKE THE REQUEST TO THE API SERVER

                        viewModelClassFileStream.WriteLine("        }");
                    }

                    viewModelClassFileStream.WriteLine("    }");

                    viewModelClassFileStream.WriteLine("}");
                }

                Console.WriteLine("- '" + nameOfTheFile + "' FILE CREATED");
            }
        }
        
        public static void WriteViewModels(List<ViewModelClass> viewModelList, string apiNameSpace, string clientNameSpace, string folderPath)
        {
            foreach (var viewModelClass in viewModelList)
            {
                string subNamespace = "";
                string subNamespaceFolder = "";
                string className = "";

                if (viewModelClass.NameOfClass.IndexOf('.') > 0)
                {
                    var nameOfClassList = viewModelClass.NameOfClass.Split('.');

                    subNamespace = nameOfClassList[nameOfClassList.Count() - 2];
                    subNamespaceFolder = subNamespace + @"\";
                    className = nameOfClassList[nameOfClassList.Count() - 1];
                }
                else
                {
                    className = viewModelClass.NameOfClass;
                }

                string directoryOfTheFile = folderPath + @"ViewModel\" + subNamespaceFolder;

                if (!Directory.Exists(directoryOfTheFile))
                {
                    Directory.CreateDirectory(directoryOfTheFile);
                }

                string nameOfTheFile = directoryOfTheFile + className + ".cs";

                using (var viewModelClassFileStream = File.CreateText(nameOfTheFile))
                {
                    viewModelClassFileStream.WriteLine("using System;");
                    viewModelClassFileStream.WriteLine("using System.Collections.Generic;");
                    viewModelClassFileStream.WriteLine("using System.Linq;");
                    viewModelClassFileStream.WriteLine();

                    if (!string.IsNullOrEmpty(subNamespace))
                    {
                        viewModelClassFileStream.WriteLine("namespace " + clientNameSpace + ".ViewModel." + subNamespace);
                    }
                    else
                    {
                        viewModelClassFileStream.WriteLine("namespace " + clientNameSpace + ".ViewModel");
                    }

                    viewModelClassFileStream.WriteLine("{");

                    viewModelClassFileStream.WriteLine("    public class " + className);

                    viewModelClassFileStream.WriteLine("    {");

                    foreach (var property in viewModelClass.Properties)
                    {
                        viewModelClassFileStream.WriteLine();
                        viewModelClassFileStream.WriteLine("        public " + GetNewNameFromClass(clientNameSpace, property.Type) + " " + property.Name + " { get; set; }");
                    }

                    viewModelClassFileStream.WriteLine("    }");

                    viewModelClassFileStream.WriteLine("}");
                }

                Console.WriteLine("- '" + nameOfTheFile + "' FILE CREATED");
            }
        }

        private static string GetNewNameFromClass(string nameSpace, string fullClassName)
        {
            // VERIFY IF IT IS A COMPLEX TYPE
            if (fullClassName.IndexOf('.') == -1)
                return fullClassName;

            var nameOfClassList = fullClassName.Split('.');

            string subNameSpace = nameOfClassList[nameOfClassList.Count() - 2];
            string className = nameOfClassList[nameOfClassList.Count() - 1];

            return nameSpace + ".ViewModel." + subNameSpace + "." + className;
        }
    }
}
