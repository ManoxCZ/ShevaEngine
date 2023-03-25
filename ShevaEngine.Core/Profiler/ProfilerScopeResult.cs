using System.Collections.Generic;

namespace ShevaEngine.Core.Profiler;

public class ProfilerScopeResult
{
    public string Name { get; }
	public List<ProfilerScopeResult> Children { get; }
    private readonly TimeBuffer _times;
	public double Time => _times.Time;


    public ProfilerScopeResult(string name, int frames)
	{
		Name = name;		
		Children = new ();
        _times = new TimeBuffer(frames);
    }

	public void AddTime(double time)
	{
		_times.Add(time);
	}
}
