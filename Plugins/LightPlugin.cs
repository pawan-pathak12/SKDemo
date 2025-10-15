using System.ComponentModel;
using Microsoft.SemanticKernel;
using SKDemo.Models;

namespace SKDemo.Plugins;

public class LightPlugin
{
    // TEMPORARY IN-MEMORY STORAGE - Data resets on app restart
    // TODO: Implement file/database persistence for permanent storage
    //Mock data for the Lights 
    private readonly List<Light> lights = new()
    {
        new Light { Id = 1, Name = "Table Light", IsOn = false },
        new Light { Id = 2, Name = "Porch Light ", IsOn = false },
        new Light { Id = 3, Name = "Chandelier", IsOn = true }
    };

    [KernelFunction("get_lights")]
    [Description("Gets a list of lights and their current state")]
    [return: Description("An array of lights")]
    public Task<List<Light>> GetLightsAsync()
    {
        return Task.FromResult(lights);
    }

    [KernelFunction("change_state")]
    [Description("Changes the state of the light")]
    [return: Description("The updated state of the light; will return null if the light does not exist")]
    public Task<Light?> ChangeStateAsync([Description("This is identifer of light.")] int id,
        [Description("True if light is on ,false if light is off ")]
        bool isOn)
    {
        var light = lights.FirstOrDefault(a => a.Id == id);
        if (light is null) return Task.FromResult((Light?)null);

        light.IsOn = isOn;
        return Task.FromResult(light);
    }
}