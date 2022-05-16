// See https://aka.ms/new-console-template for more information

using TclSharp.Core;
using TclSharp.Core.Extensions;


Console.WriteLine("TclSharp v0.0.3");

var script = new Script();

script.AddPutsCommand("");
script.AddPutsCommand("Hello", true);
script.AddPutsCommand(", world!");
script.AddPutsCommand("By by...");

script.AddSetCommand("test", "test-value");
script.AddSetCommand("test", "test-value2");
script.AddSetCommand("test");

script.AddPutsCommand("The test value is '$test'");

script.AddSetCommand("test1", "1: $test");
script.AddSetCommand("test2", "2: $test1 + $test");
script.AddSetCommand("test3", "3: $test2 + $test");

script.AddPutsCommand("test1 = $test1");
script.AddPutsCommand("test2 = $test2");
script.AddPutsCommand("test3 = $test3");

script.AddPutsCommand("test4 = ${test}3");
script.AddPutsCommand("test5 = ${a}test3");


var interpreter = new Interpreter(
    new ConsoleOutputWriter());

interpreter.AddPutsCommand();
interpreter.AddSetCommand();

var result = interpreter.Execute(script);

Console.WriteLine("---");
Console.WriteLine("{0}: {1}", result.Message, result.Data);
