using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SKDemo.Services;

public static class ChatService
{
    private static int count;

    public static async Task StartChatAsync(Kernel kernel)
    {
        var (history, isNewConversation) = FileService.LoadConversation();
        if (isNewConversation)
        {
            history.AddSystemMessage(@"You are a helpful AI assistant. 
            Keep responses clear, concise, and friendly.
            Answer questions directly without unnecessary details.
            Use simple language that's easy to understand.");
            Console.WriteLine(" New conversation started");
        }
        else
        {
            Console.WriteLine($" Loaded previous conversation ({history.Count} messages)");
        }

        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        //auto function  calling 
        OpenAIPromptExecutionSettings executionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        await ChatLoopAsync(history, chatService, executionSettings, kernel);
        FileService.SaveConversation(history);
    }

    private static async Task ChatLoopAsync(ChatHistory chatHistory,
        IChatCompletionService chatCompletionService,
        OpenAIPromptExecutionSettings executionSettings,
        Kernel kernel)
    {
        var count = 0;
        while (true)
        {
            Console.Write("\nUser > ");
            var userPrompt = Console.ReadLine()!;

            #region Input Validation

            if (count >= 3)
            {
                Console.WriteLine("You enter invalid input multiple time , exiting......");
                break;
            }

            if (string.IsNullOrWhiteSpace(userPrompt))
            {
                count++;
                Console.WriteLine("PLease Enter Valid Input");
                continue;
            }


            if (userPrompt.ToLower() == "exit" || userPrompt.ToLower() == "quit")
            {
                Console.WriteLine("exting ....");
                break;
            }

            #endregion

            //Add to history 
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