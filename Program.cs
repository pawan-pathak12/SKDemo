using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKDemo;

var builder = Kernel.CreateBuilder();

// Services 

#region Connection to Ollama

builder.AddOpenAIChatCompletion(
    "qwen2.5:7b",
    "not-needed",
    httpClient: new HttpClient
    {
        BaseAddress = new Uri("http://localhost:11434/v1")
    });

#endregion

//PLugins 
builder.Plugins.AddFromType<NewsPlugin>();

var kernel = builder.Build();


var chatService = kernel.GetRequiredService<IChatCompletionService>();
var chatMessages = new ChatHistory();

#region Chat loop

while (true)
{
    Console.WriteLine();
    Console.Write("Prompt:");
    //Console.WriteLine();
    chatMessages.AddUserMessage(Console.ReadLine()!);

    var completion = chatService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        },
        kernel);

    var fullMessage = "";

    await foreach (var context in completion)
    {
        Console.Write(context.Content);
        fullMessage = context.Content;
    }

    chatMessages.AddAssistantMessage(fullMessage);
    Console.WriteLine();
}

#endregion