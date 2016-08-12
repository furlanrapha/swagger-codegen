using Newtonsoft.Json.Linq;
using SwaggerCodegen.Structure;
using SwaggerCodegen.SwaggerStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwaggerCodegen
{
    public class SwaggerReader
    {
        public static List<ServiceClass> ReadServices(SwaggerObject swaggerObject, string apiNameSpace, string clientNameSpace)
        {
            List<ServiceClass> serviceList = new List<ServiceClass>();

            foreach (var path in swaggerObject.paths)
            {
                if (path.Value.parameters != null)
                {
                    // TODO: Check what value comes
                }

                if (path.Value.get != null)
                {
                    var serviceClass = GetOrGenerateServiceClass(ref serviceList, path.Value.get);

                    AddServiceMethod(ref serviceClass, HttpVerb.GET, path.Key, path.Value.get, apiNameSpace, clientNameSpace);
                }

                if (path.Value.post != null)
                {
                    var serviceClass = GetOrGenerateServiceClass(ref serviceList, path.Value.post);

                    AddServiceMethod(ref serviceClass, HttpVerb.POST, path.Key, path.Value.post, apiNameSpace, clientNameSpace);
                }

                if (path.Value.put != null)
                {
                    var serviceClass = GetOrGenerateServiceClass(ref serviceList, path.Value.put);

                    AddServiceMethod(ref serviceClass, HttpVerb.PUT, path.Key, path.Value.put, apiNameSpace, clientNameSpace);
                }

                if (path.Value.delete != null)
                {
                    var serviceClass = GetOrGenerateServiceClass(ref serviceList, path.Value.delete);

                    AddServiceMethod(ref serviceClass, HttpVerb.DELETE, path.Key, path.Value.delete, apiNameSpace, clientNameSpace);
                }

                if (path.Value.patch != null)
                {
                    var serviceClass = GetOrGenerateServiceClass(ref serviceList, path.Value.patch);

                    AddServiceMethod(ref serviceClass, HttpVerb.PATCH, path.Key, path.Value.patch, apiNameSpace, clientNameSpace);
                }

                if (path.Value.options != null)
                {
                    var serviceClass = GetOrGenerateServiceClass(ref serviceList, path.Value.options);

                    AddServiceMethod(ref serviceClass, HttpVerb.OPTIONS, path.Key, path.Value.options, apiNameSpace, clientNameSpace);
                }

                if (path.Value.head != null)
                {
                    var serviceClass = GetOrGenerateServiceClass(ref serviceList, path.Value.head);

                    AddServiceMethod(ref serviceClass, HttpVerb.HEAD, path.Key, path.Value.head, apiNameSpace, clientNameSpace);
                }
            }
            
            return serviceList;
        }

        private static ServiceClass GetOrGenerateServiceClass(ref List<ServiceClass> serviceList, SwaggerOperationObject swaggerOperationObject)
        {
            string className = swaggerOperationObject.tags[0];

            ServiceClass serviceClass = serviceList.FirstOrDefault(x => x.NameOfClass.Equals(className));

            if (serviceClass == null)
            {
                serviceClass = new ServiceClass();

                serviceClass.NameOfClass = className;

                serviceList.Add(serviceClass);
            }

            return serviceClass;
        }

        private static void AddServiceMethod(ref ServiceClass serviceClass, HttpVerb httpVerb, string routeUrl, SwaggerOperationObject swaggerOperationObject, string apiNameSpace, string clientNameSpace)
        {
            ServiceMethod serviceMethod = new ServiceMethod();

            serviceMethod.Name = swaggerOperationObject.operationId;
            serviceMethod.HttpVerb = httpVerb;
            serviceMethod.RouteUrl = routeUrl;

            if (swaggerOperationObject.parameters != null)
            {
                foreach (var parameter in swaggerOperationObject.parameters)
                {
                    string CSharpType;

                    if (parameter.type == null)
                    {
                        // Swagger works with JSON Pointer for $ref
                        string jsonPointer = "#/definitions/";

                        CSharpType = TranslateNameSpace(parameter.schema._ref.Substring(jsonPointer.Length), apiNameSpace, clientNameSpace);
                    }
                    else
                    {
                        if (parameter.format == null)
                        {
                            CSharpType = parameter.type;
                        }
                        else
                        {
                            CSharpType = TranslateFormat(parameter.format);
                        }
                    }

                    serviceMethod.Parameters.Add(parameter.name, CSharpType);
                }
            }

            serviceClass.Methods.Add(serviceMethod);
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
