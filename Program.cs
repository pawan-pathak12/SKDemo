using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = Kernel.CreateBuilder();

// Services 

#region Connecting AI Serivce

var groqApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");

builder.AddOpenAIChatCompletion(
    "llama-3.1-8b-instant",
    apiKey: groqApiKey,
    endpoint: new Uri("https://api.groq.com/openai/v1")
);

#endregion

//PLugins 
var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();
var chatMessages = new ChatHistory();

while (true)
{
    Console.WriteLine("Prompt:");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    var completion = chatService.GetStreamingChatMessageContentsAsync(chatMessages, kernel: kernel);

    var fullMessage = "";

    await foreach (var context in completion)
    {
        Console.Write(context.Content);
        fullMessage = context.Content;
    }

    chatMessages.AddAssistantMessage(fullMessage);
    Console.WriteLine();
}