/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core;


/// <summary>
/// A token.
/// </summary>
public class Token : IToken
{
    public TokenCode Code { get; }
    public string StringValue { get; }
    public IList<IToken> Children { get; }


    private Token(TokenCode tokenCode, string stringValue = "")
    {
        Code = tokenCode;
        StringValue = stringValue;
        Children = new List<IToken>();
    }


    public static IToken EofToken()
        => new Token(TokenCode.EoF);
    
    
    public static IToken CommandSeparatorToken()
    => new Token(TokenCode.CommandSeparator);
    
    
    public static IToken TextToken(string text)
        => new Token(TokenCode.Text, text);
    
    
    public static IToken WordToken()
        => new Token(TokenCode.Word, "word");

    
    public static IToken WordToken(string text)
        => WordToken(TextToken(text));

    
    private static IToken WordToken(IToken child)
    {
        var tok = WordToken();
        
        tok.Children.Add(child);

        return tok;
    }
    
    
    public static IToken CommandSubstitutionToken(string commandSubstitution)
        => new Token(TokenCode.CommandSubstitution, commandSubstitution);
    
    
    public static IToken VariableSubstitutionToken(string variableName)
        => new Token(TokenCode.VariableSubstitution, variableName);
}