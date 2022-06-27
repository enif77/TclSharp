/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Commands;

using TclSharp.Core.Results;


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

        var getVariableNameResult = GetVariableName(scriptCommand.Arguments);
        if (getVariableNameResult.IsSuccess == false)
        {
            return getVariableNameResult;
        }

        var variableName = getVariableNameResult.Data!;
        

        var getValueResult = GetValue(scriptCommand.Arguments);
        if (getValueResult.IsSuccess == false)
        {
            return getValueResult;
        }

        var value = getValueResult.Data;        
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
    
    
    private IResult<string> GetVariableName(IList<ICommandArgument> arguments)
    {
        // The first argument is always a command name. So we expect at least two arguments.
        if (arguments.Count < 2)
        {
            return Result<string>.Error("At least one argument, a variable name, expected.");
        }

        var variableNameArgument = arguments[1];
        var interpretArgumentValueResult = _interpreter.Interpret(variableNameArgument);
        if (interpretArgumentValueResult.IsSuccess == false)
        {
            return Result<string>.Error(variableNameArgument.Value, interpretArgumentValueResult.Message);
        }
        
        var variableName = interpretArgumentValueResult.Data;
        
        return string.IsNullOrWhiteSpace(variableName)
            ? Result<string>.Error("A variable name expected.")
            : Result<string>.Ok(variableName, null);
    }


    private IResult<string> GetValue(IList<ICommandArgument> arguments)
    {
        string? value = null;
        
        if (arguments.Count > 2)
        {
            var valueArgument = arguments[2];
            var interpretArgumentValueResult = _interpreter.Interpret(valueArgument);
            if (interpretArgumentValueResult.IsSuccess == false)
            {
                return Result<string>.Error(valueArgument.Value, interpretArgumentValueResult.Message);
            }

            value = interpretArgumentValueResult.Data ?? string.Empty;
        }

        return Result<string>.Ok(value, null);
    }
}