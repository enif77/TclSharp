/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Tests;

using Xunit;

using TclSharp.Core.CommandArgumentValues;


public class VariableSubstitutionCommandArgumentValue_Tests
{
    private const string TestVarName = "testvar";
    private const string TestVarValue = "test";
    
    
    [Fact]
    public void ValueIsSetByConstructor()
    {
        var av = new VariableSubstitutionCommandArgumentValue(TestVarName);
        
        Assert.Equal(TestVarName, av.Value);
    }
    
    [Fact]
    public void GetValueFromNonExistingVariableIsError()
    {
        var av = new VariableSubstitutionCommandArgumentValue("nosuchvar");

        var getVariableResult = av.Interpret(new Interpreter(new NullOutputWriter()));
        
        Assert.False(getVariableResult.IsSuccess);
        Assert.Contains("no such variable", getVariableResult.Message);
    }
    
    
    [Fact]
    public void ValueFromExistingVariableIsReturned()
    {
        var i = new Interpreter(new NullOutputWriter());
        
        i.SetVariableValue(TestVarName, TestVarValue);
        
        var av = new VariableSubstitutionCommandArgumentValue(TestVarName);

        var getVariableResult = av.Interpret(i);
        
        Assert.True(getVariableResult.IsSuccess);
        Assert.Equal(TestVarValue, getVariableResult.Data);
    }
}