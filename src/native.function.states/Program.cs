using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Plugins;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");
kernel.ImportFunctions(new UnitedStatesPlugin(), "UnitedStatesPlugin");

var function = kernel.Functions.GetFunction("UnitedStatesPlugin", "GetPopulation");
ContextVariables variables = new ContextVariables
{
    { "input", "2020" }
};

var result = await kernel.RunAsync(
    variables,
    function
);

Console.WriteLine(result.GetValue<string>());
