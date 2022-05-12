/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


public class Interpreter : IInterpreter
{
    public IOutputWriter Output { get; }


    public Interpreter(IOutputWriter output)
    {
        Output = output ?? throw new ArgumentNullException(nameof(output));

        _commands = new List<ICommand>();
    }


    public IResult<ICommand> AddCommand(ICommand command)
    {
        _commands.Add(command);

        return Result<ICommand>.Ok(command, $"The '{command.Name}' command added.");
    }

    
    public IResult<string> EvaluateParameter(string parameter)
    {
        return Result<string>.Ok(parameter, parameter);
    }

    
    public IResult<string> Execute()
    {
        var lastResult = Result<string>.Ok();
        foreach (var command in _commands)
        {
            lastResult = command.Execute();
            if (lastResult.IsSuccess == false)
            {
                break;
            }
        }

        return lastResult;
    }


    private readonly List<ICommand> _commands;
}