/* TclSharp - (C) 2022 Premysl Fara */

using System.Text;

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
    
    
    #region command arguments

    public IResult<string> ProcessArgumentValue(ICommandArgument argument)
    {
        var value = argument.Value;
        
        var sb = new StringBuilder(value.Length);

        var reader = new StringSourceReader(value);
        
        var c = reader.NextChar();
        while (c >= 0)
        {
            if (c == '$')
            {
                var extractVariableNameResult = ExtractVariableName(reader);
                if (extractVariableNameResult.IsSuccess == false)
                {
                    return extractVariableNameResult;
                }

                sb.Append(GetVariableValue(extractVariableNameResult.Data!));
                c = reader.CurrentChar;
                
                continue;
            }

            sb.Append((char)c);
            c = reader.NextChar();
        }

        return Result<string>.Ok(sb.ToString(), null);
    }

    
    private IResult<string> ExtractVariableName(ISourceReader reader)
    {
        var c = reader.NextChar();  // Eat '$' 
        if (c < 0)
        {
            return Result<string>.Error("Unexpected '$' at the end of the string.");
        }

        var nameSb = new StringBuilder();

        // ${name} = any-char-except '}'
        if (c == '{')
        {
            c = reader.NextChar();  // Eat '{'.
            while (c >= 0)
            {
                if (c == '}')
                {
                    reader.NextChar();
                    
                    break;
                }

                nameSb.Append((char)c);
                c = reader.NextChar();
            }
        }
        else
        {
            // $name = $A-Z,a-z,0-9,_
            while (c >= 0)
            {
                if (c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_')
                {
                    nameSb.Append((char)c);
                    c = reader.NextChar();
                    
                    continue;
                }

                break;
            }
        }
        
        return (nameSb.Length == 0)
            ? Result<string>.Error("A variable name expected.")
            : Result<string>.Ok(nameSb.ToString(), null);
    }
    
    #endregion
    
    
    #region commands
    
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
    
    #endregion

    
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