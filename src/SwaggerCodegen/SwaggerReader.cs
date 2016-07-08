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

        public static List<ViewModelClass> ReadViewModels(JObject swaggerJsonFile, string apiNameSpace, string clientNameSpace)
        {
            List<ViewModelClass> viewModelList = new List<ViewModelClass>();

            foreach (var definition in swaggerJsonFile.SelectToken("definitions").ToObject<JObject>())
            {
                ViewModelClass viewModelClass = new ViewModelClass();

                string nameOfTheClass = definition.Key.Replace(apiNameSpace, clientNameSpace);

                viewModelClass.NameOfClass = nameOfTheClass;

                foreach (var property in definition.Value.SelectToken("properties").ToObject<JObject>())
                {
                    string CSharpType = "string";

                    var propertyType = property.Value.SelectToken("type");

                    if (propertyType == null)
                    {
                        var propertyRef = property.Value.SelectToken("$ref").ToObject<string>();

                        CSharpType = propertyRef.Replace("#/definitions/", "");
                    }
                    else
                    {
                        var propertyFormat = property.Value.SelectToken("format");

                        if (propertyFormat == null)
                        {
                            CSharpType = propertyType.ToObject<string>();
                        }
                        else
                        {
                            var format = propertyFormat.ToObject<string>();

                            if (format.Equals("int32"))
                            {
                                CSharpType = "int";
                            }
                            else if (format.Equals("double"))
                            {
                                CSharpType = "decimal";
                            }
                            else if (format.Equals("date-time"))
                            {
                                CSharpType = "DateTime";
                            }
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
    }
}
