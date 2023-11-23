/// <summary>
/// This sample demonstrates how to use the Semantic Kernel to 
/// create a custom plugin that uses the OpenAI get a users intent.
/// </summary>

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");

string prompt = @"Bot: How can I help you?
User: {{$input}}

---------------------------------------------

The intent of the user in 5 words or less: ";

OpenAIRequestSettings requestSettings = new()
{
    ExtensionData = {
        {"MaxTokens", 500},
        {"Temperature", 0.0},
        {"TopP", 0.0},
        {"PresencePenalty", 0.0},
        {"FrequencyPenalty", 0.0}
    }
};

var getIntentFunction = kernel.CreateSemanticFunction(prompt, requestSettings, "GetIntent");

var result = await kernel.RunAsync(
    "I want to send an email to the marketing team celebrating their recent milestone.",
    getIntentFunction
);

Console.WriteLine(result);