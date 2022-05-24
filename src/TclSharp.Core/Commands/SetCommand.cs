/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Commands;


/// <summary>
/// Read and write variables.
/// https://www.tcl.tk/man/tcl/TclCmd/set.html
/// set variable-name [ value ] 
/// </summary>
public class SetCommand : ICommandImplementation
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <exception cref="ArgumentNullException">Thrown, when the interpreter parameter is null.</exception>
    public SetCommand(IInterpreter interpreter)
    {
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
    }
    

    public IResult<string> Execute(IScriptCommand scriptCommand)
    {
        if (scriptCommand == null) throw new ArgumentNullException(nameof(scriptCommand));

        if (scriptCommand.Arguments.Count == 1)
        {
            return Result<string>.Error("At least one argument, a variable name, expected.");
        }

        var firstArgument = scriptCommand.Arguments[1];
        var getFirstArgumentValueResult = _interpreter.InterpretCommandArgument(firstArgument);
        if (getFirstArgumentValueResult.IsSuccess == false)
        {
            return Result<string>.Error(firstArgument.Value, getFirstArgumentValueResult.Message);
        }
        
        var variableName = getFirstArgumentValueResult.Data;
        if (string.IsNullOrWhiteSpace(variableName))
        {
            return Result<string>.Error("A variable name expected.");
        }

        string? value = null;

        if (scriptCommand.Arguments.Count > 2)
        {
            var secondArgument = scriptCommand.Arguments[2];
            var getSecondArgumentValueResult = _interpreter.InterpretCommandArgument(secondArgument);
            if (getSecondArgumentValueResult.IsSuccess == false)
            {
                return Result<string>.Error(secondArgument.Value, getSecondArgumentValueResult.Message);
            }

            value = getSecondArgumentValueResult.Data ?? string.Empty;
        }

        if (value != null)
        {
            _interpreter.SetVariableValue(variableName, value);
        }

        return Result<string>.Ok(
            _interpreter.HasVariable(variableName)
                ? _interpreter.GetVariableValue(variableName)
                : string.Empty,
    null);
    }


    private readonly IInterpreter _interpreter;
}