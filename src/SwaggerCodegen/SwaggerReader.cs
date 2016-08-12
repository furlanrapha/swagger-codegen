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

            if (string.IsNullOrEmpty(swaggerOperationObject.operationId))
                return;

            serviceMethod.Name = swaggerOperationObject.operationId;
            serviceMethod.HttpVerb = httpVerb;
            serviceMethod.RouteUrl = routeUrl;

            if (swaggerOperationObject.parameters != null)
            {
                foreach (var parameter in swaggerOperationObject.parameters)
                {
                    string CSharpType = TranslateCSharpType(parameter.type,
                                                            parameter.format,
                                                            parameter.schema != null ? parameter.schema._ref : null,
                                                            parameter.items,
                                                            apiNameSpace,
                                                            clientNameSpace);

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
                    string CSharpType = TranslateCSharpType(property.Value.type,
                                                            property.Value.format,
                                                            property.Value._ref,
                                                            property.Value.items,
                                                            apiNameSpace,
                                                            clientNameSpace);

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

        private static string TranslateCSharpType(string type, string format, string _ref, SwaggerSchemaObject schema, string apiNameSpace, string clientNameSpace)
        {
            string CSharpType;

            if (type == null)
            {
                CSharpType = TranslateJsonPointer(_ref, apiNameSpace, clientNameSpace);
            }
            else
            {
                if (format == null)
                {
                    CSharpType = TranslateType(type, schema, apiNameSpace, clientNameSpace);
                }
                else
                {
                    CSharpType = TranslateFormat(format);
                }
            }

            return CSharpType;
        }

        /// <summary>
        /// Translates the type from Swagger to a C# format
        /// </summary>
        /// <param name="type">The type from Swagger</param>
        /// <param name="schema">The schema from Swagger</param>
        /// <returns>The C# format</returns>
        private static string TranslateType(string type, SwaggerSchemaObject schema, string apiNameSpace, string clientNameSpace)
        {
            if (type.Equals("integer"))
            {
                return "long";
            }
            else if (type.Equals("number"))
            {
                return "double";
            }
            else if (type.Equals("boolean"))
            {
                return "bool";
            }
            else if (type.Equals("array"))
            {
                string CSharpType = TranslateCSharpType(schema.type, schema.format, schema._ref, schema.items, apiNameSpace, clientNameSpace);

                return "List<" + CSharpType + ">";
            }
            else
            {
                return type;
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

        /// <summary>
        /// Translates the $ref from Swagger to a C# format
        /// </summary>
        /// <param name="_ref">The $ref from Swagger</param>
        /// <returns>The C# format</returns>
        private static string TranslateJsonPointer(string _ref, string apiNameSpace, string clientNameSpace)
        {
            // Swagger works with JSON Pointer for $ref
            string jsonPointer = "#/definitions/";

            return TranslateNameSpace(_ref.Substring(jsonPointer.Length), apiNameSpace, clientNameSpace);
        }
    }
}
