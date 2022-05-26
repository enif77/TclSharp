/* TclSharp - (C) 2022 Premysl Fara */

using System.Text;

namespace TclSharp.Core;


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
                sb.Append(value.Interpret(_interpreter));
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


    /// <summary>
    /// Adds a value to this instance.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value parameter is null.</exception>
    public void AddValue(ICommandArgumentValue value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        _values.Add(value);
    }


    private readonly IInterpreter _interpreter;
    private readonly IList<ICommandArgumentValue> _values;
}
