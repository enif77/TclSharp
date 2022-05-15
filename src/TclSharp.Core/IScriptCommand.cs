/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A script command.
/// </summary>
public interface IScriptCommand
{
    /// <summary>
    /// A name of this command.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A list of command arguments.
    /// </summary>
    IList<ICommandArgument> Arguments { get; }
}
