/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


public class Interpreter : IInterpreter
{
    public IOutputWriter Output { get; }


    public Interpreter(IOutputWriter output)
    {
        Output = output ?? throw new ArgumentNullException(nameof(output));

        _commands = new List<ICommand>();
        _commandImplementations = new Dictionary<string, ICommandImplementation>();
    }


    public IResult<ICommand> AddCommand(ICommand command)
    {
        _commands.Add(command);

        return Result<ICommand>.Ok(command, $"The '{command.Name}' command added.");
    }


    public bool IsKnownCommand(string commandName)
    {
        return _commandImplementations.ContainsKey(commandName);
    }


    public IResult<ICommandImplementation> AddCommandImplementation(string commandName, ICommandImplementation commandImplementation)
    {
        if (_commandImplementations.ContainsKey(commandName))
        {
            return Result<ICommandImplementation>.Error(commandImplementation, $"The '{commandName}' is already defined.");
        }

        _commandImplementations.Add(commandName, commandImplementation);

        return Result<ICommandImplementation>.Ok(commandImplementation, $"The '{commandName}' command implementation added.");
    }

    
    public IResult<string> Execute()
    {
        var lastResult = Result<string>.Ok();
        foreach (var command in _commands)
        {
            if (_commandImplementations.ContainsKey(command.Name) == false)
            {
                return Result<string>.Error($"The '{command.Name}' not defined.");
            }

            var commandImplementation = _commandImplementations[command.Name];
            
            lastResult = commandImplementation.Execute(command);
            if (lastResult.IsSuccess == false)
            {
                break;
            }
        }

        return lastResult;
    }


    private readonly List<ICommand> _commands;
    private readonly IDictionary<string, ICommandImplementation> _commandImplementations;
}