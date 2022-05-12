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

        Assert.Equal(0, r.NextChar());
    }
    
    [Fact]
    public void CurrentCharIsZeroInNewInstance()
    {
        var r = new StringSourceReader("test");

        Assert.Equal(0, r.CurrentChar);
    }
    
    [Fact]
    public void NextCharReturnsSourceChar()
    {
        var r = new StringSourceReader("test");

        Assert.Equal('t', r.NextChar());
    }
    
    [Fact]
    public void CurrentCharIsSetByNextChar()
    {
        var r = new StringSourceReader("test");

        var c = r.NextChar();
        
        Assert.Equal(c, r.CurrentChar);
    }
    
    [Fact]
    public void CurrentCharIsZeroWhenAllCharsAreRead()
    {
        var r = new StringSourceReader("t");

        Assert.NotEqual(0, r.NextChar());  // 't'
        
        r.NextChar();
        
        Assert.Equal(0, r.CurrentChar);    // 0
    }
    
    [Fact]
    public void NextCharReturnsZeroWhenAllCharsAreRead()
    {
        var r = new StringSourceReader("t");

        Assert.NotEqual(0, r.NextChar());  // 't'
        
        Assert.Equal(0, r.NextChar());     // 0
    }
}
