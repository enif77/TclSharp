/* TclSharp - (C) 2022 Premysl Fara */

namespace TclSharp.Core.Tests;


public class NullOutputWriter : IOutputWriter
{
    public void Write(string format, params object[] arg)
    {
    }


    public void WriteLine()
    {
    }


    public void WriteLine(string format, params object[] arg)
    {
    }
}
