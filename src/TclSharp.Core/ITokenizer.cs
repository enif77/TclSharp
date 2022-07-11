/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using TclSharp.Core.Results;


/// <summary>
/// Defines a tokenizer.
/// </summary>
public interface ITokenizer
{
    /// <summary>
    /// The last extracted (the current) token.
    /// </summary>
    IToken CurrentToken { get; }
    
    
    /// <summary>
    /// Extracts the next token from the source.
    /// </summary>
    /// <returns>The next token from the source or EOF at the end of the source.</returns>
    IResult<IToken> NextToken();
}