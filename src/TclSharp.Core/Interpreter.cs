/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


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
        return _variables.ContainsKey(name)
            ? _variables[name]
            : string.Empty;
    }


    public void SetVariableValue(string name, string value)
    {
        _variables[name] = value;
    }

    #endregion
    
    
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

    
    public IResult<string> Execute(IScript script)
    {
        if (script == null) throw new ArgumentNullException(nameof(script));
        
        var lastResult = Result<string>.Ok();
        foreach (var command in script.Commands)
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

    
    private readonly IDictionary<string, ICommandImplementation> _commandImplementations;
    private readonly IDictionary<string, string> _variables;
}