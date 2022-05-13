/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Extensions;

using TclSharp.Core.Commands;
using TclSharp.Core.CommandArguments;


/// <summary>
/// IInterpreter related helper methods.
/// </summary>
public static class InterpreterExtensions
{
    /// <summary>
    /// Adds the puts command to an interpreter.
    /// </summary>
    /// <param name="interpreter">An IInterpreter instance.</param>
    /// <param name="message">A message.</param>
    /// <param name="noNewLine">If true, no new line char(s) will be emitted at the end of this command output.</param>
    /// <returns>The added ICommand instance.</returns>
    public static IResult<ICommand> AddPutsCommand(this IInterpreter interpreter, string? message = default, bool noNewLine = false)
    {
        const string PutsCommandName = "puts";
        
        if (interpreter.IsKnownCommand(PutsCommandName) == false)
        {
            var addCommandImplementationResult = interpreter.AddCommandImplementation(PutsCommandName, new PutsCommand(interpreter));
            if (addCommandImplementationResult.IsSuccess == false)
            {
                return Result<ICommand>.Error(addCommandImplementationResult.Message);
            }
        }

        var command = new Command(PutsCommandName);

        if (noNewLine)
        {
            command.Arguments.Add(new SimpleArgument("-nonewline"));
        }

        command.Arguments.Add(new SimpleArgument(message ?? string.Empty));
        
        return interpreter.AddCommand(command);
    }
}
