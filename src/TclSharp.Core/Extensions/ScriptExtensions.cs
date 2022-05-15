/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Extensions;

using TclSharp.Core.CommandArguments;


/// <summary>
/// IScript related helper methods.
/// </summary>
public static class ScriptExtensions
{
    /// <summary>
    /// Adds the puts command to a script.
    /// </summary>
    /// <param name="script">An IScript instance.</param>
    /// <param name="message">A message.</param>
    /// <param name="noNewLine">If true, no new line char(s) will be emitted at the end of this command output.</param>
    /// <returns>The added ICommand instance.</returns>
    public static IResult<IScriptCommand> AddPutsCommand(this IScript script, string? message = default, bool noNewLine = false)
    {
        var command = new ScriptCommand("puts");

        if (noNewLine)
        {
            command.Arguments.Add(new SimpleArgument("-nonewline"));
        }

        command.Arguments.Add(new SimpleArgument(message ?? string.Empty));
        
        return script.AddCommand(command);
    }
}
