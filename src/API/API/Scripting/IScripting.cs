using System;

namespace API.Scripting
{
    public interface IScripting
    {
        Action<string> RunFile { get; set; }
    }
}
