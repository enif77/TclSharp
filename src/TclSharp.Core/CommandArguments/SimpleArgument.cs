/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.CommandArguments;


/// <summary>
/// A command argument, that does no value processing or substitution.
/// </summary>
public class SimpleArgument : ICommandArgument
{
    public string Value { get; }
    
    
    public SimpleArgument(string value)
    {
        Value = value;
    }
}
