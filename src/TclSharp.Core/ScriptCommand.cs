/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A basic script command.
/// </summary>
public class ScriptCommand : IScriptCommand
{
    public string Name =>
        (Arguments.Count > 0)
            ? Arguments[0].Value
            : string.Empty;

    public IList<ICommandArgument> Arguments { get; } = new List<ICommandArgument>();
}