/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// An IOutputWriter implementation, that writes to System.Console.
/// </summary>
public class ConsoleOutputWriter : IOutputWriter
{
    public void Write(string format, params object[] arg)
    {
        Console.Write(
            (arg.Length == 0)
                ? EscapeBraces(format)
                : format,
            arg);
    }


    public void WriteLine()
    {
        Console.WriteLine();
    }


    public void WriteLine(string format, params object[] arg)
    {
        Console.WriteLine(
            (arg.Length == 0)
                ? EscapeBraces(format)
                : format,
            arg);
    }
    
    
    private static string EscapeBraces(string format)
        => format
            .Replace("{", "{{")
            .Replace("}", "}}");
}
