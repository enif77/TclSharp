/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using TclSharp.Core.Results;


/// <summary>
/// A basic script.
/// </summary>
public class Script : IScript
{
    public IReadOnlyList<IScriptCommand> Commands => _commands;
    
    
    public IResult<IScriptCommand> AddCommand(IScriptCommand scriptCommand)
    {
        _commands.Add(scriptCommand);

        return Result<IScriptCommand>.Ok(scriptCommand, $"The '{scriptCommand.Name}' command added.");
    }
    
    
    private readonly List<IScriptCommand> _commands = new List<IScriptCommand>();
}