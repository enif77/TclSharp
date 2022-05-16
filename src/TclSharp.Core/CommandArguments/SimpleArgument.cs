/* TclSharp - (C) 2022 Premysl Fara */

using System.Text;

namespace TclSharp.Core.CommandArguments;


/// <summary>
/// A command argument, that does no value processing or substitution.
/// </summary>
public class SimpleArgument : ICommandArgument
{
    public string Value { get; }
    
    
    public SimpleArgument(string value)
    {
        Value = value;
    }
    
    
    public IResult<string> GetProcessedValue(IInterpreter interpreter)
    {
        var sb = new StringBuilder(Value.Length);

        var reader = new StringSourceReader(Value);
        
        var c = reader.NextChar();
        while (c != 0)
        {
            if (c == '$')
            {
                var extractVariableNameResult = ExtractVariableName(reader);
                if (extractVariableNameResult.IsSuccess == false)
                {
                    return extractVariableNameResult;
                }

                sb.Append(interpreter.GetVariableValue(extractVariableNameResult.Data!));
                c = reader.CurrentChar;
                
                continue;
            }

            sb.Append(c);
            c = reader.NextChar();
        }

        return Result<string>.Ok(sb.ToString(), null);
    }


    private IResult<string> ExtractVariableName(ISourceReader reader)
    {
        var c = reader.NextChar();  // Eat '$' 
        if (c == 0)
        {
            return Result<string>.Error("Unexpected '$' at the end of the string.");
        }

        var nameSb = new StringBuilder();

        // ${name} = any-char-except '}'
        if (c == '{')
        {
            c = reader.NextChar();  // Eat '{'.
            while (c != 0)
            {
                if (c == '}')
                {
                    reader.NextChar();
                    
                    break;
                }

                nameSb.Append(c);
                c = reader.NextChar();
            }
        }
        else
        {
            // $name = $A-Z,a-z,0-9,_
            while (c != 0)
            {
                if (c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_')
                {
                    nameSb.Append(c);
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

}

// https://wiki.tcl-lang.org/page/Dodekalogue

// variable-name:
//   $name = $A-Z,a-z,0-9,_
//   ${name} = any-char-except '}' 