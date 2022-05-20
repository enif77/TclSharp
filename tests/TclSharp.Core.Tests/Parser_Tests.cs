/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Tests;

using System;

using Xunit;


public class Parser_Tests
{
    [Fact]
    public void NullSourceReaderIsNotValid()
    {
        var p = new Parser();
        
        Assert.Throws<ArgumentNullException>(() => p.Parse(null));
    }
    
    [Fact]
    public void EmptySourceIsValid()
    {
        var p = new Parser();

        Assert.True(p.Parse(new StringSourceReader(string.Empty)).IsSuccess);
    }
    
    [Fact]
    public void EmptySourceIsParsedToEmptyScript()
    {
        var p = new Parser();
        var s = p.Parse(new StringSourceReader(string.Empty)).Data;

        Assert.NotNull(s);
        Assert.NotNull(s.Commands);
        Assert.Empty(s.Commands);
    }
}
