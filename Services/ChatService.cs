using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SKDemo.Services;

internal class ChatService
{
    public static async Task ChatLoopAsync(ChatHistory history, IChatCompletionService chatCompletionService,
        Kernel kernel)
    {
    }

    public async Task StartChatAsync(Kernel kernel, IChatCompletionService chatCompletionService,
        ChatHistory chatHistory)
    {
        Console.WriteLine("Hey , I am your Agent ");
        //auto function calling
        OpenAIPromptExecutionSettings executionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        #region Chatloop...... (after creating chatoop method code will be transfer)

        while (true)
        {
            Console.WriteLine();
            Console.Write("User > ");
            var userMessage = Console.ReadLine()!;

            #region Input Validation

            if (userMessage.ToLower() == "exit" || userMessage.ToLower() == "quit")
            {
                Console.WriteLine("Exiting......");
                break;
            }

            #endregion

            #region code of agent after code of Agent Later will transfer to Seperate class

            chatHistory.AddUserMessage(userMessage);
            var resposne = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
            chatHistory.AddAssistantMessage(resposne.Content);
            //     return resposne.Content ?? "No Response from Personal Agent";

            #endregion

            Console.WriteLine($"AI > {resposne}");
            chatHistory.AddAssistantMessage(userMessage);
        }

        #endregion
    }
    /*private readonly IChatCompletionService ChatCompletionService;
    private readonly ChatHistory chatHistory;
    private Kernel kernel;*/

    #region code of Agent Later will transfer to Seperate class

    /*public ChatService()
    {
        _ChatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        _chatHistory = new ChatHistory();
        _chatHistory.AddSystemMessage(@"You are helpful assistant, do the task accroding to user
            prompt andtry to answer each question in short way as more possible , make
            eavey thing simple , short and sweet  ");
    }*/

    #endregion
}

public class RefactorChatSerivce
/*
 * What to do here : Seperate code of Chatloop , StrtChat and keep Agent code in StrtChatAsync -sperate it after seperating code of Chatloop and strtchatAsync
 */
{
    public async Task StartChatAsync(Kernel kernel)
    {
        var history = new ChatHistory();
        history.AddSystemMessage("You are Asistant of user , you have to answer each question i short and sweet way ");

        kernel.GetRequiredService<IChatCompletionService>();

        //auto calling function 
    }

    public async Task ChatLoopAsync(ChatHistory chatHistory, IChatCompletionService chatCompletionService)
    {
        while (true)
        {
            Console.WriteLine("/n User > ");
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
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("/n Asistant > ");
            Console.WriteLine(response);
            Console.ResetColor();
            chatHistory.AddAssistantMessage(userPrompt);
        }
    }
}