using Newtonsoft.Json.Linq;
using SwaggerCodegen.Structure;
using SwaggerCodegen.SwaggerStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwaggerCodegen
{
    public class SwaggerWriter
    {
        public static void WriteServices(List<ServiceClass> serviceList, Infrastructure infrastructure, string baseApiUrl, string apiNameSpace, string clientNameSpace, string folderPath)
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
                    viewModelClassFileStream.WriteLine("using Newtonsoft.Json;");
                    viewModelClassFileStream.WriteLine("using System;");
                    viewModelClassFileStream.WriteLine("using System.Collections.Generic;");
                    viewModelClassFileStream.WriteLine("using System.Linq;");
                    viewModelClassFileStream.WriteLine("using System.Net.Http;");
                    viewModelClassFileStream.WriteLine("using System.Net.Http.Headers;");
                    viewModelClassFileStream.WriteLine("using System.Threading.Tasks;");
                    viewModelClassFileStream.WriteLine("using " + clientNameSpace + ".ViewModel;");
                    viewModelClassFileStream.WriteLine();
                    
                    viewModelClassFileStream.WriteLine("namespace " + clientNameSpace + ".ClientService");

                    viewModelClassFileStream.WriteLine("{");

                    viewModelClassFileStream.WriteLine("    public class " + className);

                    viewModelClassFileStream.WriteLine("    {");

                    if (infrastructure.AdditionalHeaderParameters.Count > 0)
                    {
                        string classConstructor = string.Empty;

                        foreach (var additionalHeader in infrastructure.AdditionalHeaderParameters)
                        {
                            viewModelClassFileStream.WriteLine("        private string header_" + additionalHeader + " { get; set; }");
                        }

                        viewModelClassFileStream.WriteLine();

                        var constructorParameters = string.Join(",", infrastructure.AdditionalHeaderParameters.Select(x => "string _header_" + x));

                        viewModelClassFileStream.WriteLine("        public " + className + "(" + constructorParameters + ")");
                        viewModelClassFileStream.WriteLine("        {");

                        foreach (var additionalHeader in infrastructure.AdditionalHeaderParameters)
                        {
                            viewModelClassFileStream.WriteLine("            header_" + additionalHeader + " = _header_" + additionalHeader + ";");
                        }
                        viewModelClassFileStream.WriteLine("        }");
                    }

                    foreach (var method in serviceClass.Methods)
                    {
                        viewModelClassFileStream.WriteLine();

                        string methodParameters = string.Join(", ", method.Parameters.Select(x => GetNewNameFromClass(clientNameSpace, x.Value) + " " + x.Key));

                        List<string> parametersInsideRouteUrl = new List<string>();

                        foreach (var parameter in method.Parameters)
                        {
                            if (method.RouteUrl.Contains("{" + parameter.Key + "}"))
                            {
                                parametersInsideRouteUrl.Add(parameter.Key);
                            }
                        }

                        if (string.IsNullOrEmpty(method.Returns))
                        {
                            viewModelClassFileStream.WriteLine("        public async Task " + method.Name + "(" + methodParameters + ")");
                        }
                        else
                        {
                            viewModelClassFileStream.WriteLine("        public async Task<" + method.Returns + "> " + method.Name + "(" + methodParameters + ")");
                        }

                        viewModelClassFileStream.WriteLine("        {");

                        viewModelClassFileStream.WriteLine("            HttpClient client = new HttpClient();");

                        viewModelClassFileStream.WriteLine("            client.BaseAddress = new Uri(\"" + baseApiUrl + "\");");
                        viewModelClassFileStream.WriteLine("            client.DefaultRequestHeaders.Accept.Clear();");
                        viewModelClassFileStream.WriteLine("            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        if (infrastructure.AdditionalHeaderParameters.Count > 0)
                        {
                            foreach (var additionalHeader in infrastructure.AdditionalHeaderParameters)
                            {
                                viewModelClassFileStream.WriteLine("            client.DefaultRequestHeaders.Add(\"" + additionalHeader + "\", header_" + additionalHeader + ");");
                            }
                        }

                        viewModelClassFileStream.Write("            HttpRequestMessage requestMessage = new HttpRequestMessage(");
                        viewModelClassFileStream.Write("new HttpMethod(\"" + method.HttpVerb.ToString() + "\"), ");
                        viewModelClassFileStream.Write("\"" + method.RouteUrl.Replace("{", "\" + ").Replace("}", " + \"") + "\"");
                        viewModelClassFileStream.WriteLine(");");

                        if (method.HttpVerb != HttpVerb.GET)
                        {
                            viewModelClassFileStream.WriteLine("            var formData = new Dictionary<string, string>();");
                            viewModelClassFileStream.WriteLine("            var formUrlEncodedContent = new FormUrlEncodedContent(formData);");
                            
                            viewModelClassFileStream.WriteLine("            requestMessage.Content = formUrlEncodedContent;");
                        }
                        
                        viewModelClassFileStream.WriteLine("            var httpResponseMessage = await client.SendAsync(requestMessage);");

                        viewModelClassFileStream.WriteLine("            if (!httpResponseMessage.IsSuccessStatusCode)");
                        viewModelClassFileStream.WriteLine("            {");
                        viewModelClassFileStream.WriteLine("                throw new Exception(\"Error connecting to the service\");");
                        viewModelClassFileStream.WriteLine("            }");

                        if (!string.IsNullOrEmpty(method.Returns))
                        {
                            viewModelClassFileStream.WriteLine("            else");
                            viewModelClassFileStream.WriteLine("            {");
                            viewModelClassFileStream.WriteLine("                var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();");
                            viewModelClassFileStream.WriteLine("                return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<" + method.Returns + ">(jsonResponse));");
                            viewModelClassFileStream.WriteLine("            }");
                        }

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

                    if (!string.IsNullOrEmpty(apiNameSpace))
                    {
                        subNamespace = nameOfClassList[nameOfClassList.Count() - 2];
                        subNamespaceFolder = subNamespace + @"\";
                    }

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

            // VERIFY IF IT IS A LIST
            if (fullClassName.StartsWith("List<") && fullClassName.EndsWith(">"))
                return fullClassName;

            var nameOfClassList = fullClassName.Split('.');

            string subNameSpace = nameOfClassList[nameOfClassList.Count() - 2];
            string className = nameOfClassList[nameOfClassList.Count() - 1];

            return nameSpace + "." + subNameSpace + "." + className;
        }
    }
}
