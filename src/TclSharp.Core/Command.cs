/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A basic command implementation.
/// </summary>
public class Command : ICommand
{
    public string Name { get; }

    public IList<ICommandArgument> Arguments { get; }


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">A command name.</param>
    public Command(string name)
    {
        Name = name;
        Arguments = new List<ICommandArgument>();
    }
}