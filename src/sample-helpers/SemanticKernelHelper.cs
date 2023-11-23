using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

public static class SemanticKernelHelper
{
    public static BingConnector GetBingConnector(string secret)
    {
        _ = secret ?? throw new ArgumentNullException(nameof(secret));

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(secret)
            .Build();

        string bingKey = configuration["Bing:ApiKey"];
        var bingConnector = new BingConnector(bingKey);
        return bingConnector;
    }

    public static IKernel GetChatCompletionKernel(string secret)
    {
        _ = secret ?? throw new ArgumentNullException(nameof(secret));

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(secret)
            .Build();

        string apiKey = configuration["AzureOpenAI:ApiKey"];
        string deploymentName = configuration["AzureOpenAI:DeploymentName"];
        string endpoint = configuration["AzureOpenAI:Endpoint"];

        var kernelBuilder = new KernelBuilder()
            .WithAzureOpenAIChatCompletionService(deploymentName, endpoint, apiKey);

        var kernel = kernelBuilder.Build();
        return kernel;
    }

    public static IKernel GetTextCompletionKernel(string secret)
    {
        _ = secret ?? throw new ArgumentNullException(nameof(secret));

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(secret)
            .Build();

        string apiKey = configuration["AzureOpenAI:ApiKey"];
        string deploymentName = configuration["AzureOpenAI:DeploymentName"];
        string endpoint = configuration["AzureOpenAI:Endpoint"];

        var kernelBuilder = new KernelBuilder()
            .WithAzureTextCompletionService(deploymentName, endpoint, apiKey);

        var kernel = kernelBuilder.Build();
        return kernel;
    }
}

// https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/kernel/adding-services
// .WithAzureTextCompletionService()
// .WithAzureOpenAIChatCompletionService()
// .WithOpenAITextCompletionService()
// .WithOpenAIChatCompletionService()
// .WithCompletionService()
