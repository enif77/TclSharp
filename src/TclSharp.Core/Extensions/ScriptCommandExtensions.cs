/* TclSharp - (C) 2022 Premysl Fara */

using TclSharp.Core.CommandArgumentValues;

namespace TclSharp.Core.Extensions;


public static class ScriptCommandExtensions
{
    /// <summary>
    /// Adds a simple text argument to a command.
    /// </summary>
    /// <param name="scriptCommand">A script command.</param>
    /// <param name="value">A value.</param>
    public static void AddTextArgument(this IScriptCommand scriptCommand, string value)
    {
        scriptCommand.Arguments.Add(new SimpleCommandArgument(value));
    }
}