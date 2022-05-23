// See https://aka.ms/new-console-template for more information

using TclSharp.Core;
using TclSharp.Core.Extensions;


Console.WriteLine("TclSharp v0.0.6");

TestInterpreter();

TestTokenizer("  word1 $word2 \nword3;word4;;word5;   ");
TestTokenizer("word3;word4");
TestTokenizer("word1");

TestParser("puts hello");
TestParser("set msg hello; puts $msg");

const string scriptsFolderPath = "../../../../../scripts/";

TestParser(File.ReadAllText(scriptsFolderPath + "empty.tcl"));
TestParser(File.ReadAllText(scriptsFolderPath + "hello.tcl"));
TestParser(File.ReadAllText(scriptsFolderPath + "hello-world.tcl"));
TestParser(File.ReadAllText(scriptsFolderPath + "hello-world-braces.tcl"));
TestParser(File.ReadAllText(scriptsFolderPath + "hello-variable.tcl"));
TestParser(File.ReadAllText(scriptsFolderPath + "puts-nonewline.tcl"));
TestParser(File.ReadAllText(scriptsFolderPath + "hello-world-nl.tcl"));
TestParser(File.ReadAllText(scriptsFolderPath + "hello-world-braces-nl.tcl"));
TestParser(File.ReadAllText(scriptsFolderPath + "braces-nl-escapes.tcl"));


Console.WriteLine("DONE");


void TestInterpreter()
{
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
    
    Console.WriteLine("...");
    
    Console.WriteLine("{0}: {1}", result.Message, result.Data);
    
    Console.WriteLine("---");
}


void TestTokenizer(string source)
{
    var tokenizer = new Tokenizer(new StringSourceReader(source));

    var token = tokenizer.NextToken();
    while (token.Code != TokenCode.EoF)
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

    Console.WriteLine(token.Code == TokenCode.EoF
        ? "EOF"
        : "EOF expected!");

    Console.WriteLine("---");
}


void TestParser(string source)
{
    var parser = new Parser();

    var parseResult = parser.Parse(new StringSourceReader(source));
    if (parseResult.IsSuccess == false)
    {
        Console.WriteLine("Parsing of '{0}' failed with the '{1}' error.", source, parseResult.Message);
        
        return;
    }

    Console.WriteLine("Parsing of '{0}' {1}.", source, parseResult.Message);
    
    Console.WriteLine("...");
    
    var interpreter = new Interpreter(
        new ConsoleOutputWriter());

    interpreter.AddPutsCommand();
    interpreter.AddSetCommand();

    var executeResult = interpreter.Execute(parseResult.Data!);
    Console.WriteLine((executeResult.IsSuccess)
        ? "Execution succeeded with message '{0}'."
        : "Executing failed with the '{0}' error.",
        executeResult.Message);
    
    Console.WriteLine("---");
}