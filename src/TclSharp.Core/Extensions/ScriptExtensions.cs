/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Extensions;

using TclSharp.Core.Results;


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
        var command = new ScriptCommand();
        
        command.AddTextArgument("puts");
        
        if (noNewLine)
        {
            command.AddTextArgument("-nonewline");
        }

        command.AddTextArgument(message ?? string.Empty);
        
        return script.AddCommand(command);
    }

    /// <summary>
    /// Adds the set command to a script.
    /// </summary>
    /// <param name="script">An IScript instance.</param>
    /// <param name="variableName">A variable name.</param>
    /// <param name="value">An optional value.</param>
    /// <returns>The added ICommand instance.</returns>
    public static IResult<IScriptCommand> AddSetCommand(this IScript script, string variableName, string? value = default)
    {
        var command = new ScriptCommand();

        command.AddTextArgument("set");
        command.AddTextArgument(variableName);

        if (value != null)
        {
            command.AddTextArgument(value);
        }

        return script.AddCommand(command);
    }
}
