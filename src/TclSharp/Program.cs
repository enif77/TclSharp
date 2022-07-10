// See https://aka.ms/new-console-template for more information

using TclSharp.Core;
using TclSharp.Core.Extensions;


Console.WriteLine("TclSharp v0.0.10");

// TestInterpreter();
//
// TestTokenizer("  word1 $word2 \nword3;word4;;word5;   ");
// TestTokenizer("word3;word4");
// TestTokenizer("word1");
//
// TestParser(new StringSourceReader("puts hello-puts"));
// TestParser(new StringSourceReader("set msg hello-msg; puts $msg"));

const string scriptsFolderPath = "../../../../../scripts/";

// TestParser(new FileSourceReader(scriptsFolderPath + "empty.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello-world.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello-world-braces.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello-variable.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "puts-nonewline.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello-world-nl.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello-world-braces-nl.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "braces-nl-escapes.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello-world-braces-no-subst.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello-world-quotes-with-subst.tcl"));
// TestParser(new FileSourceReader(scriptsFolderPath + "hello-world-cmd-subst.tcl"));


//TestParser(new StringSourceReader("set v XXX; set vv aaa[set v]bbb;puts $vv"));
//TestParser(new StringSourceReader("set v XXX; puts [set v]"));
//TestParser(new StringSourceReader("set v XXX; puts ${v}x"));
// TestParser(new StringSourceReader("set v XXX; puts \"${v}\""));
// TestParser(new StringSourceReader("set v XXX; puts \"${v}xxx\""));
// TestParser(new StringSourceReader("set v XXX; puts \"xxx${v}\""));
// TestParser(new StringSourceReader("set v XXX; puts xxx\"\"xxx"));
// TestParser(new StringSourceReader("set v XXX; puts xxx${v}"));
// TestParser(new StringSourceReader("set v XXX; puts xxx${v}xxx"));
// TestParser(new StringSourceReader("set v XXX; puts ${v}xxx"));
// TestParser(new StringSourceReader("set v XXX; puts ${v}"));
//TestParser(new StringSourceReader("set v XXX; puts xxx[set v]"));
//TestParser(new StringSourceReader("set v XXX; puts [set v]xxx"));
//TestParser(new StringSourceReader("set v XXX; puts xxx[set v]xxx"));
//TestParser(new StringSourceReader("set v XXX; puts [set v]"));
//TestParser(new StringSourceReader("set v XXX; set vv YYY; puts [set v [set vv]]; puts $v"));

//TestParser(new StringSourceReader("puts \"[ set v \"xxx\"]\""));   // -> xxx
//TestParser(new StringSourceReader("puts \"[ set v \"x]x\" ]\""));  // -> x]x
//TestParser(new StringSourceReader("puts \"[ set v \"x\\[x\" ]\""));  // -> x[x
//TestParser(new StringSourceReader("puts \"[ set v \"x]\\\"x\" ]\""));  // -> x]"x
//TestParser(new StringSourceReader("puts \"[ set v \"x\\\"x\" ]\""));  // -> x"x
//TestParser(new StringSourceReader("set y [set x 0][incr x][incr x] ; puts $y"));  // -> 012

TestParser(new StringSourceReader("set greeting \"Hello World!\"; puts $greeting"));  // -> Hello World!
TestParser(new StringSourceReader("set greeting Hello; set who World; puts \"$greeting $who!\""));  // -> Hello World!
TestParser(new StringSourceReader("set greeting \"Hello World\"; puts \"$greeting!\""));  // -> Hello World!
TestParser(new StringSourceReader("puts $nosuchvar"));  // -> Error: no such vriale
TestParser(new StringSourceReader("puts $$"));  // -> $$
TestParser(new StringSourceReader("puts $="));  // -> $=
TestParser(new StringSourceReader("puts \"$$\""));  // -> $$


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

    interpreter.AddIncrCommand();
    interpreter.AddPutsCommand();
    interpreter.AddSetCommand();

    var result = interpreter.Interpret(script);
    
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


void TestParser(ISourceReader sourceReader)
{
    var interpreter = new Interpreter(
        new ConsoleOutputWriter());

    interpreter.AddIncrCommand();
    interpreter.AddPutsCommand();
    interpreter.AddSetCommand();
    
    
    var parser = new Parser(interpreter);

    var parseResult = parser.Parse(sourceReader);
    if (parseResult.IsSuccess == false)
    {
        Console.WriteLine("Parsing failed with the '{0}' error.", parseResult.Message);
        
        return;
    }
   
    
    var executeResult = interpreter.Interpret(parseResult.Data!);
    Console.WriteLine((executeResult.IsSuccess)
        ? "-> {0}."
        : "Executing failed with the '{0}' error.",
        executeResult.Message);
    
    Console.WriteLine("---");
}