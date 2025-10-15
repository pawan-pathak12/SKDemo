using System.ComponentModel;
using Microsoft.SemanticKernel;
using SKDemo.Models;

namespace SKDemo.Plugins;

public class WeatherPlugin
{
    private readonly List<Weather> weathers = new()
    {
        new Weather { Id = 1, Location = "Kathmandu, Nepal", Temperature = 12.22 },
        new Weather { Id = 1, Location = "BirtaMode, Nepal", Temperature = 34.22 },
        new Weather { Id = 1, Location = "Illam, Nepal", Temperature = 12.22 },
        new Weather { Id = 1, Location = "Birtnagar, Nepal", Temperature = 18.22 },
        new Weather { Id = 1, Location = "Palpa, Nepal", Temperature = 10.22 },
        new Weather { Id = 1, Location = "Taplajung, Nepal", Temperature = 8.22 },
        new Weather { Id = 1, Location = "Chitwan, Nepal", Temperature = 4.22 },
        new Weather { Id = 1, Location = "Haldibari, Nepal", Temperature = 40.22 },
        new Weather { Id = 1, Location = "Jhapa, Nepal", Temperature = 42.22 }
    };

    #region Get Weather

    [KernelFunction]
    [Description("Gets a Location and Its temperature")]
    [return: Description("An array of Weather")]
    public Task<List<Weather>> GetWeatherAsync()
    {
        return Task.FromResult(weathers);
    }

    #endregion

    #region Update Temperature

    [KernelFunction("update_temperature")]
    [Description("Update the Temperature according to the Id")]
    [return: Description("The updated state of weather ; will return null if the location exits exist")]
    public Task<Weather> UpdateTemperatureAsync(int id, double temperature)
    {
        var weather = weathers.FirstOrDefault(x => x.Id == id);
        if (weathers is null) return Task.FromResult((Weather?)null);

        weather.Temperature = temperature;
        return Task.FromResult(weather);
    }

    #endregion
}