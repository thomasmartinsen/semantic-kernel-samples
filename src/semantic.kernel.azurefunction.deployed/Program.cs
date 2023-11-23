using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Functions.OpenAPI.OpenAI;
using Microsoft.SemanticKernel.Planners;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");

string weatherManifestUrl = "https://impnd02-plugins.azurewebsites.net/api/.well-known/ai-plugin.json";
await kernel.ImportOpenAIPluginFunctionsAsync("WeatherPlugin", new Uri(weatherManifestUrl));

var prompt = "how is the weather in copenhagen tomorrow?";
var planner = new StepwisePlanner(kernel);
var plan = planner.CreatePlan(prompt);
var resultPlanner = await kernel.RunAsync(plan);
Console.WriteLine(resultPlanner.GetValue<string>());