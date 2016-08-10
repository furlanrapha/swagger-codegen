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

When running the application, it will ask you four parameters:
- URL of the `.json` file from Swagger
- API Namespace/Package to be translated at the generation of the files
- Client Namespace/Package to be translated at the generation of the files
- Path to save the generated files
