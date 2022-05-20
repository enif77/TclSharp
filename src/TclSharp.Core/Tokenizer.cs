/* TclSharp - (C) 2022 Premysl Fara */

using System.Text;

namespace TclSharp.Core;


public class Tokenizer : ITokenizer
{
    public IToken CurrentToken { get; private set; }


    public Tokenizer(ISourceReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        CurrentToken = _eofToken;
        
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
                    c = NextChar();
                
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
                        NextChar();
                        
                        // and return the EofC token.
                        return CurrentToken = _commandSeparatorToken;
                    }
                    // A command separator finishes a word extracting.
                    return CurrentToken = WordToken(wordSb.ToString());

                case '"':
                    if (wordSb != null)
                    {
                        return ErrorToken("Unexpected quoted word start character '\"' found.");
                    }

                    var extractQuotedWordResult = ExtractQuotedWord();
                    if (extractQuotedWordResult.IsSuccess == false)
                    {
                        return ErrorToken(extractQuotedWordResult.Message);
                    }
                    return CurrentToken = WordToken(extractQuotedWordResult.Data!);
                
                case '{':
                    if (wordSb != null)
                    {
                        return ErrorToken("Unexpected braced word start character '{' found.");
                    }

                    var extractBracketedWordResult = ExtractBracketedWord();
                    if (extractBracketedWordResult.IsSuccess == false)
                    {
                        return ErrorToken(extractBracketedWordResult.Message);
                    }
                    return CurrentToken = WordToken(extractBracketedWordResult.Data!);
                
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

    private int _nextChar = -1;

    private int NextChar()
    {
        if (_nextChar >= 0)
        {
            var nextChar = _nextChar;
            _nextChar = -1;

            return nextChar;
        }

        var c = _reader.NextChar();
        if (c != '\\')
        {
            return c;
        }

        c = _reader.NextChar();
        if (c != '\n')
        {
            _nextChar = c;

            return '\\';
        }

        c = _reader.NextChar();
        while (IsWhiteSpace(c))
        {
            c = _reader.NextChar();
        }

        _nextChar = c;

        return ' ';
    }


    private static bool IsWhiteSpace(int c)
        => c != '\n' && char.IsWhiteSpace((char)c);


    private static IToken WordToken(string word)
        => new Token(TokenCode.Word, word);
    
    private static IToken ErrorToken(string msg)
        => new Token(TokenCode.Unknown, msg);


    private IResult<string> ExtractQuotedWord()
    {
        var wordSb = new StringBuilder();
        
        var c = NextChar();
        while (c >= 0)
        {
            if (c == '"')
            {
                NextChar();
        
                return Result<string>.Ok(wordSb.ToString(), null);
            }

            wordSb.Append((char) c);
            
            c = NextChar();
        }

        return Result<string>.Error("The quoted word end character '\"' expected.");
    }
    
    
    private IResult<string> ExtractBracketedWord()
    {
        var wordSb = new StringBuilder();

        var bracketLevel = 1;
        var c = NextChar();
        while (c >= 0)
        {
            switch (c)
            {
                case '\\':
                    var escapeBracketedCharResult = EscapeBracketedChar();
                    if (escapeBracketedCharResult.IsSuccess == false)
                    {
                        return Result<string>.Error(escapeBracketedCharResult.Message);
                    }
                    c = escapeBracketedCharResult.Data;
                    break;
                
                case '{':
                    bracketLevel++;
                    break;
                
                case '}':
                {
                    bracketLevel--;
                    if (bracketLevel == 0)
                    {
                        NextChar();
        
                        return Result<string>.Ok(wordSb.ToString(), null);
                    }
                    break;
                }
            }

            wordSb.Append((char) c);
            
            c = NextChar();
        }

        return Result<string>.Error("The bracketed word end character '}' expected.");
    }


    private IResult<char> EscapeBracketedChar()
    {
        var c = NextChar();
        switch (c)
        {
            case '{':
            case '}':
                return Result<char>.Ok((char)c);
            
            default:
                return Result<char>.Error($"Unsupported escaped character '{(char)c}' in bracketed word.");
        }
    }
}