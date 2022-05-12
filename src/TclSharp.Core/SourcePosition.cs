/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A position in a source.
/// </summary>
public class SourcePosition
{
    public int Line { get; set; } = -1;
    public int Column { get; set; } = -1;
}