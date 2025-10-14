using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SKDemo.Services;

public static class RefactorChatSerivce
/*
 * What to do here : Seperate code of Chatloop , StrtChat and keep Agent code in StrtChatAsync -sperate it after seperating code of Chatloop and strtchatAsync
 */
{
    public static async Task StartChatAsync(Kernel kernel)
    {
        var history = new ChatHistory();
        history.AddSystemMessage("You are Asistant of user , you have to answer each question i short and sweet way ");

        var chatSerice = kernel.GetRequiredService<IChatCompletionService>();

        //auto calling function 
        OpenAIPromptExecutionSettings executionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        await ChatLoopAsync(history, chatSerice);
    }

    public static async Task ChatLoopAsync(ChatHistory chatHistory, IChatCompletionService chatCompletionService)
    {
        while (true)
        {
            Console.Write("\nUser > ");
            var userPrompt = Console.ReadLine()!;

            #region Input Validation

            if (userPrompt.ToLower() == "exit" || userPrompt.ToLower() == "quit")
            {
                Console.WriteLine("exting.....");
                break;
            }

            #endregion

            chatHistory.AddUserMessage(userPrompt);

            var response = await chatCompletionService.GetChatMessageContentAsync(userPrompt);
            Console.Write("\n Asistant > ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(response);
            Console.ResetColor();
            chatHistory.AddAssistantMessage(userPrompt);
        }
    }
}