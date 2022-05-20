/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// An IOutputWriter implementation, that writes to System.Console.
/// </summary>
public class ConsoleOutputWriter : IOutputWriter
{
    public void Write(string format, params object[] arg)
    {
        if (arg.Length == 0)
        {
            format = format
                .Replace("{", "{{")
                .Replace("}", "}}");
        }

        Console.Write(format, arg);
    }


    public void WriteLine()
    {
        Console.WriteLine();
    }


    public void WriteLine(string format, params object[] arg)
    {
        if (arg.Length == 0)
        {
            format = format
                .Replace("{", "{{")
                .Replace("}", "}}");
        }
        
        Console.WriteLine(format, arg);
    }
}
