using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireStarterApp_ApiService>("apiservice");

var webApp = builder.AddProject<Projects.AspireStarterApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

// set to true if you want to use OpenAI
bool useOpenAI = true;
if (useOpenAI)
{
    const string openAIName = "openai";
    const string textEmbeddingName = "text-embedding-3-small";
    const string chatModelName = "gpt-35-turbo-16k";
    // to use an existing OpenAI resource, add the following to the AppHost user secrets:
    // "ConnectionStrings": {
    //   "openai": "Key=<API Key>" (to use https://api.openai.com/)
    //     -or-
    //   "openai": "Endpoint=https://<name>.openai.azure.com/" (to use Azure OpenAI)
    // }
    IResourceBuilder<IResourceWithConnectionString> openAI;
    if (builder.Configuration.GetConnectionString(openAIName) is not null)
    {
        openAI = builder.AddConnectionString(openAIName);

        apiService
        .WithReference(openAI)
        .WithEnvironment("AI__OPENAI__EMBEDDINGNAME", textEmbeddingName);
        webApp
            .WithReference(openAI)
            .WithEnvironment("AI__OPENAI__CHATMODEL", chatModelName);
    }
    else
    {
        //// to use Azure provisioning, add the following to the AppHost user secrets:
        //// "Azure": {
        ////   "SubscriptionId": "<your subscription ID> "
        ////   "Location": "<location>"
        //// }
        //openAI = builder.AddAzureOpenAI(openAIName)
        //    .AddDeployment(new AzureOpenAIDeployment(chatModelName, "gpt-35-turbo", "0613"))
        //    .AddDeployment(new AzureOpenAIDeployment(textEmbeddingName, "text-embedding-3-small", "1"));
    }
}

builder.Build().Run();
