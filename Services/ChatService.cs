using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SKDemo.Services;

public static class ChatService
{
    private static int count;

    #region Conversation Monitor

    private static void ManageContextWindow(ChatHistory chatHistory)
    {
        if (chatHistory.Count > 90)
        {
            Console.WriteLine($"📝 Conversation getting long ({chatHistory.Count} messages).");
            Console.Write("How many old messages to remove? (0 to keep all): ");

            if (int.TryParse(Console.ReadLine(), out var messageToRemove) && messageToRemove > 0)
            {
                //keep system message + recent messages 
                var systemMessage = chatHistory.FirstOrDefault(m => m.Role == AuthorRole.System);
                var recentMessage = chatHistory.TakeLast(chatHistory.Count - messageToRemove).ToList();
                chatHistory.Clear();
                if (systemMessage != null) chatHistory.Add(systemMessage);

                foreach (var message in recentMessage) chatHistory.Add(message);
                Console.WriteLine($"✅ Removed {messageToRemove} old messages. Now {chatHistory.Count} messages.");
            }
            else
            {
                Console.WriteLine("✅ Keeping all messages.");
            }
        }
    }

    #endregion

    #region Chat Setup

    public static async Task StartChatAsync(Kernel kernel)
    {
        var (history, isNewConversation) = FileService.LoadConversation();
        if (isNewConversation)
        {
            history.AddSystemMessage(@"You are a helpful AI personal assistant. 
            Keep responses clear, concise, and friendly.
            Answer questions directly without unnecessary details.
            Use simple language that's easy to understand.

            CRITICAL: For weather queries, ALWAYS call the actual WeatherRealTimePlugin.
            NEVER use cached responses from conversation history.
            ALWAYS fetch fresh data from the API.
            ");
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

            ManageContextWindow(chatHistory);
        }
    }

    #endregion
}