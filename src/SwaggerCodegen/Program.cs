using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwaggerCodegen.Structure;
using SwaggerCodegen.SwaggerStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SwaggerCodegen
{
    public class Program
    {
        static void Main(string[] args)
        {
            string configFileName = "config.json";
            
            if (!File.Exists(configFileName))
            {
                var streamWriter = File.CreateText(configFileName);

                // Unload the file
                streamWriter.Dispose();
            }

            var configFile = File.OpenText(configFileName);

            ConfigurationData config = JsonConvert.DeserializeObject<ConfigurationData>(configFile.ReadToEnd());

            // Unload the file
            configFile.Dispose();

            if (config == null)
            {
                config = new ConfigurationData()
                {
                    APIUrl = "",
                    APISwaggerUrl = "",
                    APINameSpace = "",
                    ClientNameSpace = "",
                    FolderPath = ""
                };
            }
            else
            {
                Console.WriteLine("--- DETECTED A CONFIGURATION FILE IN THE SYSTEM");

                Console.WriteLine("> API URL: " + config.APIUrl);

                Console.WriteLine("> API URL FROM SWAGGER FILE: " + config.APISwaggerUrl);

                Console.WriteLine("> API NAMESPACE: " + config.APINameSpace);

                Console.WriteLine("> CLIENT NAMESPACE: " + config.ClientNameSpace);

                Console.WriteLine("> FOLDER: " + config.FolderPath);

                Console.WriteLine();
            }

            Console.WriteLine("--- GETTING THE CONFIGURATION FROM THE USER");

            string inputFromUser = "";

            Console.WriteLine("> ENTER THE API URL: (EXAMPLE: \"http://api.yourwebsite.com/\")");
            inputFromUser = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(inputFromUser))
                config.APIUrl = inputFromUser;

            Console.WriteLine("> ENTER THE API URL FROM SWAGGER FILE: (EXAMPLE: \"http://api.yourwebsite.com/swagger/docs/v1\")");
            inputFromUser = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(inputFromUser))
                config.APISwaggerUrl = inputFromUser;

            Console.WriteLine("> ENTER THE API NAMESPACE: (EXAMPLE: \"ExampleNameSpace\")");
            inputFromUser = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(inputFromUser))
                config.APINameSpace = inputFromUser;

            Console.WriteLine("> ENTER THE CLIENT NAMESPACE: (EXAMPLE: \"ExampleNameSpace.Web.MyClientAPI\")");
            inputFromUser = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(inputFromUser))
                config.ClientNameSpace = inputFromUser;

            Console.WriteLine("> ENTER THE FOLDER: (EXAMPLE: \"C:\\TEMP_SWAGGER\\\")");
            inputFromUser = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(inputFromUser))
                config.FolderPath = inputFromUser;

            if (!config.FolderPath.EndsWith("\\"))
                config.FolderPath = config.FolderPath + "\\";

            File.WriteAllText(configFileName, JsonConvert.SerializeObject(config));

            Console.WriteLine("--- CONFIGURATION INSERTED");

            Console.WriteLine("--- GETTING THE JSON FILE FROM SWAGGER");

            SwaggerObject swaggerObject = GetSwaggerObject(config.APISwaggerUrl);

            Console.WriteLine("- JSON File Loaded");

            Console.WriteLine("--- GROUPING THE DATA FROM THE JSON FILE");

            DateTime executionDateTime = DateTime.Now;
            
            var services = SwaggerReader.ReadServices(swaggerObject, config.APINameSpace, config.ClientNameSpace);

            Console.WriteLine("> SERVICES (Quantity: " + services.Count + ")");
            Console.WriteLine("");

            foreach (var serviceClass in services)
            {
                Console.WriteLine(serviceClass.ToString());
            }

            Console.WriteLine("> EXECUTION TIME: " + (DateTime.Now - executionDateTime).TotalSeconds + " SECONDS");
            Console.WriteLine("");
            Console.WriteLine("> PRESS ANY KEY TO GENERATE THE FILES...");

            Console.ReadLine();

            executionDateTime = DateTime.Now;

            var viewModels = SwaggerReader.ReadViewModels(swaggerObject, config.APINameSpace, config.ClientNameSpace);
            
            Console.WriteLine("> VIEW MODELS (Quantity: " + viewModels.Count + ")");
            Console.WriteLine("");

            foreach (var viewModelClass in viewModels)
            {
                Console.WriteLine(viewModelClass.ToString());
            }

            Console.WriteLine("> EXECUTION TIME: " + (DateTime.Now - executionDateTime).TotalSeconds + " SECONDS");
            Console.WriteLine("");
            Console.WriteLine("> PRESS ANY KEY TO GENERATE THE FILES...");

            Console.ReadLine();

            Infrastructure infrastructure = new Infrastructure();
            infrastructure.AdditionalHeaderParameters = SwaggerReader.ReadAdditionalHeaders(swaggerObject);

            executionDateTime = DateTime.Now;

            ConfigureFolder(config.FolderPath);

            Console.WriteLine("> CREATING THE SERVICES FILES");
            Console.WriteLine("");

            SwaggerWriter.WriteServices(services, infrastructure, config.APIUrl, config.APINameSpace, config.ClientNameSpace, config.FolderPath);

            Console.WriteLine("");
            Console.WriteLine("> CREATING THE VIEW MODELS FILES");
            Console.WriteLine("");

            SwaggerWriter.WriteViewModels(viewModels, config.APINameSpace, config.ClientNameSpace, config.FolderPath);

            Console.WriteLine("");
            Console.WriteLine("> EXECUTION TIME: " + (DateTime.Now - executionDateTime).TotalSeconds + " SECONDS");
            Console.WriteLine("");
            Console.WriteLine("> PROCCESS EXECUTED WITH SUCCESS");

            Console.ReadLine();
        }

        private static void ConfigureFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static SwaggerObject GetSwaggerObject(string APIUrl)
        {
            WebRequest request = WebRequest.Create(APIUrl);

            var response = request.GetResponseAsync().Result;
            
            var responseReader = new StreamReader(response.GetResponseStream());
            var responseData = responseReader.ReadToEndAsync().Result;

            var swaggerObject = JsonConvert.DeserializeObject<SwaggerObject>(responseData, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            });

            return swaggerObject;
        }
    }
}
