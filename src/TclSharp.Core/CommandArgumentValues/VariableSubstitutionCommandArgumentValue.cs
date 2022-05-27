/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.CommandArgumentValues;

using TclSharp.Core.Results;


/// <summary>
/// A value representing a variable name, that will be replaced by its value.
/// </summary>
public class VariableSubstitutionCommandArgumentValue : ICommandArgumentValue
{
    public string Value { get; }


    public VariableSubstitutionCommandArgumentValue(string variableName)
    {
        Value = variableName;
    }


    public IResult<string> Interpret(IInterpreter interpreter)
        => Result<string>.Ok(interpreter.GetVariableValue(Value), null);
}