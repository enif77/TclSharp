/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using System.Text;

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

    public IResult<string> InterpretCommandArgument(ICommandArgument argument)
    {
        var value = argument.Value;

        if (value.StartsWith("{"))
        {
            // {bracketed string} -> bracketed string
            return Result<string>.Ok(value.Substring(1, value.Length - 2), null);
        }

        var sb = new StringBuilder(value.Length);

        var reader = new StringSourceReader(value);
        
        var c = reader.NextChar();
        while (c >= 0)
        {
            if (c == '$')
            {
                var substituteVariableResult = SubstituteVariable(reader);
                if (substituteVariableResult.IsSuccess == false)
                {
                    return substituteVariableResult;
                }

                sb.Append(substituteVariableResult.Data!);
                c = reader.CurrentChar;
                
                continue;
            }

            // if (c == '[')
            // {
            //     var substituteCommandResult = SubstituteCommand(reader);
            //     if (substituteCommandResult.IsSuccess == false)
            //     {
            //         return substituteCommandResult;
            //     }
            //
            //     sb.Append(substituteCommandResult.Data!);
            //     c = reader.CurrentChar;
            //     
            //     continue;
            // }

            sb.Append((char)c);
            c = reader.NextChar();
        }

        return Result<string>.Ok(sb.ToString(), null);
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
            var interpretCommandNameResult = InterpretCommandArgument(command.Arguments[0]);
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
    
    
    private IResult<string> SubstituteVariable(ISourceReader reader)
    {
        var c = reader.NextChar();  // Eat '$' 
        if (c < 0)
        {
            return Result<string>.Error("Unexpected '$' at the end of the script.");
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
            : Result<string>.Ok(GetVariableValue(nameSb.ToString()), null);
    }


    private IResult<string> SubstituteCommand(ISourceReader reader)
    {
        var c = reader.NextChar();  // Eat '[' 
        if (c < 0)
        {
            return Result<string>.Error("Unexpected '[' at the end of the word.");
        }

        return Result<string>.Error("Command substitution not supported.");
    }
}