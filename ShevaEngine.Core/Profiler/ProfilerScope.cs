using System;

namespace ShevaEngine.Core.Profiler;

internal struct ProfilerScope : IDisposable
{
	public ProfilerService _profilerService;
    public readonly string Name;
	public readonly DateTime StartTime;


	public ProfilerScope(ProfilerService profilerService, string name)
	{
		_profilerService = profilerService;
		Name = name;
		StartTime = DateTime.Now;
	}

	public void Dispose()
	{
		_profilerService.EndScope();
	}
}
