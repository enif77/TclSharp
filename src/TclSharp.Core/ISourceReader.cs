/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Defines a source reader.
/// </summary>
public interface ISourceReader
{
    /// <summary>
    /// The last read (the current) character.
    /// </summary>
    int CurrentChar { get; }
    
    
    /// <summary>
    /// Reads the next character from the source.
    /// </summary>
    /// <returns>The next character from the source or -1 (EOF) at the end of the source.</returns>
    int NextChar();
}