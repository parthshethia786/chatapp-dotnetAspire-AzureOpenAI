# .NET Aspire Chat application leveraging Azure Open AI service and gpt-35-turbo model

![alt text](https://github.com/parthshethia786/chatapp-dotnetAspire-AzureOpenAI/blob/master/Preview%20of%20the%20app.png)

Below is the step-by-step guide to run the application locally.

**Prerequisites:**
1.	Visual Studio V17.10.0 and .NET 8.0 (for .NET Aspire applications)
2.	Azure Developer CLI - <code>azd</code>
3.	Azure OpenAI service instance and gpt-35-turbo model deployed to it (we will see how to set this up below)
 
**Step 1:** 

Let’s create a sample .NET Aspire application:

- Open Visual Studio -> Create new project (example name AspireApp) -> Select .NET Aspire Starter Application template -> Select .NET 8.0 as target framework -> Enable – Configure for https
- Hit F5 and see the sample application running with default click counter and weather API modules. 

**Step 2:**

Let’s create a new Azure Open AI service instance using Azure Developer CLI:

- If you do not have Azure Developer CLI installed, you can install it on Windows using winget or visit the page to perform the download:
  - <code>winget install microsoft.azd</code> 
  - https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/install-azd?tabs=winget-windows%2Cbrew-mac%2Cscript-linux&pivots=os-windows 
- In Visual Studio, open the <code>Developer Powershell</code> (<code>View -> Terminal</code>).
- Navigate to the Project Folder - <code>AspireApp.AppHost</code>.
- Login to Azure, use <code>az login</code> and your Azure credentials.
- Set your Az CLI to your subscription: <code>az account set --subscription "yourSubscriptionId"</code>
- Create a new Resource Group: <code>az group create --name some-rg-name --location eastus2</code>
- Create a new Azure OpenAI Resource: <code>az cognitiveservices account create --name openai --resource-group some-rg-name --location eastus --kind OpenAI --sku s0 --subscription "yourSubscriptionId"</code>
- Deploy the GPT model: <code>az cognitiveservices account deployment create --name openai --resource-group some-rg-name --deployment-name gpt-35-turbo-16k --model-name gpt-35-turbo --model-version "0613" --model-format OpenAI --sku-capacity "1" --sku-name "Standard"</code>

**Step 3:**

Let’s integrate this new Azure Open AI service instance in our .NET Aspire project:

- First, lets add the endpoint and the keys in our project using below command:
  - Run this command from Developer Powershell in Visual Studio and from AppHost project file location - <code>dotnet user-secrets set "ConnectionStrings:OpenAI" "Endpoint=your-azure-openai-endpoint-here;Key=your-azure-openai-key-here"</code>
  - You can get the endpoint and key information from <code>Azure Portal -> Azure OpenAI resource -> Resource Management -> Keys and Endpoint</code>
- In the AspireApp.AppHost project, install NuGet package - <code>Aspire.Hosting.Azure.CognitiveServices</code>.
- In the AspireApp.AppHost project's <code>Program.cs</code>, add environment variable for Open AI connection string.
- In the web front end project - <code>AspireApp.Web</code>, install below NuGet packages:
  - <code>Aspire.Azure.AI.OpenAI</code>
  - <code>Microsoft.SemanticKernel</code>
- In the <code>Program.cs</code> of web front end project - <code>AspireApp.Web</code>, register <code>AzureOpenAIClient</code> and <code>AzureOpenAIChatCompletion service</code>. The core magic is with below 3 lines:
  - <code>builder.Services.AddKernel();</code>
  - <code>builder.AddAzureOpenAIClient("openai");</code>
  - <code>builder.Services.AddAzureOpenAIChatCompletion(deploymentName);</code> - deploymentName here will be <code>gpt-3.5-turbo-16k</code>
- Create a <code>ChatState.cs</code> wrapper that will be invoked from chat UI and will talk to chat completion service using the Semantic Kernel.
- In <code>ChatState.cs</code>, we are initializing the <code>ChatHistory</code> with system message (initial prompt to the model). This can be customized based on business needs. 
- The chat context is preserved is ChatHistory every time user asks a new questions/sends a new prompt. Hence entire context is provided to the model while processing the latest prompt.
- The chat UI is inspired from the eShop application from Microsoft. You can copy the <code>Chatbot</code> folder from this repo under your Web project of the application - AspireApp.Web, under the Components folder. Add the <code>chat.png</code> image under <code>wwwroot</code> folder of AspireApp.Web. Hook the <code>ShowChatbotButton</code> component in <code>MainLayout.razor</code>. 
- Once everything is wired up, run the .NET Aspire application and test the chat application from the webapp localhost URL.
- Note - the constraint to answer only about Microsoft related questions is defined using System Prompt, it can easily be bypassed using prompt injection or jailbreaks. Please ensure your applications have right controls in place and leverage the existing tools to prevent these type of attacks. 
  
**References**:

- https://github.com/Azure-Samples/eShop-AI-Lab-Build2024
