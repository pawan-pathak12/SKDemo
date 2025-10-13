using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKDemo.Plugins;

// Setup Kernel
var kernel = CreateKernel();
var chatService = kernel.GetRequiredService<IChatCompletionService>();
var chatHistory = CreateChatHistory();

// Register plugins
kernel.Plugins.AddFromType<LightPlugin>();

// Main chat loop
await RunChatLoop(chatService, chatHistory, kernel);

// ========== METHODS ==========

static Kernel CreateKernel()
{
    return Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(
            modelId: "qwen2.5:7b",
            apiKey: "not-needed",
            httpClient: new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434/v1")
            })
        .Build();
}

static ChatHistory CreateChatHistory()
{
    var history = new ChatHistory();
    history.AddSystemMessage("You are a helpful Personal Assistant.");
    return history;
}


static async Task RunChatLoop(IChatCompletionService chatService, ChatHistory history, Kernel kernel)
{
    var settings = new OpenAIPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    while (true)
    {
        Console.Write("\nUser > ");
        var userInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userInput)) continue;

        history.AddUserMessage(userInput);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"\nAssistant >");
        var response = await GetAIResponse(chatService, history, settings, kernel);


        Console.WriteLine(response);

        history.AddAssistantMessage(response);
        Console.ResetColor();
    }
}

static async Task<string> GetAIResponse(IChatCompletionService chatService, ChatHistory history,
    OpenAIPromptExecutionSettings settings, Kernel kernel)
{
    var fullMessage = new System.Text.StringBuilder();

    var stream = chatService.GetStreamingChatMessageContentsAsync(history, settings, kernel);

    await foreach (var chunk in stream)
    {
        Console.Write(chunk.Content);
        fullMessage.Append(chunk.Content);
    }

    return fullMessage.ToString();
}