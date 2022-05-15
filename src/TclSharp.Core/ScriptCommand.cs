/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A basic command implementation.
/// </summary>
public class ScriptCommand : IScriptCommand
{
    public string Name { get; }

    public IList<ICommandArgument> Arguments { get; }


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">A command name.</param>
    public ScriptCommand(string name)
    {
        Name = name;
        Arguments = new List<ICommandArgument>();
    }
}