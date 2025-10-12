using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKDemo;
using SKDemo.Plugins;

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

OpenAIPromptExecutionSettings promptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var kernel = builder.Build();

kernel.Plugins.AddFromType<LightPlugin>("Lights");

var lights = await kernel.Plugins
    .GetFunction("Lights", "get_lights")
    .InvokeAsync(kernel);
Console.WriteLine(lights);

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