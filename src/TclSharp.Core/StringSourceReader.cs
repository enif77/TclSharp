/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Reads a source from a string.
/// </summary>
public class StringSourceReader : ISourceReader
{
    public int CurrentChar { get; private set; }


    public StringSourceReader(string src)
    {
        _src = src ?? throw new ArgumentNullException(nameof(src));
        _sourcePosition = -1;
        CurrentChar = -1;
    }


    public int NextChar()
    {
        _sourcePosition++;
        if (_sourcePosition < _src.Length)
        {
            return CurrentChar = _src[_sourcePosition];
        }
        
        _sourcePosition = _src.Length;

        return CurrentChar = -1;
    }


    private readonly string _src;
    private int _sourcePosition;
}