using System;

namespace ShevaEngine.Core.Profiler;

public class ProfilerService
{ 

    public IDisposable BeginScope(string name)
    {
        return new ProfilerScope(this, name);
    }

    internal void EndScope() 
    {

    }
}
