/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A script command implementation.
/// </summary>
public interface ICommandImplementation
{
    /// <summary>
    /// The body (aka the operation) of this command.
    /// </summary>
    /// <returns>An IResult instance with a string representing the result of this command operation.</returns>
    IResult<string> Execute(IScriptCommand scriptCommand);
}
