/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Commands;


/// <summary>
/// Writes a message to the output.
/// puts [ -nonewline ] message 
/// </summary>
public class PutsCommand : ICommandImplementation
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <exception cref="ArgumentNullException">Thrown, when the interpreter parameter is null.</exception>
    public PutsCommand(IInterpreter interpreter)
    {
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
    }
    

    public IResult<string> Execute(ICommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (command.Arguments.Count == 0)
        {
            return Result<string>.Error("At least one argument, a message, expected.");
        }
        
        var noNewLine = false;
        string message;
        
        var firstArgument = command.Arguments[0];
        var getFirstArgumentValueResult = firstArgument.GetProcessedValue(_interpreter);
        if (getFirstArgumentValueResult.IsSuccess == false)
        {
            return Result<string>.Error(firstArgument.Value, getFirstArgumentValueResult.Message);
        }
        
        var firstArgumentValue = getFirstArgumentValueResult.Data ?? string.Empty;
        if (firstArgumentValue.Equals("-nonewline", StringComparison.InvariantCultureIgnoreCase))
        {
            noNewLine = true;

            if (command.Arguments.Count < 2)
            {
                return Result<string>.Error("The second argument, a message, expected.");
            }

            var secondArgument = command.Arguments[1];
            var getSecondArgumentValueResult = secondArgument.GetProcessedValue(_interpreter);
            if (getSecondArgumentValueResult.IsSuccess == false)
            {
                return Result<string>.Error(secondArgument.Value, getSecondArgumentValueResult.Message);
            }

            message = getSecondArgumentValueResult.Data ?? string.Empty;
        }
        else
        {
            // TODO: The second value can be an output identifier.
            
            message = firstArgumentValue;
        }

        if (string.IsNullOrEmpty(message) && noNewLine == false)
        {
            _interpreter.Output.WriteLine();

            return Result<string>.Ok();
        }

        if (noNewLine)
        {
            _interpreter.Output.Write(message);
        }
        else
        {
            _interpreter.Output.WriteLine(message);    
        }

        return Result<string>.Ok(message, null);
    }


    private readonly IInterpreter _interpreter;
}