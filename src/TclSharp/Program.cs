﻿// See https://aka.ms/new-console-template for more information

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

Console.WriteLine("---");

var tokenizer = new Tokenizer(new StringSourceReader("  word1 $word2 \nword3;word4;;word5;   "));
//var tokenizer = new Tokenizer(new StringSourceReader("word3;word4"));
//var tokenizer = new Tokenizer(new StringSourceReader("word1"));

var token = tokenizer.NextToken();
while (token.Code != TokenCode.Eof)
{
    switch (token.Code)
    {
        case TokenCode.Word:
            Console.WriteLine("word: {0}", token.StringValue);
            break;
        
        case TokenCode.CommandSeparator:
            Console.WriteLine("command separator");
            break;
        
        case TokenCode.Unknown:
            Console.WriteLine("UNKNOWN");
            break;
        
        default:
            Console.WriteLine("UNIMPLEMENTED token {0} returned!", token.Code);
            break;
    }

    token = tokenizer.NextToken();
}

Console.WriteLine(token.Code == TokenCode.Eof
    ? "EOF"
    : "cEOF expected!");

Console.WriteLine("---");

Console.WriteLine("DONE");
