/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using TclSharp.Core.Results;


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
            : UnexpectedTokenResult(token, "EoF");
    }

    /// <summary>
    /// Parsers commands.
    /// commands :: command { command-separator command } .  
    /// </summary>
    private IResult<string> ParseCommands(ITokenizer tokenizer, IScript script)
    {
        IScriptCommand? currentScriptCommand = null;
        
        var token = tokenizer.CurrentToken;
        
        while (token.Code != TokenCode.EoF)
        {
            currentScriptCommand ??= new ScriptCommand();
            var parseCommandResult = ParseCommand(tokenizer, currentScriptCommand, false);
            if (parseCommandResult.IsSuccess == false)
            {
                return parseCommandResult;
            }

            if (currentScriptCommand.Arguments.Count > 0)
            {
                // We are not adding empty script commands.
                script.AddCommand(currentScriptCommand);
                currentScriptCommand = null;
            }

            token = tokenizer.CurrentToken;
            
            if (token.Code == TokenCode.CommandSeparator)
            {
                token = tokenizer.NextToken();
            }
            
            // Here we start a next command parsing or we are at EoF.
        }
       
        return Result<string>.Ok();
    }


    /// <summary>
    /// Parses a command.
    /// command :: [ word { words-separator word } ] .
    /// </summary>
    private IResult<string> ParseCommand(ITokenizer tokenizer, IScriptCommand scriptCommand, bool isNested)
    {
        var token = tokenizer.CurrentToken;

        // An empty command?
        if (token.Code is TokenCode.CommandSeparator)
        {
            return Result<string>.Ok();
        }
        
        var done = false;
        while (done == false)
        {
            switch (token.Code)
            {
                case TokenCode.Word:
                    var parseWordResult = ParseWord(tokenizer, scriptCommand);
                    if (parseWordResult.IsSuccess == false)
                    {
                        return parseWordResult;
                    }
                    token = tokenizer.CurrentToken;
                    break;
                
                case TokenCode.CommandSeparator:
                case TokenCode.EoF:
                    done = true;
                    break;
                
                default:
                    return UnexpectedTokenResult(token, "A command-separator or EoF");
            }
        }
        
        return Result<string>.Ok();
    }

    /// <summary>
    /// Parses a word.
    /// word :: basic-word | quoted-word | bracketed-word .
    /// </summary>
    private IResult<string> ParseWord(ITokenizer tokenizer, IScriptCommand scriptCommand)
    {
        var token = tokenizer.CurrentToken;
        if (token.Code != TokenCode.Word)
        {
            return UnexpectedTokenResult(token, "A word");
        }

        var tokenValue = token.Children[0].StringValue;
        
        scriptCommand.Arguments.Add(new CommandArgument(tokenValue));
        
        tokenizer.NextToken();

        return Result<string>.Ok();
    }
    
    /// <summary>
    /// Parses a basic-word.
    /// basic-word :: char { chars } .
    /// </summary>
    private IResult<string> ParseSimpleWord(ITokenizer tokenizer, ICommandArgument commandArgument)
    {
        return Result<string>.Error($"The basic-word parsing is not implemented yet.");
    }
    
    /// <summary>
    /// Parses a quoted-word.
    /// quoted-word :: '"' { chars } '"' .
    /// </summary>
    private IResult<string> ParseQuotedWord(ITokenizer tokenizer, ICommandArgument commandArgument)
    {
        return Result<string>.Error($"The quoted-word parsing is not implemented yet.");
    }
    
    /// <summary>
    /// Parses a bracketed-word.
    /// bracketed-word :: '{' { chars } '}' .
    /// </summary>
    private IResult<string> ParseBracketedWord(ITokenizer tokenizer, ICommandArgument commandArgument)
    {
        return Result<string>.Error($"The bracketed-word parsing is not implemented yet.");
    }


    private static IResult<string> UnexpectedTokenResult(IToken token, string expectedToken)
    {
        return Result<string>.Error($"Unexpected token with code {token.Code} found while parsing commands. {expectedToken} expected.");
    }
}