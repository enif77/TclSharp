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
        
        // At what char we were last time?
        var c = _reader.CurrentChar;
        while (c >= 0)
        {
            // Skip white chars...
            if (IsWhiteSpace(c))
            {
                if (wordSb == null)
                {
                    c = _reader.NextChar();
                
                    continue;
                }

                // A white char ends a word. We'll return it below.
                break;
            }
            
            switch (c)
            {
                case '\n':
                case ';':
                    // Are we extracting a word now?
                    if (wordSb == null)
                    {
                        // No, so consume the command separator char...
                        _reader.NextChar();
                        
                        // and return the EofC token.
                        return CurrentToken = _commandSeparatorToken;
                    }
                    // A command separator finishes a word extracting.
                    return CurrentToken = WordToken(wordSb.ToString());
                    
                default:
                    wordSb ??= new StringBuilder();
                    wordSb.Append((char) c);
                    break;
            }
        
            c = _reader.NextChar();
        }

        // We got here, because we are at the end of the source, 
        // or because we just finished extracting a word.
        return CurrentToken = (wordSb == null)
            ? _eofToken
            : WordToken(wordSb.ToString());
    }
    
    
    private readonly ISourceReader _reader;
    private readonly IToken _eofToken = new Token(TokenCode.EoF);
    private readonly IToken _commandSeparatorToken = new Token(TokenCode.CommandSeparator);


    private static bool IsWhiteSpace(int c)
        => c != '\n' && char.IsWhiteSpace((char)c);


    private static IToken WordToken(string word)
        => new Token(TokenCode.Word, word);
}