/* TclSharp - (C) 2022 Premysl Fara */

using System.Text;

namespace TclSharp.Core;


public class Tokenizer : ITokenizer
{
    public IToken CurrentToken { get; private set; }


    public Tokenizer(ISourceReader reader)
    {
        CurrentToken = _eofToken;
        
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));

        _reader.NextChar();
    }


    public IToken NextToken()
    {
        StringBuilder? wordSb = null;
        
        int c = _reader.CurrentChar;
        while (c >= 0)
        {
            if (IsWhiteSpace(c))
            {
                if (wordSb != null)
                {
                    break;
                }

                c = _reader.NextChar();
                
                continue;
            }
            
            switch (c)
            {
                case '\n':
                case ';':
                    if (wordSb == null)
                    {
                        _reader.NextChar();
                        
                        return _commandSeparatorToken;
                    }
                    break;
        
                default:
                    wordSb ??= new StringBuilder();
                    wordSb.Append((char) c);
                    break;
            }
        
            c = _reader.NextChar();
        }

        return (wordSb == null)
            ? _eofToken
            : new Token(TokenCode.Word, wordSb.ToString());
    }
    
    
    private readonly ISourceReader _reader;
    private readonly IToken _eofToken = new Token(TokenCode.Eof);
    private readonly IToken _commandSeparatorToken = new Token(TokenCode.CommandSeparator);


    private static bool IsWhiteSpace(int c)
    {
        return c != '\n' && char.IsWhiteSpace((char)c);
    }
}