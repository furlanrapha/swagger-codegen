using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerCodeGenerator
{
    public class Program
    {
        public static string NameSpaceFromAPI = "Kondominio";
        public static string NameSpace = "Kondominio.Web.API";
        public static string PathToCreate = @"C:\TEMP_SWAGGER\";

        static void Main(string[] args)
        {
            List<ViewModelClass> viewModelList = new List<ViewModelClass>();

            Console.WriteLine("--- INICIANDO CONSTRUÇÃO DO CLIENTE DA API");

            RestClient client = new RestClient("http://kondominio-api.gear.host/");
            RestRequest request = new RestRequest("swagger/docs/v1");

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                JObject responseJson = JObject.Parse(response.Content);

                Console.WriteLine("---------------------------------------------------------------");
                Console.WriteLine("  HOST : " + responseJson.SelectToken("host"));
                Console.WriteLine("---------------------------------------------------------------");

                Console.WriteLine("> SERVIÇOS (Quantidade: " + responseJson.SelectToken("paths").ToObject<JObject>().Count + ")");

                foreach (var item in responseJson.SelectToken("paths").ToObject<JObject>())
                {
                    Console.WriteLine(item.Key);
                }

                Console.WriteLine("> CLASSES (Quantidade: " + responseJson.SelectToken("definitions").ToObject<JObject>().Count + ")");

                foreach (var definition in responseJson.SelectToken("definitions").ToObject<JObject>())
                {
                    ViewModelClass viewModelClass = new ViewModelClass();

                    string nameOfTheClass = definition.Key.Replace(NameSpaceFromAPI, NameSpace);

                    viewModelClass.NameOfClass = nameOfTheClass;

                    foreach (var property in definition.Value.SelectToken("properties").ToObject<JObject>())
                    {
                        string CSFormat = "string";

                        var propertyType = property.Value.SelectToken("type");

                        if (propertyType == null)
                        {
                            var propertyRef = property.Value.SelectToken("$ref").ToObject<string>();

                            CSFormat = propertyRef.Replace("#/definitions/", "");
                        }
                        else
                        {
                            var propertyFormat = property.Value.SelectToken("format");
                            
                            if (propertyFormat == null)
                            {
                                CSFormat = propertyType.ToObject<string>();
                            }
                            else
                            {
                                var format = propertyFormat.ToObject<string>();

                                if (format.Equals("int32"))
                                {
                                    CSFormat = "int";
                                }
                                else if (format.Equals("double"))
                                {
                                    CSFormat = "decimal";
                                }
                                else if (format.Equals("date-time"))
                                {
                                    CSFormat = "DateTime";
                                }
                            }
                        }

                        viewModelClass.Attributes.Add(property.Key, CSFormat);
                    }

                    viewModelList.Add(viewModelClass);

                    Console.WriteLine(viewModelClass.ToString());
                }
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
            }

            Console.ReadLine();

            if (!Directory.Exists(PathToCreate))
            {
                Directory.CreateDirectory(PathToCreate);
            }

            Console.WriteLine("> CRIANDO OS ARQUIVOS DOS VIEW MODELS");

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

                string directoryOfTheFile = PathToCreate + @"ViewModel\" + subNamespaceFolder;

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
                        viewModelClassFileStream.WriteLine("namespace " + NameSpace + ".ViewModel." + subNamespace);
                    }
                    else
                    {
                        viewModelClassFileStream.WriteLine("namespace " + NameSpace + ".ViewModel");
                    }

                    viewModelClassFileStream.WriteLine("{");

                    viewModelClassFileStream.WriteLine("    public class " + className);

                    viewModelClassFileStream.WriteLine("    {");

                    foreach (var attribute in viewModelClass.Attributes)
                    {
                        viewModelClassFileStream.WriteLine();

                        if (attribute.Value.IndexOf('.') > 0)
                        {
                            viewModelClassFileStream.WriteLine("        public " + GetNewNameFromClass(attribute.Value) + " " + attribute.Key + " { get; set; }");
                        }
                        else
                        {
                            viewModelClassFileStream.WriteLine("        public " + attribute.Value + " " + attribute.Key + " { get; set; }");
                        }
                    }

                    viewModelClassFileStream.WriteLine("    }");

                    viewModelClassFileStream.WriteLine("}");
                }

                Console.WriteLine("- '" + nameOfTheFile + "' criado com sucesso!");
            }

            Console.ReadLine();
        }

        private static string GetNewNameFromClass(string className)
        {
            var nameOfClassList = className.Split('.');

            string subNamespace = nameOfClassList[nameOfClassList.Count() - 2];
            className = nameOfClassList[nameOfClassList.Count() - 1];
            
            return NameSpace + ".ViewModel." + subNamespace + "." + className;
        }
    }
}
