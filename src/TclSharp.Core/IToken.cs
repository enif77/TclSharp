/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// Known token codes.
/// </summary>
public enum TokenCode
{
    /// <summary>
    /// An unknown token.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// The end of file (source).
    /// </summary>
    EoF,
    
    /// <summary>
    /// '\n' or ';'.
    /// </summary>
    CommandSeparator,
    
    /// <summary>
    /// A word (a command name or argument) and "quoted word".
    /// </summary>
    Word,
    
    /// <summary>
    /// A {braced word}, that does not require any substitution.
    /// </summary>
    RawWord
}


/// <summary>
/// Represents a token extracted from a source. 
/// </summary>
public interface IToken
{
    /// <summary>
    /// A code defining this token.
    /// </summary>
    TokenCode Code { get; }

    /// <summary>
    /// An optional string value of this token (a word, name, number, ...).  
    /// </summary>
    string StringValue { get; }
}