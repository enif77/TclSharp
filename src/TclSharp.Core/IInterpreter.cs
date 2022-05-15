/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Defines an interpreter.
/// </summary>
public interface IInterpreter
{
    /// <summary>
    /// The text output for commands.
    /// </summary>
    IOutputWriter Output { get; }

    
    /// <summary>
    /// Checks, if a command implementation exists.
    /// </summary>
    /// <param name="commandName">A command name.</param>
    /// <returns>True, if a command implementation exists.</returns>
    bool IsKnownCommand(string commandName);
    
    /// <summary>
    /// Adds a command implementation to this instance. 
    /// </summary>
    /// <param name="commandName">A command name.</param>
    /// <param name="commandImplementation">A command implementation.</param>
    /// <returns>An IResult instance with the added command instance.</returns>
    IResult<ICommandImplementation> AddCommandImplementation(string commandName, ICommandImplementation commandImplementation);

    /// <summary>
    /// Executes all commands in this instance.
    /// </summary>
    /// <param name="script">A script.</param>
    /// <returns>An IResult instance with a string representing the result of this interpreter instance operation.</returns>
    IResult<string> Execute(IScript script);
}