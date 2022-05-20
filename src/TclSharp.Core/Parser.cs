/* TclSharp - (C) 2022 Premysl Fara */

using TclSharp.Core.CommandArguments;

namespace TclSharp.Core;


/// <summary>
/// Parser.
/// </summary>
public class Parser : IParser
{
    public IResult<IScript> Parse(ISourceReader sourceReader)
    {
        if (sourceReader == null) throw new ArgumentNullException(nameof(sourceReader));

        var tokenizer = new Tokenizer(sourceReader);
        var script = new Script();
        var parseScriptResult = ParseScript(tokenizer, script);
        
        return parseScriptResult.IsSuccess
            ? Result<IScript>.Ok(script)
            : Result<IScript>.Error(new Script(), parseScriptResult.Message);
    }

    /// <summary>
    /// Parses a script.
    /// script :: [ commands ] EOF . 
    /// </summary>
    /// <param name="tokenizer">A tokenizer for extracting tokens from a source.</param>
    /// <param name="script">A script for storing extracted commands.</param>
    /// <returns>A result of the operation.</returns>
    private IResult<string> ParseScript(ITokenizer tokenizer, IScript script)
    {
        var token = tokenizer.NextToken();

        if (token.Code == TokenCode.EoF)
        {
            return Result<string>.Ok();
        }

        var parseCommandsResult = ParseCommands(tokenizer, script);
        if (parseCommandsResult.IsSuccess == false)
        {
            return parseCommandsResult;
        }
       
        return (tokenizer.CurrentToken.Code == TokenCode.EoF)
            ? Result<string>.Ok()
            : Result<string>.Error($"Unexpected token with code {token.Code} found while parsing commands. EoF expected.");
    }

    /// <summary>
    /// Parsers commands.
    /// commands :: command { command-separator command } .  
    /// </summary>
    private IResult<string> ParseCommands(ITokenizer tokenizer, IScript script)
    {
        IScriptCommand? currentScriptCommand = null;
        
        var token = tokenizer.CurrentToken;
        var done = false;
        while (done == false)
        {
            switch (token.Code)
            {
                case TokenCode.Word:
                    if (currentScriptCommand != null)
                    {
                        currentScriptCommand.Arguments.Add(new SimpleArgument(token.StringValue));
                    }
                    else
                    {
                        currentScriptCommand = new ScriptCommand(token.StringValue);    
                    }
                    break;
                    
                case TokenCode.CommandSeparator:
                    if (currentScriptCommand != null)
                    {
                        script.AddCommand(currentScriptCommand);
                        currentScriptCommand = null;
                    }
                    break;
                
                default:
                    done = true;
                    break;
            }

            if (done == false)
            {
                token = tokenizer.NextToken();
            }
        }
        
        if (currentScriptCommand != null)
        {
            script.AddCommand(currentScriptCommand);
        }
        
        return Result<string>.Ok();
    }
}