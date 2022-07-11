/* TclSharp - (C) 2022 Premysl Fara */

using System.Collections.Generic;

namespace TclSharp.Core.Tests;

using System;

using Xunit;


public class Tokenizer_Tests
{
    #region ctor
    
    [Fact]
    public void NullSourceReaderIsNotValid()
    {
        Assert.Throws<ArgumentNullException>(() => new Tokenizer(null));
    }
    
    [Fact]
    public void EmptySourceIsValid()
    {
        var t = new Tokenizer(new StringSourceReader(string.Empty));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.EoF, getNextTokenResult.Data!.Code);
    }
    
    [Fact]
    public void SourceReaderNextCharIsCalledInNewInstance()
    {
        var reader = new StringSourceReader("test");
        
        Assert.Equal(-1, reader.CurrentChar);
        
        _ = new Tokenizer(reader);

        Assert.Equal('t', reader.CurrentChar);
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
    
    #endregion
    
    
    #region basic tests
    
    [Fact]
    public void FirstNextTokenReturnsFirstToken()
    {
        var t = new Tokenizer(new StringSourceReader("test;"));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);
    }
    
    [Fact]
    public void CurrentTokenIsSetByNextToken()
    {
        var t = new Tokenizer(new StringSourceReader("test;"));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(getNextTokenResult.Data!, t.CurrentToken);
    }
    
    [Fact]
    public void NextTokenReturnsEoFWhenAllTokensAreRead()
    {
        var t = new Tokenizer(new StringSourceReader("test"));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.EoF, getNextTokenResult.Data!.Code);
    }
    
    #endregion


    #region white chars

    [Fact]
    public void OpeningWhiteCharsAreSkipped()
    {
        var t = new Tokenizer(new StringSourceReader("  test"));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);

        var token = getNextTokenResult.Data!;
        
        Assert.Equal(TokenCode.Word, token.Code);
        Assert.Equal("test", token.Children[0].StringValue);
    }
    
    [Fact]
    public void ClosingWhiteCharsAreIgnored()
    {
        var t = new Tokenizer(new StringSourceReader("test \t "));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.EoF, getNextTokenResult.Data!.Code);
    }
    
    [Fact]
    public void WhiteCharsAreSeparatingWords()
    {
        var t = new Tokenizer(new StringSourceReader("test1 test2"));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);
        Assert.Equal("test1", getNextTokenResult.Data!.Children[0].StringValue);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);
        Assert.Equal("test2", getNextTokenResult.Data!.Children[0].StringValue);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.EoF, getNextTokenResult.Data!.Code);
    }

    #endregion


    #region command separators

    [Theory]
    [InlineData(";")]
    [InlineData(" ; ")]
    [InlineData("; ")]
    [InlineData(" ;")]
    public void SemicolonCommandsSeparatorIsFound(string source)
    {
        var t = new Tokenizer(new StringSourceReader(source));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.CommandSeparator, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.EoF, getNextTokenResult.Data!.Code);
    }
    
    [Theory]
    [InlineData("\n")]
    [InlineData(" \n ")]
    [InlineData("\n ")]
    [InlineData(" \n")]
    public void NewLineCommandsSeparatorIsFound(string source)
    {
        var t = new Tokenizer(new StringSourceReader(source));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.CommandSeparator, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.EoF, getNextTokenResult.Data!.Code);
    }
    
    [Theory]
    [InlineData(";;")]
    [InlineData(";\n")]
    [InlineData("\n;")]
    [InlineData("\n\n")]
    [InlineData(" \n; ")]
    [InlineData(" ; ; ")]
    public void MultipleCommandsSeparatorsAreValid(string source)
    {
        var t = new Tokenizer(new StringSourceReader(source));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.CommandSeparator, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.CommandSeparator, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.EoF, getNextTokenResult.Data!.Code);
    }

    [Theory]
    [InlineData("test1;test2")]
    [InlineData("test1 ; test2")]
    [InlineData("test1; test12")]
    [InlineData("test1 ;test2")]
    public void CommandsSeparatorSeparatesCommands(string source)
    {
        var t = new Tokenizer(new StringSourceReader(source));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.CommandSeparator, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);

        getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.EoF, getNextTokenResult.Data!.Code);
    }
    
    #endregion


    #region comments

    [Theory]
    [InlineData("#comment", new[] { TokenCode.EoF }) ]
    [InlineData("# comment", new[] { TokenCode.EoF })]
    [InlineData("#", new[] { TokenCode.EoF })]
    [InlineData("word ; # comment", new[] {TokenCode.Word, TokenCode.CommandSeparator, TokenCode.EoF })]
    [InlineData("word ; # comment\n123", new[] {TokenCode.Word, TokenCode.CommandSeparator, TokenCode.CommandSeparator, TokenCode.Word, TokenCode.EoF })]
    [InlineData("word\n# comment\n123", new[] {TokenCode.Word, TokenCode.CommandSeparator, TokenCode.CommandSeparator, TokenCode.Word, TokenCode.EoF })]
    [InlineData("word-with# word", new[] { TokenCode.Word, TokenCode.Word, TokenCode.EoF })]
    [InlineData("word#with word", new[] { TokenCode.Word, TokenCode.Word, TokenCode.EoF })]
    [InlineData("word #word", new[] { TokenCode.Word, TokenCode.Word, TokenCode.EoF })]
    [InlineData("word # word", new[] { TokenCode.Word, TokenCode.Word, TokenCode.Word, TokenCode.EoF })]
    public void CommentsAreRecognizedAndSkipped(string source, TokenCode[] tokens)
    {
        var t = new Tokenizer(new StringSourceReader(source));
        
        var i = 0;

        var getNextTokenResult = t.NextToken();
        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);

        var tok = getNextTokenResult.Data!;
        while (tok.Code != TokenCode.EoF)
        {
            Assert.Equal(tokens[i], tok.Code);

            i++;

            getNextTokenResult = t.NextToken();
            Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);

            tok = getNextTokenResult.Data!;
        }
    }

    #endregion
    
    
    #region quoted strings

    [Theory]
    [InlineData("\"")]
    [InlineData("\"bla")]
    [InlineData("\" bla")]
    [InlineData("\" \nbla")]
    [InlineData("\" ; bla")]
    [InlineData("\" \\\"; bla")]
    public void UnclosedQuotedWordIsError(string source)
    {
        var t = new Tokenizer(new StringSourceReader(source));

        var getNextTokenResult = t.NextToken();

        Assert.False(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
    }
    
    [Theory]
    [InlineData("\"\"")]
    public void EmptyQuotedWordIsValid(string source)
    {
        var t = new Tokenizer(new StringSourceReader(source));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);
        Assert.Equal(TokenCode.Text, getNextTokenResult.Data!.Children[0].Code);
    }
    
    [Theory]
    [InlineData("\"test1\"", "test1")]
    [InlineData("\"te\\nt1\"", "te\nt1")]
    public void QuotedWordIsExtracted(string source, string expected)
    {
        var t = new Tokenizer(new StringSourceReader(source));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);
        Assert.Equal(TokenCode.Text, getNextTokenResult.Data!.Children[0].Code);
        Assert.Equal(expected, getNextTokenResult.Data!.Children[0].StringValue);
    }

    #endregion
    
    
    #region variable substitutions
    
    [Theory]
    [InlineData("$", "$")]
    [InlineData("$$", "$$")]
    [InlineData("$=", "$=")]
    [InlineData("aa$=", "aa$=")]
    [InlineData("aa$=bb", "aa$=bb")]
    public void NotAVariableNameIsWord(string source, string expected)
    {
        var t = new Tokenizer(new StringSourceReader(source));

        var getNextTokenResult = t.NextToken();

        Assert.True(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.Equal(TokenCode.Word, getNextTokenResult.Data!.Code);
        Assert.Equal(TokenCode.Text, getNextTokenResult.Data!.Children[0].Code);
        Assert.Equal(expected, getNextTokenResult.Data!.Children[0].StringValue);
    }

    [Fact]
    public void MissingClosingBrackedInVariableDefinitionIsError()
    {
        var t = new Tokenizer(new StringSourceReader("${aaa"));

        var getNextTokenResult = t.NextToken();

        Assert.False(getNextTokenResult.IsSuccess, getNextTokenResult.Message);
        Assert.StartsWith("The '}' variable name delimiter in a variable substitution not found", getNextTokenResult.Message);
    }

    #endregion
}
