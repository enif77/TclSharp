/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.CommandArgumentValues;

using TclSharp.Core.Results;


/// <summary>
/// A simple textual value. Does no evaluation or interpretation of its value.
/// </summary>
public class TextCommandArgumentValue : ICommandArgumentValue
{
    public string Value { get; }


    public TextCommandArgumentValue(string value)
    {
        Value = value;
    }


    public IResult<string> Interpret(IInterpreter interpreter)
        => Result<string>.Ok(Value, null);
}