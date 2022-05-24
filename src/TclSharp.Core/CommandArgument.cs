/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A command argument.
/// </summary>
public class CommandArgument : ICommandArgument
{
    public string Value { get; }
    
    
    public CommandArgument(string value)
    {
        Value = value;
    }
}
