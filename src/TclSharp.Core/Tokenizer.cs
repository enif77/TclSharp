/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using System.Text;

using TclSharp.Core.Results;


public class Tokenizer : ITokenizer
{
    public IToken CurrentToken { get; private set; }


    public Tokenizer(ISourceReader reader)
    {
        _reader = new NewLineEscapingSourceReader(reader ?? throw new ArgumentNullException(nameof(reader)));
        CurrentToken = _eofToken;
        
        _reader.NextChar();
    }


    public IToken NextToken()
    {
        IToken? wordTok = null;
        StringBuilder? wordPartSb = null;
        
        // At what char we were last time?
        var c = _reader.CurrentChar;
        while (IsEoF(c) == false)
        {
            // Skip white chars...
            if (IsWhiteSpace(c))
            {
                if (wordPartSb == null)
                {
                    c = _reader.NextChar();
                
                    continue;
                }

                // A white char ends a word. We'll return it below.
                break;
            }

            if (IsWordsSeparator(c))
            {
                // Are we extracting a word now?
                if (wordPartSb == null)
                {
                    // No, so consume the command separator char...
                    _reader.NextChar();
                        
                    // and return the EofC token.
                    return CurrentToken = _commandSeparatorToken;
                }
                
                // A command separator ends a word. We'll return it below.
                break;
            }

            switch (c)
            {
                case '"':
                    if (wordPartSb == null)
                    {
                        var extractQuotedWordResult = ExtractQuotedWord();
                        if (extractQuotedWordResult.IsSuccess == false)
                        {
                            return ErrorToken(extractQuotedWordResult.Message);
                        }
                        
                        return CurrentToken = extractQuotedWordResult.Data!;    
                    }
                    wordPartSb.Append((char) c);
                    break;
                
                case '{':
                    if (wordPartSb == null)
                    {
                        var extractBracketedWordResult = ExtractBracketedWord();
                        if (extractBracketedWordResult.IsSuccess == false)
                        {
                            return ErrorToken(extractBracketedWordResult.Message);
                        }
                        
                        return CurrentToken = extractBracketedWordResult.Data!;
                    }
                    wordPartSb.Append((char) c);
                    break;
                
                // case '[':
                //     var extractCommandSubstitutionWordResult = ExtractCommandSubstitutionWord();
                //     if (extractCommandSubstitutionWordResult.IsSuccess == false)
                //     {
                //         return ErrorToken(extractCommandSubstitutionWordResult.Message);
                //     }
                //     wordPartSb ??= new StringBuilder();
                //     wordPartSb.Append(extractCommandSubstitutionWordResult.Data!);
                //     break;
                
                case '$':
                    var extractVariableSubstitutionResult = ExtractVariableSubstitution();
                    if (extractVariableSubstitutionResult.IsSuccess == false)
                    {
                        return ErrorToken(extractVariableSubstitutionResult.Message);
                    }
                    wordTok ??= WordToken();
                    wordTok.Children.Add(extractVariableSubstitutionResult.Data!);
                    break;
                
                default:
                    wordPartSb ??= new StringBuilder();
                    wordPartSb.Append((char) c);
                    break;
            }
        
            c = _reader.NextChar();
        }

        // We got here, because we are at the end of the source, 
        // or because we just finished extracting a word.
        if (wordPartSb == null)
        {
            return CurrentToken = wordTok ?? _eofToken;
        }

        wordTok ??= WordToken();
        wordTok.Children.Add(new Token(TokenCode.Text, wordPartSb.ToString()));
        
        return CurrentToken = wordTok;
    }
    
    
    private readonly ISourceReader _reader;
    private readonly IToken _eofToken = new Token(TokenCode.EoF);
    private readonly IToken _commandSeparatorToken = new Token(TokenCode.CommandSeparator);


    private static bool IsEoF(int c)
        => c < 0;
    
    private static bool IsWhiteSpace(int c)
        => IsWordsSeparator(c) == false && char.IsWhiteSpace((char)c);


    private static bool IsWordEnd(int c)
        => IsEoF(c) || IsWordsSeparator(c) || IsWhiteSpace(c);
    
    private static bool IsWordsSeparator(int c)
        => c is '\n' or ';';

    
    private static IToken TextToken(string text) => new Token(TokenCode.Text, text);
    
    
    private static IToken WordToken() => new Token(TokenCode.Word, "word");


    private static IToken WordToken(IToken child)
    {
        var tok = WordToken();
        
        tok.Children.Add(child);

        return tok;
    }
    
    
    private static IToken WordToken(string text) => WordToken(TextToken(text));

    
    private static IToken ErrorToken(string msg)
        => new Token(TokenCode.Unknown, msg);


    private IResult<IToken> ExtractQuotedWord()
    {
        var wordTok = WordToken();
        var wordSb = new StringBuilder();
        
        var c = _reader.NextChar();
        while (IsEoF(c) == false)
        {
            switch (c)
            {
                case '\\':
                    c = EscapeQuotedWordChar(wordSb);
                    break;
                
                case '$':
                    if (wordSb.Length > 0)
                    {
                        wordTok.Children.Add(TextToken(wordSb.ToString()));
                        wordSb = new StringBuilder();
                    }
                    
                    var extractVariableSubstitutionResult = ExtractVariableSubstitution();
                    if (extractVariableSubstitutionResult.IsSuccess == false)
                    {
                        return extractVariableSubstitutionResult;
                    }

                    // We can be at the '\' char, so we must process it here.
                    c = (_reader.CurrentChar == '\\')
                        ? EscapeQuotedWordChar(wordSb)
                        : _reader.CurrentChar;
                    
                    wordTok.Children.Add(extractVariableSubstitutionResult.Data!);
                    break;
                
                case '"':
                    if (IsWordEnd(_reader.NextChar()))
                    {
                        if (wordSb.Length > 0)
                        {
                            wordTok.Children.Add(TextToken(wordSb.ToString()));
                        }

                        return Result<IToken>.Ok(wordTok);
                    }
                    
                    return Result<IToken>.Error("An EoF, words or commands separator expected.");
            }

            wordSb.Append((char) c);
            
            c = _reader.NextChar();
        }

        return Result<IToken>.Error("The quoted word end character '\"' expected.");
    }
    
    
    private int EscapeQuotedWordChar(StringBuilder wordSb)
    {
        var c = _reader.NextChar();

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
            
            default:
                return c;
        }
    }
    
    
    private IResult<IToken> ExtractBracketedWord()
    {
        var wordSb = new StringBuilder();

        var bracketLevel = 1;
        var c = _reader.NextChar();
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
                        return IsWordEnd(_reader.NextChar())
                            ? Result<IToken>.Ok(WordToken(wordSb.ToString()))
                            : Result<IToken>.Error("An EoF, words or commands separator expected.");
                    }
                    break;
                }
            }

            wordSb.Append((char) c);
            
            c = _reader.NextChar();
        }

        return Result<IToken>.Error("The bracketed word end character '}' expected.");
    }


    private int EscapeBracketedWordChar(StringBuilder wordSb)
    {
        var c = _reader.NextChar();
        if (c != '{' && c != '}')
        {
            wordSb.Append('\\');
        }

        return c;
    }
    
    
    // private IResult<string> ExtractCommandSubstitutionWord()
    // {
    //     var wordSb = new StringBuilder("[");
    //
    //     var bracketLevel = 1;
    //     var c = NextChar();
    //     while (IsEoF(c) == false)
    //     {
    //         switch (c)
    //         {
    //             case '\\':
    //                 c = EscapeSquareBracketedWordChar(wordSb);
    //                 break;
    //             
    //             case '[':
    //                 bracketLevel++;
    //                 break;
    //             
    //             case ']':
    //             {
    //                 bracketLevel--;
    //                 if (bracketLevel == 0)
    //                 {
    //                     wordSb.Append(']');
    //                     
    //                     return Result<string>.Ok(wordSb.ToString(), null);
    //                 }
    //                 break;
    //             }
    //         }
    //
    //         wordSb.Append((char) c);
    //         
    //         c = NextChar();
    //     }
    //
    //     return Result<string>.Error("The bracketed word end character ']' expected.");
    // }
    //
    //
    // private int EscapeSquareBracketedWordChar(StringBuilder wordSb)
    // {
    //     var c = NextChar();
    //     if (c != '[' && c != ']')
    //     {
    //         wordSb.Append('\\');
    //     }
    //
    //     return c;
    // }


    private IResult<IToken> ExtractVariableSubstitution()
    {
        var c = _reader.NextChar();  // Eat '$' 
        if (IsEoF(c))
        {
            return Result<IToken>.Error("Unexpected '$' at the end of the script.");
        }

        var nameSb = new StringBuilder();

        // ${name} = any-char-except '}'
        if (c == '{')
        {
            c = _reader.NextChar();  // Eat '{'.
            while (IsEoF(c) == false)
            {
                if (c == '}')
                {
                    _reader.NextChar();
                    
                    break;
                }

                nameSb.Append((char)c);
                c = _reader.NextChar();
            }
        }
        else
        {
            // $name = $A-Z,a-z,0-9,_
            while (IsEoF(c) == false)
            {
                if (c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_')
                {
                    nameSb.Append((char)c);
                    c = _reader.NextChar();
                    
                    continue;
                }

                break;
            }
        }
        
        return (nameSb.Length == 0)
            ? Result<IToken>.Error("A variable name expected.")
            : Result<IToken>.Ok( new Token(TokenCode.VariableSubstitution, nameSb.ToString()));
    }
}