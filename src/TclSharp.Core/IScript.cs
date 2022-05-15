/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Represents a script with commands.
/// </summary>
public interface IScript
{
    /// <summary>
    /// A list of commands this script consists of.
    /// </summary>
    IReadOnlyList<IScriptCommand> Commands { get; }
    
    
    /// <summary>
    /// Adds a command to this script instance. 
    /// </summary>
    /// <param name="scriptCommand">A command.</param>
    /// <returns>An IResult instance with the added command instance.</returns>
    IResult<IScriptCommand> AddCommand(IScriptCommand scriptCommand);
}