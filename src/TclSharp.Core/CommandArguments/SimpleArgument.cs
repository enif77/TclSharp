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
                c = reader.NextChar();
                if (c == 0)
                {
                    return Result<string>.Error("Unexpected '$' at the end of the string.");
                }
                
                var nameSb = new StringBuilder();
                
                // $name = $A-Z,a-z,0-9,_
                while (c != 0)
                {
                    if (c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_')
                    {
                        nameSb.Append(c);
                        c = reader.NextChar();
                    }
                    else
                    {
                        break;
                    }
                }
                
                sb.Append(interpreter.GetVariableValue(nameSb.ToString()));
                
                continue;
            }

            sb.Append(c);
            c = reader.NextChar();
        }

        return Result<string>.Ok(sb.ToString(), null);
    }
}

// https://wiki.tcl-lang.org/page/Dodekalogue

// variable-name:
//   $name = $A-Z,a-z,0-9,_
//   ${name} = any-char-except '}' 