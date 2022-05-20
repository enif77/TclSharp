/* TclSharp - (C) 2022 Premysl Fara */

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
    /// script :: EOF . 
    /// </summary>
    /// <param name="tokenizer">A tokenizer for extracting tokens from a source.</param>
    /// <param name="script">A script for storing extracted commands.</param>
    /// <returns>A result of the operation.</returns>
    private IResult<string> ParseScript(ITokenizer tokenizer, IScript script)
    {
        var token = tokenizer.NextToken();

        return (token.Code == TokenCode.EoF)
            ? Result<string>.Ok()
            : Result<string>.Error("EoF expected.");
    }
}