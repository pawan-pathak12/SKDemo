using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SKDemo.Services;

// Setup Kernel
var kernel = CreateKernel();
var chatService = kernel.GetRequiredService<IChatCompletionService>();
var chathistory = new ChatHistory();
CreateKernel();
var chatservice = new ChatService();

await chatservice.StartChatAsync(kernel, chatService, chathistory);

#region Methods

static Kernel CreateKernel()
{
    return Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(
            "qwen2.5:7b",
            "not-needed",
            httpClient: new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434/v1")
            })
        .Build();
}

#endregion