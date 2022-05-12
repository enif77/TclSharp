/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Commands;


/// <summary>
/// Writes a message to the output.
/// puts [ -nonewline ] message 
/// </summary>
public class PutsCommand : ICommand
{
    public string Name => "puts";


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <param name="message">A message to be written to the output.</param>
    /// <param name="noNewLine">If true, no new line character(s) are emitted at the end of the output of this command.</param>
    /// <exception cref="ArgumentNullException">Thrown, when the interpreter parameter is null.</exception>
    public PutsCommand(IInterpreter interpreter, string? message = default, bool noNewLine = false)
    {
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        _message = message;
        _noNewLine = noNewLine;
    }
    

    public IResult<string> Execute()
    {
        if (string.IsNullOrEmpty(_message) && _noNewLine == false)
        {
            _interpreter.Output.WriteLine();

            return Result<string>.Ok();
        }

        var evaluationResult = _interpreter.EvaluateParameter(_message!);
        if (evaluationResult.IsSuccess == false)
        {
            return Result<string>.Error(_message, evaluationResult.Message);
        }
        
        if (_noNewLine)
        {
            _interpreter.Output.Write(evaluationResult.Data!);
        }
        else
        {
            _interpreter.Output.WriteLine(evaluationResult.Data!);    
        }

        return Result<string>.Ok(evaluationResult.Data, null);
    }


    private readonly IInterpreter _interpreter;
    private readonly string? _message;
    private readonly bool _noNewLine;
}