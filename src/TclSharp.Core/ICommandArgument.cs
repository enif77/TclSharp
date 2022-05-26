/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Defines a command argument.
/// </summary>
public interface ICommandArgument
{
    /// <summary>
    /// A processed argument value.
    /// </summary>
    string Value { get; }


    /// <summary>
    /// Adds a value part to.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value parameter is null.</exception>
    void AddValue(ICommandArgumentValue value);
}