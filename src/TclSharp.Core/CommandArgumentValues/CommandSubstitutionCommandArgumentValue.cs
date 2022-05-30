/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.CommandArgumentValues;

using TclSharp.Core.Results;


/// <summary>
/// A value representing one or more commands. It must be interpreted to get its value.
/// </summary>
public class CommandSubstitutionCommandArgumentValue : ICommandArgumentValue
{
    public string Value { get; }


    public CommandSubstitutionCommandArgumentValue(string command)
    {
        Value = command;
    }


    public IResult<string> Interpret(IInterpreter interpreter)
    {
        var parser = new Parser(interpreter);

        var parseResult = parser.Parse(new StringSourceReader(Value));
        
        return parseResult.IsSuccess
            ? interpreter.Execute(parseResult.Data!)
            : Result<string>.Error(parseResult.Message);
    }
}