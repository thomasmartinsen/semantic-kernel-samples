using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Models;

List<ChatMessage> _session = new();
ChatCompletionsOptions _completionOptions;
OpenAIClient _client;
bool _isSkillsEnabled = false;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("a22ed28e-227d-49a0-ac47-e603e9a70ec0")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string bingKey = configuration["Bing:ApiKey"];
string completionModel = configuration["AzureOpenAI:CompletionModel"];

_session.Add(
    new ChatMessage(
        ChatRole.System,
        $"You are an AI assistant.\n\nCurrent date and time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"));

_client = new OpenAIClient(
    new Uri(endpoint),
    new AzureKeyCredential(apiKey));

string prompt = "write a text of 100 words in danish about the world";
//string prompt = "how is the weather today in copenhagen, denmark";
Console.WriteLine(prompt);

//var result = await PromptAsync(prompt);
//Console.WriteLine(result);

await foreach (var response in PromptStreamingAsync(prompt))
{
    Console.WriteLine(response.Content);
    await Task.Delay(10);
}

async IAsyncEnumerable<TokenizedResponse> PromptStreamingAsync(string input, CancellationToken cancellationToken = default)
{
    _ = input ?? throw new ArgumentNullException(nameof(input));

    _session.Add(new ChatMessage(ChatRole.User, input));
    var options = GenerateChatCompletionsOptions();

    Azure.Response<StreamingChatCompletions> response =
        await _client.GetChatCompletionsStreamingAsync(
            deploymentOrModelName: completionModel,
            options,
            cancellationToken);

    using StreamingChatCompletions completions = response.Value;

    await foreach (StreamingChatChoice choice in completions.GetChoicesStreaming(cancellationToken))
    {
        await foreach (ChatMessage message in choice.GetMessageStreaming(cancellationToken))
        {
            yield return new TokenizedResponse(message.Content);
        }
    }
}

async Task<string> PromptAsync(string input, CancellationToken cancellationToken = default)
{
    _ = input ?? throw new ArgumentNullException(nameof(input));

    _session.Add(new ChatMessage(ChatRole.User, input));
    var options = GenerateChatCompletionsOptions();

    //WeatherSkill weatherSkill = new(
    //    _serviceOptions.Endpoint,
    //    _serviceOptions.Key,
    //    _serviceOptions.Models.Completion);

    //if (_isSkillsEnabled)
    //{
    //    options.Functions.Add(weatherSkill.GetDefinition());
    //}

    Azure.Response<ChatCompletions> response =
        await _client.GetChatCompletionsAsync(
            deploymentOrModelName: completionModel,
            options,
            cancellationToken);

    ChatCompletions completions = response.Value;
    string result = string.Empty;

    //if (_isSkillsEnabled &&
    //    response.Value.Choices[0].Message?.FunctionCall?.Name is string name)
    //{
    //    var arguments = response.Value.Choices[0].Message?.FunctionCall?.Arguments;

    //    switch (name)
    //    {
    //        case nameof(WeatherSkill):
    //            result = await weatherSkill.ExecuteAsync(arguments);
    //            break;
    //        default:
    //            result = null;
    //            break;
    //    }
    //}

    if (string.IsNullOrEmpty(result) && response.Value.Choices.Count > 0)
    {
        result = response.Value.Choices[0].Message.Content.Trim();
    }

    _session.Add(new ChatMessage(ChatRole.Assistant, result));

    return result;
}

ChatCompletionsOptions GenerateChatCompletionsOptions()
{
    return new ChatCompletionsOptions(_session)
    {
        Temperature = (float)0.7,
        MaxTokens = 500,
        NucleusSamplingFactor = (float)0.95,
        FrequencyPenalty = 0,
        PresencePenalty = 0,
    };
}