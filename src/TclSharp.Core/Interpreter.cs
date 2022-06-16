/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using TclSharp.Core.Results;


public class Interpreter : IInterpreter
{
    public IOutputWriter Output { get; }


    public Interpreter(IOutputWriter output)
    {
        Output = output ?? throw new ArgumentNullException(nameof(output));

        _commandImplementations = new Dictionary<string, ICommandImplementation>();
        _variables = new Dictionary<string, string>();
    }

    
    #region variables

    public bool HasVariable(string name)
    {
        return _variables.ContainsKey(name);
    }


    public string GetVariableValue(string name)
    {
        return HasVariable(name)
            ? _variables[name]
            : string.Empty;
    }


    public void SetVariableValue(string name, string value)
    {
        _variables[name] = value;
    }

    #endregion
    
    
    #region command arguments

    public IResult<string> Interpret(ICommandArgument argument)
    {
        var interpretCommandArgumentResult = argument.Interpret(this);
        
        return interpretCommandArgumentResult.IsSuccess == false
            ? interpretCommandArgumentResult
            : Result<string>.Ok(interpretCommandArgumentResult.Data ?? string.Empty, null);
    }

    #endregion
    
    
    #region commands
    
    public bool IsKnownCommand(string commandName)
    {
        return _commandImplementations.ContainsKey(commandName);
    }


    public IResult<ICommandImplementation> AddCommandImplementation(string commandName, ICommandImplementation commandImplementation)
    {
        if (IsKnownCommand(commandName))
        {
            return Result<ICommandImplementation>.Error(commandImplementation, $"The '{commandName}' is already defined.");
        }

        _commandImplementations.Add(commandName, commandImplementation);

        return Result<ICommandImplementation>.Ok(commandImplementation, $"The '{commandName}' command implementation added.");
    }
    
    #endregion

    
    public IResult<string> Interpret(IScript script)
    {
        if (script == null) throw new ArgumentNullException(nameof(script));
        
        var lastCommandExecutionResult = Result<string>.Ok();
        foreach (var command in script.Commands)
        {
            var interpretCommandNameResult = Interpret(command.Arguments[0]);
            if (interpretCommandNameResult.IsSuccess == false)
            {
                return interpretCommandNameResult;
            }

            var commandName = interpretCommandNameResult.Data!;
            
            if (_commandImplementations.ContainsKey(commandName) == false)
            {
                return Result<string>.Error($"The '{commandName}' command, defined as '{command.Name}', is not defined.");
            }

            var commandImplementation = _commandImplementations[commandName];
            
            lastCommandExecutionResult = commandImplementation.Execute(command);
            if (lastCommandExecutionResult.IsSuccess == false)
            {
                break;
            }
        }

        return lastCommandExecutionResult;
    }

    
    private readonly IDictionary<string, ICommandImplementation> _commandImplementations;
    private readonly IDictionary<string, string> _variables;
}