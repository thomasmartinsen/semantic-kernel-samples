//using Microsoft.Extensions.Configuration;
//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.Orchestration;

//var configuration = new ConfigurationBuilder()
//    .AddUserSecrets("02216570-f0b3-4d96-8e4b-efd984924be9")
//    .Build();

//string apiKey = configuration["AzureOpenAI:ApiKey"];
//string deploymentName = configuration["AzureOpenAI:DeploymentName"];
//string endpoint = configuration["AzureOpenAI:Endpoint"];

//var kernelBuilder = new KernelBuilder().
//    WithAzureOpenAIChatCompletionService(deploymentName, endpoint, apiKey);
//var kernel = kernelBuilder.Build();

//var pluginsDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");
//kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "MailPlugin");
//var mailFunction = kernel.Functions.GetFunction("MailPlugin", "WriteBusinessMail");

//Console.WriteLine("please enter your text:");
//var input = Console.ReadLine();
//var variables = new ContextVariables
//{
//    {
//        "input", input
//    }
//};

//var output = await kernel.RunAsync(variables, mailFunction);
//Console.WriteLine(output.GetValue<string>());
