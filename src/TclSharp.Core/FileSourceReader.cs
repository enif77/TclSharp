/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Reads a source from a file.
/// Converts CRLF to LF.
/// </summary>
public class FileSourceReader : ISourceReader
{
    public int CurrentChar { get; private set; }


    public FileSourceReader(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
        _src = Array.Empty<string>();
        _currentLinePosition = -1;
        _currentLineIndex = -1;
        CurrentChar = -1;
    }


    public int NextChar()
    {
        if (_currentLineIndex < 0)
        {
            _src = File.ReadAllLines(_path);
            _currentLineIndex = 0;
        }

        if (_currentLineIndex >= _src.Length)
        {
            _currentLineIndex = _src.Length;

            return CurrentChar = -1;
        }

        var currentLine = _src[_currentLineIndex];
        
        _currentLinePosition++;
        if (_currentLinePosition < currentLine.Length)
        {
            return CurrentChar = currentLine[_currentLinePosition];
        }

        _currentLineIndex++;
        if (_currentLineIndex < _src.Length)
        {
            _currentLinePosition = -1;

            return CurrentChar = '\n';
        }
       
        return CurrentChar = -1;
    }


    private readonly string _path;
    private string[] _src;
    private int _currentLinePosition;
    private int _currentLineIndex;
}