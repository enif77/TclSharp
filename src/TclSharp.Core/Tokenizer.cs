/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


public class Tokenizer : ITokenizer
{
    public IToken CurrentToken { get; private set; }


    public Tokenizer(ISourceReader reader)
    {
        CurrentToken = _eofToken;
        
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }


    public IToken NextToken()
    {
        return _eofToken;
    }
    
    
    private readonly ISourceReader _reader;
    private readonly IToken _eofToken = new Token(TokenCode.Eof);
}