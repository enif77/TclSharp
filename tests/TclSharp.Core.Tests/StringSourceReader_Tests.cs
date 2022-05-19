/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Tests;

using System;

using Xunit;


public class StringSourceReader_Tests
{
    [Fact]
    public void NullSourceIsNotValid()
    {
        Assert.Throws<ArgumentNullException>(() => new StringSourceReader(null));
    }
    
    [Fact]
    public void EmptySourceIsValid()
    {
        var r = new StringSourceReader(string.Empty);

        Assert.Equal(-1, r.NextChar());
    }
    
    [Fact]
    public void CurrentCharIsZeroInNewInstance()
    {
        var r = new StringSourceReader("test");

        Assert.Equal(0, r.CurrentChar);
    }
    
    [Fact]
    public void FirstNextCharReturnsFirstSourceChar()
    {
        var r = new StringSourceReader("tx");

        Assert.Equal('t', r.NextChar());
    }
    
    [Fact]
    public void CurrentCharIsSetByNextChar()
    {
        var r = new StringSourceReader("tx");

        var c = r.NextChar();
        
        Assert.Equal(c, r.CurrentChar);
    }
    
    [Fact]
    public void NextCharReturnsLessThanZeroWhenAllCharsAreRead()
    {
        var r = new StringSourceReader("t");

        Assert.Equal('t', r.NextChar());
        Assert.Equal(-1, r.NextChar());
    }
}
