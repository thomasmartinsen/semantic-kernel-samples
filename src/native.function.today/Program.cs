using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");

var time = kernel.ImportFunctions(new TimePlugin());
var result = await kernel.RunAsync(time["Today"]);

Console.WriteLine(result);