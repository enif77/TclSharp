/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A token.
/// </summary>
public class Token : IToken
{
    public TokenCode Code { get; }
    public string StringValue { get; }
    
    
    public Token(TokenCode tokenCode, string stringValue = "")
    {
        Code = tokenCode;
        StringValue = stringValue;
    }
}