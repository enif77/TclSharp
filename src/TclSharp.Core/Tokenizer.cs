﻿/* TclSharp - (C) 2022 Premysl Fara */

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


    public IResult<IToken> NextToken()
    {
        IToken? wordTok = null;
        StringBuilder? wordPartSb = null;
        
        // At what char we were the last time?
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

            if (IsCommandsSeparator(c))
            {
                // Are we extracting a word now?
                if (wordPartSb == null && wordTok == null)
                {
                    // No, so consume the command separator char...
                    _reader.NextChar();
                        
                    // and return the EofC token.
                    return Result<IToken>.Ok(CurrentToken = _commandSeparatorToken);
                }
                
                // A commands separator ends a word. We'll return it below.
                break;
            }

            if (IsCommentStart(c))
            {
                // Are we extracting a word now?
                if (wordPartSb == null && wordTok == null)
                {
                    c = SkipComment();

                    continue;
                }
                
                // A '#' character inside of a word has no special meaning.
            }

            switch (c)
            {
                case '"':
                    if (wordPartSb == null)
                    {
                        return ExtractQuotedWord();
                    }
                    wordPartSb.Append((char) c);
                    break;
                
                case '{':
                    if (wordPartSb == null)
                    {
                        return ExtractBracketedWord();
                    }
                    wordPartSb.Append((char) c);
                    break;
                
                case '[':
                    wordPartSb = AppendCollectedWordPart(wordPartSb, ref wordTok);
                    
                    var extractCommandSubstitutionResult = ExtractCommandSubstitution();
                    if (extractCommandSubstitutionResult.IsSuccess == false)
                    {
                        return extractCommandSubstitutionResult;
                    }
                    wordTok ??= Token.WordToken();
                    wordTok.Children.Add(extractCommandSubstitutionResult.Data!);
                    c = _reader.CurrentChar;
                    continue;
                
                case '$':
                    var nextChar = _reader.NextChar();
                    if (nextChar != '{' && IsVariableNameCharacter(nextChar) == false)
                    {
                        wordPartSb ??= new StringBuilder();
                        wordPartSb.Append((char)c);  // '$'

                        c = nextChar;

                        continue;
                    }

                    wordPartSb = AppendCollectedWordPart(wordPartSb, ref wordTok);
                    
                    var extractVariableSubstitutionResult = ExtractVariableSubstitution();
                    if (extractVariableSubstitutionResult.IsSuccess == false)
                    {
                        return extractVariableSubstitutionResult;
                    }
                    wordTok ??= Token.WordToken();
                    wordTok.Children.Add(extractVariableSubstitutionResult.Data!);
                    c = _reader.CurrentChar;
                    continue;
                
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
            return Result<IToken>.Ok(CurrentToken = wordTok ?? _eofToken);
        }

        wordTok ??= Token.WordToken();
        wordTok.Children.Add(Token.TextToken(wordPartSb.ToString()));
        
        return Result<IToken>.Ok(CurrentToken = wordTok);
    }

    
    private readonly ISourceReader _reader;
    
    private readonly IToken _eofToken = Token.EofToken();
    private readonly IToken _commandSeparatorToken = Token.CommandSeparatorToken();


    private static bool IsEoF(int c)
        => c < 0;
    
    
    private static bool IsWhiteSpace(int c)
        => IsCommandsSeparator(c) == false && char.IsWhiteSpace((char)c);

    
    private static bool IsWordEnd(int c)
        => IsEoF(c) || IsCommandsSeparator(c) || IsWhiteSpace(c);
    
    
    private static bool IsCommandsSeparator(int c)
        => c is '\n' or ';';

    
    private static bool IsVariableNameCharacter(int c)
        => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_';

    
    private static bool IsCommentStart(int c)
        => c is '#';
    
    
    private static bool IsCommentEnd(int c)
        => c == '\n' || IsEoF(c);

    
    private int SkipComment()
    {
        // Consume the comment start char...
        var c = _reader.NextChar();

        // and skip all chars till the nearest EoLN or EoF.
        while (IsCommentEnd(c) == false)
        {
            c = _reader.NextChar();
        }

        return c;
    }
    

    private IResult<IToken> ExtractQuotedWord()
    {
        var wordTok = Token.WordToken();
        var wordPartSb = new StringBuilder();
        
        var c = _reader.NextChar();
        while (IsEoF(c) == false)
        {
            switch (c)
            {
                case '\\':
                    c = EscapeQuotedWordChar();
                    break;
                
                case '$':
                    var nextChar = _reader.NextChar();
                    if (nextChar != '{' && IsVariableNameCharacter(nextChar) == false)
                    {
                        wordPartSb!.Append((char)c);  // '$'

                        c = nextChar;

                        continue;
                    }

                    // Add the "prefix" as a child token to the current word.
                    wordPartSb = AppendCollectedWordPart(wordPartSb, ref wordTok);
                    
                    var extractVariableSubstitutionResult = ExtractVariableSubstitution();
                    if (extractVariableSubstitutionResult.IsSuccess == false)
                    {
                        return extractVariableSubstitutionResult;
                    }
                    wordTok!.Children.Add(extractVariableSubstitutionResult.Data!);
                    
                    // Here we are at char behind the variable substitution.
                    // We want to process it as if the variable substitution was not here...
                    c = _reader.CurrentChar;
                    continue;
                
                case '[':
                    wordPartSb = AppendCollectedWordPart(wordPartSb, ref wordTok);
                    
                    var extractCommandSubstitutionResult = ExtractCommandSubstitution();
                    if (extractCommandSubstitutionResult.IsSuccess == false)
                    {
                        return extractCommandSubstitutionResult;
                    }
                    wordTok!.Children.Add(extractCommandSubstitutionResult.Data!);
                    c = _reader.CurrentChar;
                    continue;
                
                case '"':
                    if (IsWordEnd(_reader.NextChar()) == false)
                    {
                        return Result<IToken>.Error("An EoF, words or commands separator expected after the quoted word.");
                    }

                    // Add the chars before the closing " as a child token to the current word.
                    _ = AppendCollectedWordPart(wordPartSb, ref wordTok);

                    if (wordTok!.Children.Count == 0)
                    {
                        // Add the "" as a child token to the current word.
                        wordTok.Children.Add(Token.TextToken(string.Empty));
                    }
                    return Result<IToken>.Ok(CurrentToken = wordTok);
            }

            wordPartSb!.Append((char) c);
            
            c = _reader.NextChar();
        }

        return Result<IToken>.Error("The quoted word end character '\"' expected.");
    }
    
    
    private int EscapeQuotedWordChar()
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
                            ? Result<IToken>.Ok(CurrentToken = Token.WordToken(wordSb.ToString()))
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
    
    
    private IResult<IToken> ExtractCommandSubstitution()
    {
        var wordPartSb = new StringBuilder();

        var inQuotedWord = false;
        var bracketLevel = 1;
        var c = _reader.NextChar();
        while (IsEoF(c) == false)
        {
            switch (c)
            {
                case '\\':
                    // This allows to parse the "x\"x" string to the x"x string. 
                    wordPartSb.Append('\\');
                    c = _reader.NextChar();
                    break;
                
                case '"':
                    inQuotedWord = !inQuotedWord;
                    break;
                
                case '[':
                    if (inQuotedWord == false)
                    {
                        bracketLevel++;    
                    }
                    break;
                
                case ']':
                {
                    if (inQuotedWord == false)
                    {
                        bracketLevel--;
                        if (bracketLevel == 0)
                        {
                            // Eat ']'.
                            _reader.NextChar();
                        
                            return Result<IToken>.Ok(CurrentToken = Token.CommandSubstitutionToken(wordPartSb.ToString()));
                        }
                    }
                    break;
                }
            }
           
            wordPartSb.Append((char) c);
            
            c = _reader.NextChar();
        }
    
        return Result<IToken>.Error("The command substitution end character ']' expected.");
    }


    private IResult<IToken> ExtractVariableSubstitution()
    {
        var c = _reader.CurrentChar;

        var nameSb = new StringBuilder();

        // ${name} = any-char-except '}'
        if (c == '{')
        {
            var closingBracketFound = false;
            c = _reader.NextChar();  // Eat '{'.
            while (IsEoF(c) == false)
            {
                if (c == '}')
                {
                    closingBracketFound = true;
                    _reader.NextChar();  // Eat '}'.

                    break;
                }

                nameSb.Append((char)c);
                c = _reader.NextChar();
            }

            if (IsEoF(c) && closingBracketFound == false)
            {
                return Result<IToken>.Error("The '}' variable name delimiter in a variable substitution not found.");
            }
        }
        else
        {
            // $name = $A-Z,a-z,0-9,_
            while (IsEoF(c) == false)
            {
                if (IsVariableNameCharacter(c))
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
            : Result<IToken>.Ok(CurrentToken = Token.VariableSubstitutionToken(nameSb.ToString()));
    }
    
    
    private static StringBuilder? AppendCollectedWordPart(StringBuilder? wordPartSb, ref IToken? wordTok)
    {
        if (wordPartSb == null || wordPartSb.Length == 0)
        {
            return wordPartSb;
        }
        
        wordTok ??= Token.WordToken();
        wordTok.Children.Add(Token.TextToken(wordPartSb.ToString()));
        
        return new StringBuilder();
    }
}