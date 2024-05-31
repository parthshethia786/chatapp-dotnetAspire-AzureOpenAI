using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireStarterApp_ApiService>("apiservice");

var webApp = builder.AddProject<Projects.AspireStarterApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

const string openAIName = "openai";
const string chatModelName = "gpt-35-turbo-16k";
// to use an existing OpenAI resource, add the following to the AppHost user secrets:
// "ConnectionStrings": {
//   "openai": "Key=<API Key>" (to use https://api.openai.com/)
//     -or-
//   "openai": "Endpoint=https://<name>.openai.azure.com/" (to use Azure OpenAI)
// }
IResourceBuilder<IResourceWithConnectionString> openAI;
openAI = builder.AddConnectionString(openAIName);

webApp
    .WithReference(openAI)
    .WithEnvironment("AI__OPENAI__CHATMODEL", chatModelName);

builder.Build().Run();
