# swagger-codegen

A console application that generates client API using the Swagger JSON file.

## Configuring

Clone the project and run the following command in the console/terminal

```shell
> dotnet restore
```

## Running

To run the application, just run the following command in the console/terminal

```shell
> dotnet run
```

### Parameters

When running the application, it will ask you five parameters:
- URL of the API (must be the root of the API)
- URL of the `.json` file from Swagger
- API Namespace/Package to be translated at the generation of the files
- Client Namespace/Package to be translated at the generation of the files
- Path to save the generated files

In the next time you run the application, it's not required to pass the
parameters again, just press `Enter` in all the parameters and the application
will use the last used parameters.
