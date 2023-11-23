using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");

var mathPlugin = kernel.ImportFunctions(new MathPlugin.Math(), "MathPlugin");
var contextVariables = new ContextVariables
{
    ["number1"] = "12.34",
    ["number2"] = "56.78"
};

var result1 = await kernel.RunAsync(contextVariables, mathPlugin["Multiply"]);
Console.WriteLine(result1);

//var textPlugin = kernel.ImportPluginFromObject<StaticTextPlugin>();
//var variables = new ContextVariables("Today is: ");
//variables.Set("day", DateTimeOffset.Now.ToString("dddd", CultureInfo.CurrentCulture));

//FunctionResult result = await kernel.RunAsync(variables,
//    textPlugin["AppendDay"],
//    textPlugin["Uppercase"]);

//Console.WriteLine(result.GetValue<string>());

//var function = kernel.Functions.GetFunction("CalculatorPlugin", "TranslateMathProblem");
