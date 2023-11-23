using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.TemplateEngine;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");

string prompt = @"rewrite the text between triple hashtags into a business 
email using a professionally tone that is clear and concise. sign the email 
as AI assistant. ###{{$input}}###";

var function = kernel.CreateSemanticFunction(prompt, new OpenAIRequestSettings
{
    Temperature = 0.5,
    MaxTokens = 1000,
});

Console.WriteLine("Please enter the notes for your email:");
var input = Console.ReadLine();

var variables = new ContextVariables
{
    {
        "input", input
    }
};

var output = await kernel.RunAsync(function, variables);

Console.WriteLine(output.GetValue<string>());
