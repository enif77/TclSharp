/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Reads a source from a sourceReader and escapes the \nl-white-chars escape sequence.
/// </summary>
public class NewLineEscapingSourceReader : ISourceReader
{
    public int CurrentChar { get; private set; }


    public NewLineEscapingSourceReader(ISourceReader sourceReader)
    {
        _sourceReader = sourceReader ?? throw new ArgumentNullException(nameof(sourceReader));
        
        CurrentChar = _sourceReader.CurrentChar;
    }


    public int NextChar()
    {
        if (_nextCharBuffer >= 0)
        {
            var nextChar = _nextCharBuffer;
            _nextCharBuffer = -1;

            return CurrentChar = nextChar;
        }

        var c = _sourceReader.NextChar();
        if (c != '\\')
        {
            return CurrentChar = c;
        }

        c = _sourceReader.NextChar();
        if (c != '\n')
        {
            _nextCharBuffer = c;

            return CurrentChar = '\\';
        }

        c = _sourceReader.NextChar();
        while (IsWhiteSpace(c))
        {
            c = _sourceReader.NextChar();
        }

        _nextCharBuffer = c;

        return CurrentChar = ' ';
    }

    
    private readonly ISourceReader _sourceReader;
    private int _nextCharBuffer = -1;
    
    
    private static bool IsWhiteSpace(int c)
        => IsWordsSeparator(c) == false && char.IsWhiteSpace((char)c);

   
    private static bool IsWordsSeparator(int c)
        => c is '\n' or ';';
}