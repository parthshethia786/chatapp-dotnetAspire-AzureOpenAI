using System.ComponentModel;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace eShop.WebApp.Chatbot;

public class ChatState
{
    private readonly ClaimsPrincipal _user;
    private readonly ILogger _logger;
    private readonly Kernel _kernel;
    private readonly OpenAIPromptExecutionSettings _aiSettings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

    public ChatState(ClaimsPrincipal user, Kernel kernel, ILoggerFactory loggerFactory)
    {
        _user = user;
        _logger = loggerFactory.CreateLogger(typeof(ChatState));

        _kernel = kernel;

        Messages = new ChatHistory("""
            You are an AI customer service agent for the online company Microsoft.
            You NEVER respond about topics other than Microsoft.
            Your job is to answer customer questions about products in the Microsoft catalog.
            Microsoft primarily sells products and services related to AI, security, customer productivity, etc.
            You try to be concise and only provide longer responses if necessary.
            If someone asks a question about anything other than Microsoft, its catalog, or their account,
            you refuse to answer, and you instead ask if there's a topic related to Microsoft you can assist with.
            """);
        Messages.AddAssistantMessage("Hi! I'm SASsy. How can I help?");
    }

    public ChatHistory Messages { get; }

    public async Task AddUserMessageAsync(string userText, Action onMessageAdded)
    {
        Messages.AddUserMessage(userText);
        onMessageAdded();

        try
        {
            ChatMessageContent response = await _kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(Messages, _aiSettings, _kernel);
            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                Messages.Add(response);
            }
        }
        catch (Exception e)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(e, "Error getting chat completions.");
            }
            Messages.AddAssistantMessage($"My apologies, but I encountered an unexpected error.");
        }
        onMessageAdded();
    }
}
