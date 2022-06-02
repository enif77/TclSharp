/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using TclSharp.Core.Results;


/// <summary>
/// Represents a part of a command argument value.
/// </summary>
public interface ICommandArgumentValue
{
    /// <summary>
    /// An unprocessed value.
    /// </summary>
    string Value { get; }


    /// <summary>
    /// Interprets the Value and return the resulting value as a string.
    /// </summary>
    /// <param name="interpreter">An interpreter used for this value evaluations.</param>
    /// <returns>Interpreted value.</returns>
    IResult<string> Interpret(IInterpreter interpreter);
}
