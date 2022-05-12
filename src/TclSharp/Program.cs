// See https://aka.ms/new-console-template for more information

using TclSharp.Core;
using TclSharp.Core.Extensions;


Console.WriteLine("TclSharp v0.0.1");

var interpreter = new Interpreter(
    new ConsoleOutputWriter());

interpreter.AddPutsCommand("");
interpreter.AddPutsCommand("Hello", true);
interpreter.AddPutsCommand(", world!");
interpreter.AddPutsCommand("By by...");

var result = interpreter.Execute();

Console.WriteLine("---");
Console.WriteLine("{0}: {1}", result.Message, result.Data);
