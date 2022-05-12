/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Extensions;

using TclSharp.Core.Commands;


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
        return interpreter.AddCommand(new PutsCommand(interpreter, message ?? string.Empty, noNewLine));
    }
}
