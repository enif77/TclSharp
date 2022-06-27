/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Commands;

using System.Globalization;

using TclSharp.Core.Results;


/// <summary>
/// Increment the value of a variable.
/// https://www.tcl-lang.org/man/tcl/TclCmd/incr.htm
/// incr variable-name [ increment ] 
/// </summary>
public class IncrCommand : ICommandImplementation
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <exception cref="ArgumentNullException">Thrown, when the interpreter parameter is null.</exception>
    public IncrCommand(IInterpreter interpreter)
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

        
        var getOriginalVariableValueResult = GetVariableValue(variableName);
        if (getOriginalVariableValueResult.IsSuccess == false)
        {
            return Result<string>.Error(getOriginalVariableValueResult.Message);
        }

        var originalVariableValue = getOriginalVariableValueResult.Data;


        var getIncrementResult = GetIncrement(scriptCommand.Arguments);
        if (getIncrementResult.IsSuccess == false)
        {
            return Result<string>.Error(getIncrementResult.Message);
        }

        var increment = getIncrementResult.Data;


        _interpreter.SetVariableValue(variableName, string.Format(CultureInfo.InvariantCulture, "{0}", originalVariableValue + increment));

        return Result<string>.Ok(_interpreter.GetVariableValue(variableName), null);
    }


    private readonly IInterpreter _interpreter;


    private IResult<string> GetVariableName(IList<ICommandArgument> arguments)
    {
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


    private IResult<int> GetVariableValue(string variableName)
    {
        if (_interpreter.HasVariable(variableName) == false)
        {
            return Result<int>.Ok(0);
        }

        var variableValue = _interpreter.GetVariableValue(variableName);

        return int.TryParse(variableValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integerValue) 
            ? Result<int>.Ok(integerValue)
            : Result<int>.Error(0, $"The '{variableValue}' value of the {variableName} is not a valid integer number.");
    }


    private IResult<int> GetIncrement(IList<ICommandArgument> arguments)
    {
        string? increment = null;

        if (arguments.Count > 2)
        {
            var incrementArgument = arguments[2];
            var interpretArgumentValueResult = _interpreter.Interpret(incrementArgument);
            if (interpretArgumentValueResult.IsSuccess == false)
            {
                return Result<int>.Error(0, interpretArgumentValueResult.Message);
            }

            increment = interpretArgumentValueResult.Data;
        }

        increment = string.IsNullOrWhiteSpace(increment)
            ? "1"
            : increment;

        return int.TryParse(increment, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integerValue) 
            ? Result<int>.Ok(integerValue)
            : Result<int>.Error(0, $"The '{increment}' value of the increment is not a valid integer number.");
    }
}