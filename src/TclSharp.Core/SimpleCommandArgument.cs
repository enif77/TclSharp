/* TclSharp - (C) 2022 Premysl Fara */

using TclSharp.Core.Results;

namespace TclSharp.Core;


/// <summary>
/// A simple command argument.
/// </summary>
public class SimpleCommandArgument : ICommandArgument
{
    public string Value { get; private set; }
    
    
    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="value">A value.</param>
    public SimpleCommandArgument(string value)
    {
        Value = value;
    }
    
    
    public void AddValue(ICommandArgumentValue value)
    {
        Value = value.Value;
    }

    
    public IResult<string> Interpret(IInterpreter interpreter)
    {
        return Result<string>.Ok(Value, null);
    }
}
