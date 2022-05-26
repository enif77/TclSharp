/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.CommandArgumentValues;


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


    public string Interpret(IInterpreter interpreter)
        => Value;
}