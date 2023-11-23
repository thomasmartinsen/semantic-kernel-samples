﻿/// <summary>
/// This sample demonstrates how to use a built-in plugin to ask a question
/// and summarize the answer using another built-in plugin.
/// </summary>

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

var kernel = SemanticKernelHelper.GetChatCompletionKernel("a22ed28e-227d-49a0-ac47-e603e9a70ec0");

var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "AskPlugin");
kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "SummarizePlugin");

var askFunction = kernel.Functions.GetFunction("AskPlugin", "Ask");
var askVariables = new ContextVariables()
{
    {
        "input", @"when is 'hello world' used. use 200 words to answer the question."
    }
};

var askOutput = await kernel.RunAsync(askFunction, askVariables);
var askResult = askOutput.GetValue<string>();
Console.WriteLine(askResult);

Console.WriteLine("----------------------------");

var summarizeFunction = kernel.Functions.GetFunction("SummarizePlugin", "Summarize");
var summarizeVariables = new ContextVariables()
{
    {
        "input", askResult
    }
};

var summarizeOutput = await kernel.RunAsync(summarizeFunction, summarizeVariables);
var summarizeResult = summarizeOutput.GetValue<string>();
Console.WriteLine(summarizeResult);
