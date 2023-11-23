using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");

string prompt = @"write a business email that congratulates a colleague on a promotion.
use a professionally tone that is clear and concise. sign the email as AI assistant.";

var function = kernel.CreateSemanticFunction(prompt, new OpenAIRequestSettings
{
    Temperature = 0.5,
    MaxTokens = 1000,
});

var output = await kernel.RunAsync(function);

Console.WriteLine(output.GetValue<string>());
