// See https://aka.ms/new-console-template for more information

using TclSharp.Core;
using TclSharp.Core.Extensions;


Console.WriteLine("TclSharp v0.0.1");

var script = new Script();

script.AddPutsCommand("");
script.AddPutsCommand("Hello", true);
script.AddPutsCommand(", world!");
script.AddPutsCommand("By by...");

script.AddSetCommand("test", "test-value");
script.AddSetCommand("test", "test-value2");
script.AddSetCommand("test");

script.AddPutsCommand("The test value is '$test'");

script.AddSetCommand("test2", "$test");
script.AddPutsCommand("$test2");


var interpreter = new Interpreter(
    new ConsoleOutputWriter());

interpreter.AddPutsCommand();
interpreter.AddSetCommand();

var result = interpreter.Execute(script);

Console.WriteLine("---");
Console.WriteLine("{0}: {1}", result.Message, result.Data);
