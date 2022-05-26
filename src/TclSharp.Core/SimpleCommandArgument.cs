/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A simple command argument.
/// </summary>
public class SimpleCommandArgument : ICommandArgument
{
    public string Value { get; private set; }
    
    
    public void AddValue(ICommandArgumentValue value)
    {
        Value = value.Value;
    }


    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="value">A value.</param>
    public SimpleCommandArgument(string value)
    {
        Value = value;
    }
}
