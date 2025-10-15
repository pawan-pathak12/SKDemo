using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SKDemo.Services;

public static class FileService
{
    private static readonly string HistoryFile = "Data/chat_history.json";

    public static void SaveConversation(ChatHistory chatHistory)
    {
        var json = JsonSerializer.Serialize(chatHistory, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(HistoryFile, json);
        Console.WriteLine($"Conversation saved {chatHistory.Count} messages. ");
    }

    public static (ChatHistory history, bool isNew) LoadConversation()
    {
        //Create data folder first - before checking file 
        if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");

        if (!File.Exists(HistoryFile)) return (new ChatHistory(), true);

        var json = File.ReadAllText(HistoryFile);
        var history = JsonSerializer.Deserialize<ChatHistory>(json) ?? new ChatHistory();
        return (history, false); //Existing conversation 
    }

    public static void SaveToFile<T>(string filePath, T data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public static T LoadFromFile<T>(string filePath)
    {
        if (File.Exists(filePath)) return default;

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }
}