/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Tests;

using System;
using System.Text;

using Xunit;


public class NewLineEscapingSourceReader_Tests
{
    [Fact]
    public void NullSourceReaderIsNotValid()
    {
        Assert.Throws<ArgumentNullException>(() => new NewLineEscapingSourceReader(null));
    }
    
    [Fact]
    public void CurrentCharIsSetFromInputSourceReaderInConstructor()
    {
        var inputSourceReader = new StringSourceReader("test");
        var c = inputSourceReader.NextChar();

        var sourceReader = new NewLineEscapingSourceReader(inputSourceReader);
        
        Assert.Equal(c, sourceReader.CurrentChar);
    }
    
    [Fact]
    public void NextCharReturnsCharsFromInputSourceReader()
    {
        var inputSourceReader = new StringSourceReader("test");
        var sourceReader = new NewLineEscapingSourceReader(inputSourceReader);

        var c = sourceReader.NextChar();
        
        Assert.Equal(inputSourceReader.CurrentChar, c);
    }
    
    [Theory]
    [InlineData("a\\\nb")]
    [InlineData("a\\\n b")]
    [InlineData("a\\\n \t b")]
    public void NewLineEscapeSequenceIsReplacedWithASingleSpace(string script)
    {
        var inputSourceReader = new StringSourceReader(script);
        var sourceReader = new NewLineEscapingSourceReader(inputSourceReader);

        var sb = new StringBuilder();
        var c = sourceReader.NextChar();
        while (c >= 0)
        {
            sb.Append((char) c);
            
            c = sourceReader.NextChar();
        }
        
        Assert.Equal("a b", sb.ToString());
    }
    
    [Theory]
    [InlineData("a\\n", "a\\n")]
    [InlineData("a\\b", "a\\b")]
    [InlineData("a\\[b", "a\\[b")]
    public void EscapeSequenceIsNotReplaced(string script, string expected)
    {
        var inputSourceReader = new StringSourceReader(script);
        var sourceReader = new NewLineEscapingSourceReader(inputSourceReader);

        var sb = new StringBuilder();
        var c = sourceReader.NextChar();
        while (c >= 0)
        {
            sb.Append((char) c);
            
            c = sourceReader.NextChar();
        }
        
        Assert.Equal(expected, sb.ToString());
    }
}