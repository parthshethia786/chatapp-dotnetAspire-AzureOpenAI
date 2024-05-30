using AspireStarterApp.Web;
using AspireStarterApp.Web.Components;
using AspireApp.WebApp;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

//Register AzureOpenAIClient and AzureOpenAIChatCompletion service
var openAIOptions = builder.Configuration.GetSection("AI").Get<AIOptions>()?.OpenAI;
var deploymentName = openAIOptions?.ChatModel;

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("openai")) && !string.IsNullOrWhiteSpace(deploymentName))
{
    builder.Services.AddKernel();
    builder.AddAzureOpenAIClient("openai");
    builder.Services.AddAzureOpenAIChatCompletion(deploymentName);
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
