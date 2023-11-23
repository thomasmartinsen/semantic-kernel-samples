using System.Net;
using System.Text.RegularExpressions;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

namespace AzureFunction;

public class WeatherPlugin
{
    private readonly string _endpoint;
    private readonly string _apiKey;
    private readonly string _model;

    public WeatherPlugin()
    {
#if DEBUG
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets("a22ed28e-227d-49a0-ac47-e603e9a70ec0")
            .Build();

        _apiKey = configuration["AzureOpenAI:ApiKey"];
        _endpoint = configuration["AzureOpenAI:Endpoint"];
        _model = configuration["AzureOpenAI:CompletionModel"];
#else
        _apiKey = Environment.GetEnvironmentVariable("AzureOpenAIApiKey", EnvironmentVariableTarget.Process);
        _endpoint = Environment.GetEnvironmentVariable("AzureOpenAIEndpoint", EnvironmentVariableTarget.Process);
        _model = Environment.GetEnvironmentVariable("AzureOpenAICompletionModel", EnvironmentVariableTarget.Process);
#endif
    }

    [Function("GetWeather")]
    [OpenApiOperation(operationId: "GetWeather", tags: new[] { "location" }, Description = "Get the weather forecast of a given location.")]
    [OpenApiParameter(name: "location", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The city and state, e.g. San Francisco, CA")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Weatherforecast in one line")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req, [FromQuery] string location)
    {
        var data = await GetWeatherFromLocationAsync(location);
        string prompt = $"Summarize the weather in one line: {data}";

        ChatCompletionsOptions chatCompletionsOptions = new();

        chatCompletionsOptions.Messages.Add(new(ChatRole.System, "You summarize the given weather data"));
        chatCompletionsOptions.Messages.Add(new(ChatRole.User, prompt));

        var openAIClient = new OpenAIClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
        ChatCompletions chatResponse = await openAIClient.GetChatCompletionsAsync(_model, chatCompletionsOptions);

        var result = chatResponse.Choices.First().Message.Content.Trim();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync(result);

        return response;
    }

    private async Task<string> GetWeatherFromLocationAsync(string? arguments)
    {
        string prompt = $"Get longitude and latitude for {arguments}";
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

