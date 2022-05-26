/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using System.Text;

using TclSharp.Core.Results;


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
        while (IsEoF(c) == false)
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

            if (IsWordsSeparator(c))
            {
                // Are we extracting a word now?
                if (wordSb == null)
                {
                    // No, so consume the command separator char...
                    NextChar();
                        
                    // and return the EofC token.
                    return CurrentToken = _commandSeparatorToken;
                }
                
                // A command separator ends a word. We'll return it below.
                break;
            }

            switch (c)
            {
                case '"':
                    if (wordSb == null)
                    {
                        var extractQuotedWordResult = ExtractQuotedWord();
                        if (extractQuotedWordResult.IsSuccess == false)
                        {
                            return ErrorToken(extractQuotedWordResult.Message);
                        }
                        
                        return CurrentToken = WordToken(extractQuotedWordResult.Data!);    
                    }
                    wordSb.Append((char) c);
                    break;
                
                case '{':
                    if (wordSb == null)
                    {
                        var extractBracketedWordResult = ExtractBracketedWord();
                        if (extractBracketedWordResult.IsSuccess == false)
                        {
                            return ErrorToken(extractBracketedWordResult.Message);
                        }
                        
                        return CurrentToken = WordToken(extractBracketedWordResult.Data!);
                    }
                    wordSb.Append((char) c);
                    break;
                
                case '[':
                    var extractCommandSubstitutionWordResult = ExtractCommandSubstitutionWord();
                    if (extractCommandSubstitutionWordResult.IsSuccess == false)
                    {
                        return ErrorToken(extractCommandSubstitutionWordResult.Message);
                    }
                    wordSb ??= new StringBuilder();
                    wordSb.Append(extractCommandSubstitutionWordResult.Data!);
                    break;
                
                default:
                    wordSb ??= new StringBuilder();
                    wordSb.Append((char) c);
                    break;
            }
        
            c = NextChar();
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

    private int _nextCharBuffer = -1;

    
    private int NextChar()
    {
        if (_nextCharBuffer >= 0)
        {
            var nextChar = _nextCharBuffer;
            _nextCharBuffer = -1;

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
            _nextCharBuffer = c;

            return '\\';
        }

        c = _reader.NextChar();
        while (IsWhiteSpace(c))
        {
            c = _reader.NextChar();
        }

        _nextCharBuffer = c;

        return ' ';
    }


    private static bool IsEoF(int c)
        => c < 0;
    
    private static bool IsWhiteSpace(int c)
        => IsWordsSeparator(c) == false && char.IsWhiteSpace((char)c);


    private static bool IsWordEnd(int c)
        => IsEoF(c) || IsWordsSeparator(c) || IsWhiteSpace(c);
    
    private static bool IsWordsSeparator(int c)
        => c is '\n' or ';';


    private static IResult<string> CheckWordEnd(int c, StringBuilder wordSb)
    {
        return IsWordEnd(c)
            ? Result<string>.Ok(wordSb.ToString(), null)
            : Result<string>.Error("An EoF, words or commands separator expected.");
    }


    private static IToken WordToken(string word)
    {
        var tok = new Token(TokenCode.Word, "word");

        tok.Children.Add(new Token(TokenCode.Text, word));
        
        return tok;
    }
   
    
    private static IToken ErrorToken(string msg)
        => new Token(TokenCode.Unknown, msg);


    private IResult<string> ExtractQuotedWord()
    {
        var wordSb = new StringBuilder();
        
        var c = NextChar();
        while (IsEoF(c) == false)
        {
            switch (c)
            {
                case '\\':
                    c = EscapeQuotedWordChar(wordSb);
                    break;
                
                case '"':
                    return CheckWordEnd(NextChar(), wordSb);
            }

            wordSb.Append((char) c);
            
            c = NextChar();
        }

        return Result<string>.Error("The quoted word end character '\"' expected.");
    }
    
    
    private int EscapeQuotedWordChar(StringBuilder wordSb)
    {
        var c = NextChar();

        // TODO: Octal and UTF chars.
        
        switch (c)
        {
            case 'a' : return 0x7;
            case 'b' : return 0x8;
            case 'f' : return 0xC;
            case 'n' : return 0xA;
            case 'r' : return 0xD;
            case 't' : return 0x9;
            case 'v' : return 0xB;
            case '\\' : return '\\';
            
            default: return c;
        }
    }
    
    
    private IResult<string> ExtractBracketedWord()
    {
        var wordSb = new StringBuilder();

        var bracketLevel = 1;
        var c = NextChar();
        while (IsEoF(c) == false)
        {
            switch (c)
            {
                case '\\':
                    c = EscapeBracketedWordChar(wordSb);
                    break;
                
                case '{':
                    bracketLevel++;
                    break;
                
                case '}':
                {
                    bracketLevel--;
                    if (bracketLevel == 0)
                    {
                        return CheckWordEnd(NextChar(), wordSb);
                    }
                    break;
                }
            }

            wordSb.Append((char) c);
            
            c = NextChar();
        }

        return Result<string>.Error("The bracketed word end character '}' expected.");
    }


    private int EscapeBracketedWordChar(StringBuilder wordSb)
    {
        var c = NextChar();
        if (c != '{' && c != '}')
        {
            wordSb.Append('\\');
        }

        return c;
    }
    
    
    private IResult<string> ExtractCommandSubstitutionWord()
    {
        var wordSb = new StringBuilder("[");

        var bracketLevel = 1;
        var c = NextChar();
        while (IsEoF(c) == false)
        {
            switch (c)
            {
                case '\\':
                    c = EscapeSquareBracketedWordChar(wordSb);
                    break;
                
                case '[':
                    bracketLevel++;
                    break;
                
                case ']':
                {
                    bracketLevel--;
                    if (bracketLevel == 0)
                    {
                        wordSb.Append(']');
                        
                        return Result<string>.Ok(wordSb.ToString(), null);
                    }
                    break;
                }
            }

            wordSb.Append((char) c);
            
            c = NextChar();
        }

        return Result<string>.Error("The bracketed word end character ']' expected.");
    }
    
    
    private int EscapeSquareBracketedWordChar(StringBuilder wordSb)
    {
        var c = NextChar();
        if (c != '[' && c != ']')
        {
            wordSb.Append('\\');
        }

        return c;
    }
}