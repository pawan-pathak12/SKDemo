using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SKDemo.Services;

public static class ChatService
{
    public static async Task StartChatAsync(Kernel kernel)
    {
        var history = new ChatHistory();
        history.AddSystemMessage(@"You are a helpful AI assistant. 
        Keep responses clear, concise, and friendly.
        Answer questions directly without unnecessary details.
        Use simple language that's easy to understand.");

        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        OpenAIPromptExecutionSettings executionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        await ChatLoopAsync(history, chatService, executionSettings, kernel);
    }

    private static async Task ChatLoopAsync(ChatHistory chatHistory,
        IChatCompletionService chatCompletionService,
        OpenAIPromptExecutionSettings executionSettings,
        Kernel kernel)
    {
        while (true)
        {
            Console.Write("\nUser > ");
            var userPrompt = Console.ReadLine()!;

            #region Input Validation

            if (userPrompt.ToLower() == "exit" || userPrompt.ToLower() == "quit")
            {
                Console.WriteLine("Exiting...");
                break;
            }

            #endregion

            chatHistory.AddUserMessage(userPrompt);

            var response = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory, executionSettings, kernel);

            #region Display Assistant Response

            Console.Write("\nAssistant > ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(response);
            Console.ResetColor();

            #endregion

            chatHistory.AddAssistantMessage(response.Content);
        }
    }
}