/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;

using TclSharp.Core.Results;


/// <summary>
/// Defines a parser.
/// </summary>
public interface IParser
{
    /// <summary>
    /// Parses a source into an IScript instance.
    /// </summary>
    /// <param name="sourceReader">An ISourceReader instance.</param>
    /// <returns>An IResult instance representing the parsing result with parsed script as data.</returns>
    IResult<IScript> Parse(ISourceReader sourceReader);
}
