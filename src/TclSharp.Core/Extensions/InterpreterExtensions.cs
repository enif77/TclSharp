/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Extensions;

using TclSharp.Core.Commands;


/// <summary>
/// IInterpreter related helper methods.
/// </summary>
public static class InterpreterExtensions
{
    /// <summary>
    /// Adds the puts command implementation to an interpreter.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <returns>The IResult representing the add-command operation result.</returns>
    public static IResult AddPutsCommand(this IInterpreter interpreter)
    {
        const string putsCommandName = "puts";

        if (interpreter.IsKnownCommand(putsCommandName))
        {
            return SimpleResult.Ok($"The '{putsCommandName}' command implementation was already added.");
        }
       
        var addCommandImplementationResult = interpreter.AddCommandImplementation(putsCommandName, new PutsCommand(interpreter));
        
        return addCommandImplementationResult.IsSuccess == false
            ? SimpleResult.Error(addCommandImplementationResult.Message)
            : SimpleResult.Ok($"The '{putsCommandName}' command implementation added successfully.");
    }
    
    /// <summary>
    /// Adds the set command implementation to an interpreter.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <returns>The IResult representing the add-command operation result.</returns>
    public static IResult AddSetCommand(this IInterpreter interpreter)
    {
        const string setCommandName = "set";

        if (interpreter.IsKnownCommand(setCommandName))
        {
            return SimpleResult.Ok($"The '{setCommandName}' command implementation was already added.");
        }
       
        var addCommandImplementationResult = interpreter.AddCommandImplementation(
            setCommandName,
            new SetCommand(interpreter));
        
        return addCommandImplementationResult.IsSuccess == false
            ? SimpleResult.Error(addCommandImplementationResult.Message)
            : SimpleResult.Ok($"The '{setCommandName}' command implementation added successfully.");
    }
}
