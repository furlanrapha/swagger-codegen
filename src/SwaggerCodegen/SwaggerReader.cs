using Newtonsoft.Json.Linq;
using SwaggerCodegen.Structure;
using SwaggerCodegen.SwaggerStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerCodegen
{
    public class SwaggerReader
    {
        public static List<ServiceClass> ReadServices(JObject swaggerJsonFile)
        {
            List<ServiceClass> serviceList = new List<ServiceClass>();

            foreach (var restService in swaggerJsonFile.SelectToken("paths").ToObject<JObject>())
            {
                var restServiceHttpVerbs = restService.Value.ToObject<JObject>().Properties().Select(x => x.Name);

                foreach (string restServiceHttpVerb in restServiceHttpVerbs)
                {
                    string nameOfClass = restService.Value.SelectToken(restServiceHttpVerb + ".tags").ToObject<List<string>>().FirstOrDefault();

                    string nameOfMethod = "MethodWithoutName";

                    // CHECK IF HAS THE NAME OF THE METHOD AND SPLIT THE INITIAL NAME OF THE CLASS FROM THE METHOD
                    if (restService.Value.SelectToken(restServiceHttpVerb + ".operationId") != null)
                        nameOfMethod = restService.Value.SelectToken(restServiceHttpVerb + ".operationId").ToObject<string>().Split('_')[1];
                    else
                        nameOfMethod = restService.Key.Split('/').LastOrDefault();

                    ServiceClass serviceClass;

                    if (!serviceList.Any(x => x.NameOfClass.Equals(nameOfClass)))
                    {
                        serviceClass = new ServiceClass();

                        serviceClass.NameOfClass = nameOfClass;

                        ServiceMethod serviceMethod = ReadServiceMethod(restService, restServiceHttpVerb, nameOfMethod);

                        serviceClass.Methods.Add(serviceMethod);

                        serviceList.Add(serviceClass);
                    }
                    else
                    {
                        serviceClass = serviceList.SingleOrDefault(x => x.NameOfClass.Equals(nameOfClass));

                        ServiceMethod serviceMethod = ReadServiceMethod(restService, restServiceHttpVerb, nameOfMethod);

                        serviceClass.Methods.Add(serviceMethod);
                    }
                }
            }
            
            return serviceList;
        }

        private static ServiceMethod ReadServiceMethod(KeyValuePair<string, JToken> restService, string restServiceHttpVerb, string nameOfMethod)
        {
            ServiceMethod serviceMethod = new ServiceMethod();

            serviceMethod.Name = nameOfMethod;
            serviceMethod.HttpVerb = restServiceHttpVerb.ToUpper();
            serviceMethod.RouteUrl = restService.Key;

            if (restService.Value.SelectToken(restServiceHttpVerb + ".parameters") != null)
            {
                foreach (var jsonParameter in restService.Value.SelectToken(restServiceHttpVerb + ".parameters").ToList())
                {
                    SwaggerJSONParameter parameter = jsonParameter.ToObject<SwaggerJSONParameter>();

                    if (parameter.Type == null)
                    {
                        string parameterRef = jsonParameter.SelectToken("schema.$ref").ToObject<string>();

                        parameterRef = parameterRef.Replace("#/definitions/", "");

                        serviceMethod.Parameters.Add(parameter.Name, parameterRef);
                    }
                    else
                    {
                        serviceMethod.Parameters.Add(parameter.Name, parameter.Type);
                    }
                }
            }

            return serviceMethod;
        }

        public static List<ViewModelClass> ReadViewModels(SwaggerObject swaggerObject, string apiNameSpace, string clientNameSpace)
        {
            List<ViewModelClass> viewModelList = new List<ViewModelClass>();

            foreach (var definition in swaggerObject.definitions)
            {
                ViewModelClass viewModelClass = new ViewModelClass();
                
                viewModelClass.NameOfClass = TranslateNameSpace(definition.Key, apiNameSpace, clientNameSpace);

                foreach (var property in definition.Value.properties)
                {
                    string CSharpType;
                    
                    if (property.Value.type == null)
                    {
                        // Swagger works with JSON Pointer for $ref
                        string jsonPointer = "#/definitions/";

                        CSharpType = TranslateNameSpace(property.Value._ref.Substring(jsonPointer.Length), apiNameSpace, clientNameSpace);
                    }
                    else
                    {
                        if (property.Value.format == null)
                        {
                            CSharpType = property.Value.type;
                        }
                        else
                        {
                            CSharpType = TranslateFormat(property.Value.format);
                        }
                    }

                    viewModelClass.Properties.Add(new ViewModelProperty
                    {
                        Name = property.Key,
                        Type = CSharpType
                    });
                }

                viewModelList.Add(viewModelClass);
            }

            return viewModelList;
        }

        /// <summary>
        /// Translates the full class name using the api and client namespace to make a replace
        /// </summary>
        /// <param name="fullClassName">The full namespace plus the class name</param>
        /// <param name="apiNameSpace">The API namespace</param>
        /// <param name="clientNameSpace">The Client namespace</param>
        /// <returns>The translated full class name</returns>
        private static string TranslateNameSpace(string fullClassName, string apiNameSpace, string clientNameSpace)
        {
            if (!fullClassName.StartsWith(apiNameSpace))
            {
                return fullClassName;
            }
            else
            {
                return clientNameSpace + fullClassName.Substring(apiNameSpace.Length);
            }
        }

        /// <summary>
        /// Translates the format from Swagger to a C# format
        /// </summary>
        /// <param name="format">The format from Swagger</param>
        /// <returns>The C# format</returns>
        private static string TranslateFormat(string format)
        {
            if (format.Equals("int32"))
            {
                return "int";
            }
            else if (format.Equals("int64"))
            {
                return "int";
            }
            else if (format.Equals("float"))
            {
                return "float";
            }
            else if (format.Equals("double"))
            {
                return "double";
            }
            else if (format.Equals("byte"))
            {
                return "byte";
            }
            else if (format.Equals("binary"))
            {
                return "byte[]";
            }
            else if (format.Equals("date"))
            {
                return "DateTime";
            }
            else if (format.Equals("date-time"))
            {
                return "DateTime";
            }
            else if (format.Equals("password"))
            {
                return "string";
            }
            else
            {
                return "string";
            }
        }
    }
}
