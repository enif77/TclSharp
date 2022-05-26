/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Represents a command value.
/// </summary>
public interface ICommandArgumentValue
{
    /// <summary>
    /// An unprocessed value.
    /// </summary>
    string Value { get; }


    /// <summary>
    /// Interprets its value and return the resulting value as a string.
    /// </summary>
    /// <param name="interpreter">An interpreter used for this value evaluations.</param>
    /// <returns>Interpreted value.</returns>
    string Interpret(IInterpreter interpreter);
}
