using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace SKDemo.Plugins
{
    public class WeatherRealTimePlugin
    {
        private readonly string _apiKey;

        public WeatherRealTimePlugin()
        {
            _apiKey = SetupWeatherApiKey();
        }

        [KernelFunction, Description("Get current weather for a city")]
        public async Task<string> GetWeatherAsync([Description("The city ")] string city)
        {
            using var client = new HttpClient();
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            //parse json and return formatted weather 
            return FormatWeatherResponse(json);
        }

        private static string FormatWeatherResponse(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var city = root.GetProperty("name").GetString();
                var temp = root.GetProperty("main").GetProperty("temp").GetDouble();
                var desc = root.GetProperty("weather")[0].GetProperty("description").GetString();

                return $"🌤️ {city}: {temp}°C, {desc}";
            }
            catch
            {
                return "Sorry, couldn't fetch weather data.";
            }
        }

        private static string SetupWeatherApiKey()
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false)
                 .Build();
            return configuration["OpenWeatherMap:ApiKey"];
        }
    }
}