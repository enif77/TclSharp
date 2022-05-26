/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using TclSharp.Core.Results;


/// <summary>
/// Defines a command argument.
/// </summary>
public interface ICommandArgument
{
    /// <summary>
    /// An unprocessed argument value.
    /// </summary>
    string Value { get; }


    /// <summary>
    /// Adds a value part to.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value parameter is null.</exception>
    void AddValue(ICommandArgumentValue value);
    
    /// <summary>
    /// Interprets its values and return the resulting value as a string.
    /// </summary>
    /// <param name="interpreter">An interpreter used for this value evaluations.</param>
    /// <returns>Interpreted value.</returns>
    IResult<string> Interpret(IInterpreter interpreter);
}