using System;
using System.Diagnostics;

namespace ShevaEngine.Core.Profiler;

internal struct ProfilerScope : IDisposable
{	
	private readonly ProfilerService _profilerService;
	private readonly Stopwatch _stopwatch;


	public ProfilerScope(ProfilerService profilerService, Stopwatch stopwatch)
	{
		_profilerService = profilerService;
		_stopwatch = stopwatch;

		_stopwatch.Restart();
	}

	public void Dispose()
	{
		_stopwatch.Stop();

		_profilerService.EndScope(_stopwatch);
	}
}
