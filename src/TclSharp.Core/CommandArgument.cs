/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using System.Text;

using TclSharp.Core.Results;


/// <summary>
/// A command argument.
/// </summary>
public class CommandArgument : ICommandArgument
{
    public string Value
    {
        get
        {
            var sb = new StringBuilder();

            foreach (var value in _values)
            {
                sb.Append(value.Value);
            }
            
            return sb.ToString();
        }
    }
    
    
    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="interpreter">An IInterpreter used for evaluating this argument's values.</param>
    /// <exception cref="ArgumentNullException">Thrown when the interpreter parameter is null.</exception>
    public CommandArgument(IInterpreter interpreter)
    {
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        
        _values = new List<ICommandArgumentValue>();
    }


    public void AddValue(ICommandArgumentValue value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        _values.Add(value);
    }


    public IResult<string> Interpret(IInterpreter interpreter)
    {
        var sb = new StringBuilder();

        foreach (var value in _values)
        {
            var interpretValueResult = value.Interpret(interpreter);
            if (interpretValueResult.IsSuccess == false)
            {
                return interpretValueResult;
            }
            
            sb.Append(interpretValueResult.Data);
        }
            
        return Result<string>.Ok(sb.ToString(), null);
    }


    private readonly IInterpreter _interpreter;
    private readonly IList<ICommandArgumentValue> _values;
}
