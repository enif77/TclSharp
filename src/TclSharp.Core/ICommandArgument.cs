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
}