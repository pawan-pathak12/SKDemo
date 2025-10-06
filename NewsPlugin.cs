using System.ComponentModel;
using Microsoft.SemanticKernel;
using SimpleFeedReader;

namespace SKDemo;

public class NewsPlugin
{
    [KernelFunction("get_news")]
    [Description("Get news items for today's current date.")]
    [return: Description("A list of current news stories")]
    public async Task<string> GetNewsAsync(string category)
    {
        var reader = new FeedReader();

        var feed = await reader.RetrieveFeedAsync(
            $"https://rss.nytimes.com/services/xml/rss/nyt/{category}.xml"
        );

        var headlines = feed.Take(5)
            .Select(item => $"- {item.Title}")
            .ToList();

        return string.Join("\n", headlines);
    }
}