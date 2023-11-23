using System.ComponentModel;
using System.Net.Http.Json;
using Microsoft.SemanticKernel;
using Models;

namespace Plugins;

public sealed class UnitedStatesPlugin
{
    [SKFunction, Description("Get the United States population for a specific year")]
    public async Task<string> GetPopulation([Description("The year")] int year)
    {
        string request = "https://datausa.io/api/data?drilldowns=Nation&measures=Population";
        HttpClient client = new HttpClient();
        var result = await client.GetFromJsonAsync<UnitedStatesResult>(request);
        var populationData = result.data.FirstOrDefault(x => x.Year == year.ToString());
        return $"The population number in the United States in {year} was {populationData.Population}";
    }
}
