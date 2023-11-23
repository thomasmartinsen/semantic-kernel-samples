using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Azure;
using Azure.AI.OpenAI;
using Models;
using Newtonsoft.Json;

namespace Plugins;

public class WeatherPlugin
{
    private readonly string _endpoint;
    private readonly string _apiKey;
    private readonly string _model;

    public WeatherPlugin(string endpoint, string apiKey, string model)
    {
        _apiKey = apiKey;
        _endpoint = endpoint;
        _model = model;
    }

    public FunctionDefinition GetDefinition()
        => new()
        {
            Name = nameof(WeatherPlugin),
            Description = "Get the weather forecast of a given location.",
            Parameters = BinaryData.FromObjectAsJson(
            new
            {
                Type = "object",
                Properties = new
                {
                    Location = new
                    {
                        Type = "string",
                        Description = "The city and state, e.g. San Francisco, CA",
                    }
                },
                Required = new[] { "location" },
            },
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
        };

    public async Task<string> ExecuteAsync(string? arguments)
    {
        var data = await RunAsync(arguments);
        string prompt = $"Summarize the weather in one line: {data}";

        ChatCompletionsOptions chatCompletionsOptions = new();

        chatCompletionsOptions.Messages.Add(new(ChatRole.System, "You summarize the given weather data"));
        chatCompletionsOptions.Messages.Add(new(ChatRole.User, prompt));

        var openAIClient = new OpenAIClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
        ChatCompletions response = await openAIClient.GetChatCompletionsAsync(_model, chatCompletionsOptions);

        return response.Choices.First().Message.Content.Trim();
    }

    public async Task<string> RunAsync(string? arguments)
    {
        GetCurrentWeatherArgs weatherArgs = JsonConvert.DeserializeObject<GetCurrentWeatherArgs>(arguments);

        string prompt = $"Get longitude and latitude for {weatherArgs.Location}";
        ChatCompletionsOptions options = new();
        options.Messages.Add(new(ChatRole.System, "You convert location to longitude latitude"));
        options.Messages.Add(new(ChatRole.User, prompt));

        var oaiclient = new OpenAIClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
        ChatCompletions rsponse = await oaiclient.GetChatCompletionsAsync(_model, options);
        string completionText = rsponse.Choices.First().Message.Content;

        Regex regex = new Regex(@"(-?\d+\.\d+)");
        MatchCollection matches = regex.Matches(completionText);

        if (matches.Count < 2) return "Sorry, I couldn't generate a response.";

        string latitude = matches[0].Value;
        string longitude = matches[1].Value;
        var weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";
        
        var weather = await new HttpClient().GetStringAsync(weatherUrl);

        return ($"Longitude: {longitude}, Latitude: {latitude}, Weather: {weather}");
    }
}
