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

        var i = 0;
        while (i < Value.Length)
        {
            var c = Value[i];

            if (c == '$')
            {
                i++;
                if (i >= Value.Length)
                {
                    return Result<string>.Error("Unexpected '$' at the end of the string.");
                }

                var nameSb = new StringBuilder();
                
                // $name = $A-Z,a-z,0-9,_
                while (i < Value.Length)
                {
                    c = Value[i];
                    if (c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_')
                    {
                        nameSb.Append(c);
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                sb.Append(interpreter.GetVariableValue(nameSb.ToString()));
            }
            else
            {
                sb.Append(c);
                i++;
            }
        }

        return Result<string>.Ok(sb.ToString(), null);
    }
}

// https://wiki.tcl-lang.org/page/Dodekalogue

// variable-name:
//   $name = $A-Z,a-z,0-9,_
//   ${name} = any-char-except '}' 