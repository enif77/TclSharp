/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Tests;

using System;

using Xunit;


public class Tokenizer_Tests
{
    [Fact]
    public void NullSourceReaderIsNotValid()
    {
        Assert.Throws<ArgumentNullException>(() => new Tokenizer(null));
    }
    
    [Fact]
    public void EmptySourceIsValid()
    {
        var t = new Tokenizer(new StringSourceReader(string.Empty));

        Assert.Equal(TokenCode.EoF, t.NextToken().Code);
    }
    
    [Fact]
    public void CurrentTokenIsSetInNewInstance()
    {
        var t = new Tokenizer(new StringSourceReader("test"));

        Assert.NotNull(t.CurrentToken);
    }
    
    [Fact]
    public void CurrentTokenIsEoFInNewInstance()
    {
        var t = new Tokenizer(new StringSourceReader("test"));

        Assert.Equal(TokenCode.EoF, t.CurrentToken.Code);
    }
    
    [Fact]
    public void FirstNextTokenReturnsFirstToken()
    {
        var t = new Tokenizer(new StringSourceReader("test;"));

        Assert.Equal(TokenCode.Word, t.NextToken().Code);
    }
    
    [Fact]
    public void CurrentTokenIsSetByNextToken()
    {
        var t = new Tokenizer(new StringSourceReader("test;"));

        var tok = t.NextToken();
        
        Assert.Equal(tok, t.CurrentToken);
    }
    
    [Fact]
    public void NextTokenReturnsEoFWhenAllTokensAreRead()
    {
        var t = new Tokenizer(new StringSourceReader("test"));

        Assert.Equal(TokenCode.Word, t.NextToken().Code);
        Assert.Equal(TokenCode.EoF, t.NextToken().Code);
    }
}
