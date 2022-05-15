// See https://aka.ms/new-console-template for more information

using TclSharp.Core;
using TclSharp.Core.Extensions;


Console.WriteLine("TclSharp v0.0.1");

var script = new Script();

script.AddPutsCommand("");
script.AddPutsCommand("Hello", true);
script.AddPutsCommand(", world!");
script.AddPutsCommand("By by...");

var interpreter = new Interpreter(
    new ConsoleOutputWriter());

interpreter.AddPutsCommand();

var result = interpreter.Execute(script);

Console.WriteLine("---");
Console.WriteLine("{0}: {1}", result.Message, result.Data);
