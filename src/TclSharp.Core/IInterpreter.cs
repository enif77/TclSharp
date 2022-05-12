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
    /// Adds a command implementation to this instance. 
    /// </summary>
    /// <param name="command">A command implementation.</param>
    /// <returns>An IResult instance with the added command instance.</returns>
    IResult<ICommand> AddCommand(ICommand command);


    /// <summary>
    /// Evaluates an parameter. Does all necessary substitutions and interpretations.
    /// </summary>
    /// <param name="parameter">A command parameter value.</param>
    /// <returns>An IResult instance with a string representing the result of the parameter evaluation.</returns>
    IResult<string> EvaluateParameter(string parameter);

    /// <summary>
    /// Executes all commands in this instance.
    /// </summary>
    /// <returns>An IResult instance with a string representing the result of this interpreter instance operation.</returns>
    IResult<string> Execute();
}