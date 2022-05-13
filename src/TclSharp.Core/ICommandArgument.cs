/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


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
    /// Gets a processed value.
    /// </summary>
    /// <param name="interpreter">An interpreter with shared values/state.</param>
    /// <returns>A processed value.</returns>
    IResult<string> GetProcessedValue(IInterpreter interpreter);
}