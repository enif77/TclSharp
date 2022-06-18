/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Extensions;

using TclSharp.Core.Commands;
using TclSharp.Core.Results;



/// <summary>
/// IInterpreter related helper methods.
/// </summary>
public static class InterpreterExtensions
{
    /// <summary>
    /// Adds the incr command implementation to an interpreter.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <returns>The IResult representing the add-command operation result.</returns>
    public static IResult AddIncrCommand(this IInterpreter interpreter)
    {
        const string commandName = "incr";

        if (interpreter.IsKnownCommand(commandName))
        {
            return SimpleResult.Ok($"The '{commandName}' command implementation was already added.");
        }
       
        var addCommandImplementationResult = interpreter.AddCommandImplementation(
            commandName,
            new IncrCommand(interpreter));
        
        return addCommandImplementationResult.IsSuccess
            ? SimpleResult.Ok($"The '{commandName}' command implementation added successfully.")
            : SimpleResult.Error(addCommandImplementationResult.Message);
    }
    
    /// <summary>
    /// Adds the puts command implementation to an interpreter.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <returns>The IResult representing the add-command operation result.</returns>
    public static IResult AddPutsCommand(this IInterpreter interpreter)
    {
        const string commandName = "puts";

        if (interpreter.IsKnownCommand(commandName))
        {
            return SimpleResult.Ok($"The '{commandName}' command implementation was already added.");
        }
       
        var addCommandImplementationResult = interpreter.AddCommandImplementation(commandName, new PutsCommand(interpreter));
        
        return addCommandImplementationResult.IsSuccess
            ? SimpleResult.Ok($"The '{commandName}' command implementation added successfully.")
            : SimpleResult.Error(addCommandImplementationResult.Message);
    }
    
    /// <summary>
    /// Adds the set command implementation to an interpreter.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <returns>The IResult representing the add-command operation result.</returns>
    public static IResult AddSetCommand(this IInterpreter interpreter)
    {
        const string commandName = "set";

        if (interpreter.IsKnownCommand(commandName))
        {
            return SimpleResult.Ok($"The '{commandName}' command implementation was already added.");
        }
       
        var addCommandImplementationResult = interpreter.AddCommandImplementation(
            commandName,
            new SetCommand(interpreter));
        
        return addCommandImplementationResult.IsSuccess
            ? SimpleResult.Ok($"The '{commandName}' command implementation added successfully.")
            : SimpleResult.Error(addCommandImplementationResult.Message);
    }
}
