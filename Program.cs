using Microsoft.SemanticKernel;
using SKDemo.Services;

// Setup Kernel
var kernel = CreateKernel();
CreateKernel();


await ChatService.StartChatAsync(kernel);

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