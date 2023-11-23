using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Plugins.Web;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");
var bing = SemanticKernelHelper.GetBingConnector("a22ed28e-227d-49a0-ac47-e603e9a70ec0");

var pluginsDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");
kernel.ImportFunctions(new TimePlugin());
kernel.ImportFunctions(new WebSearchEnginePlugin(bing), "BingPlugin");
kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "MailPlugin");

var planner = new StepwisePlanner(kernel);

var ask = @"Write a fun and relaxed mail to share a 
birthday message to my boss and include the highest 
temperature in celcius measured in denmark today. 
when mentioning the day today, remember to include the date and time.";

var plan = planner.CreatePlan(ask);
var result = await kernel.RunAsync(plan);

Console.WriteLine(result.GetValue<string>());
Console.ReadLine();

if (result.FunctionResults.First().TryGetMetadataValue("stepCount", out string? stepCount))
{
    Console.WriteLine("Steps Taken: " + stepCount);
}
if (result.FunctionResults.First().TryGetMetadataValue("functionCount", out string? functionCount))
{
    Console.WriteLine("Functions Used: " + functionCount);
}
if (result.FunctionResults.First().TryGetMetadataValue("iterations", out string? iterations))
{
    Console.WriteLine("Iterations: " + iterations);
}
