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
    char CurrentChar { get; }
    
    
    /// <summary>
    /// Reads the next character from the input.
    /// </summary>
    /// <returns>The next character from the input or 0 at the end of the source.</returns>
    char NextChar();
}