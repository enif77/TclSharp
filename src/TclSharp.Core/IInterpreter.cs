/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using TclSharp.Core.Results;


/// <summary>
/// Defines an interpreter.
/// </summary>
public interface IInterpreter
{
    /// <summary>
    /// The text output for commands.
    /// </summary>
    IOutputWriter Output { get; }
    
    
    #region variables

    /// <summary>
    /// Checks, if a certain variable exists.
    /// </summary>
    /// <param name="name">A variable name.</param>
    /// <returns>True, if a variable with given name exists.</returns>
    bool HasVariable(string name);

    /// <summary>
    /// Gets a value of a variable.
    /// </summary>
    /// <param name="name">A variable name.</param>
    /// <returns>A variable value or an empty string, if no such variable exists.</returns>
    string GetVariableValue(string name);

    /// <summary>
    /// Sets a value to a variable.
    /// If no variable exists, creates it.
    /// </summary>
    /// <param name="name">A variable name.</param>
    /// <param name="value">A value.</param>
    void SetVariableValue(string name, string value);
    
    #endregion

    
    #region command arguments

    /// <summary>
    /// Substitutes variables and commands in a command argument.
    /// </summary>
    /// <param name="argument">A command arguments.</param>
    /// <returns>Processed command argument.</returns>
    IResult<string> InterpretCommandArgument(ICommandArgument argument);

    #endregion
    

    #region commands

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

    #endregion
    

    /// <summary>
    /// Executes all commands in this instance.
    /// </summary>
    /// <param name="script">A script.</param>
    /// <returns>An IResult instance with a string representing the result of this interpreter instance operation.</returns>
    IResult<string> Execute(IScript script);
}