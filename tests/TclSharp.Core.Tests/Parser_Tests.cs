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
        Assert.NotNull(s!.Commands);
        Assert.Empty(s.Commands);
    }
    
    [Fact]
    public void EmptyCommandIsValid()
    {
        var p = new Parser();

        Assert.True(p.Parse(new StringSourceReader(";")).IsSuccess);
    }
    
    [Fact]
    public void EmptyCommandProducesNoScriptCommand()
    {
        var p = new Parser();

        var s = p.Parse(new StringSourceReader(";")).Data;
        
        Assert.Empty(s!.Commands);
    }
    
    [Fact]
    public void SimpleWordCommandProducesSingleScriptCommand()
    {
        var p = new Parser();

        var s = p.Parse(new StringSourceReader("test")).Data;
        
        Assert.NotNull(s);
        Assert.Equal(1, s!.Commands.Count);
    }
    
    [Fact]
    public void MultipleWordCommandProducesSingleScriptCommandWithArguments()
    {
        var p = new Parser();

        var s = p.Parse(new StringSourceReader("test arg1 arg2")).Data;
        
        Assert.Equal(1, s!.Commands.Count);
        Assert.Equal(3, s.Commands[0].Arguments.Count);
    }
    
    
    [Fact]
    public void FirstWordIsScriptCommandNameAndFirstArgument()
    {
        var p = new Parser();

        var s = p.Parse(new StringSourceReader("test")).Data;
        
        Assert.Equal("test", s!.Commands[0].Name);
        Assert.Equal("test", s.Commands[0].Arguments[0].Value);
    }
    
    [Fact]
    public void MultipleWordCommandProducesMultipleScriptCommand()
    {
        var p = new Parser();

        var s = p.Parse(new StringSourceReader("test1;test2")).Data;
        
        Assert.NotNull(s);
        Assert.Equal(2, s!.Commands.Count);
    }
    
    [Fact]
    public void SemicolonIsCommandSeparator()
    {
        var p = new Parser();

        var s = p.Parse(new StringSourceReader("test1;test2")).Data;
        
        Assert.NotNull(s);
        Assert.Equal(2, s!.Commands.Count);
    }
    
    [Fact]
    public void NewlineIsCommandSeparator()
    {
        var p = new Parser();

        var s = p.Parse(new StringSourceReader("test1\ntest2")).Data;
        
        Assert.NotNull(s);
        Assert.Equal(2, s!.Commands.Count);
    }
    
    [Fact]
    public void EscapedNewlineIsNotCommandSeparator()
    {
        var p = new Parser();

        var s = p.Parse(new StringSourceReader("test1\\\ntest2")).Data;
        
        Assert.NotNull(s);
        Assert.Equal(1, s!.Commands.Count);
    }
}
