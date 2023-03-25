using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShevaEngine.Core.Profiler;

public class ProfilerService
{
    private const string ROOT_NODE_NAME = "Tick";
    private const int TIME_FRAMES_COUNT = 30;

    private readonly ProfilerScopeResult _rootNode;
    private readonly Stack<ProfilerScopeResult> _nodesStack;
    private readonly Stack<Stopwatch> _stopWatchesPool = new();

    public ProfilerScopeResult RootNode => _rootNode;


    public ProfilerService()
    {
        _rootNode = new(ROOT_NODE_NAME, TIME_FRAMES_COUNT);
        
        _nodesStack= new();
        _nodesStack.Push(_rootNode);     
    }

    public IDisposable BeginScope(string name)
    {
        var actualNode = _nodesStack.Peek();

        var childNode = actualNode.Children.FirstOrDefault(item => item.Name == name);

        if (childNode == null)
        {
            childNode = new (name, TIME_FRAMES_COUNT);

            actualNode.Children.Add(childNode);
        }

        _nodesStack.Push(childNode);

        if (_stopWatchesPool.Count == 0)
        {
            _stopWatchesPool.Push(new Stopwatch());
        }

        return new ProfilerScope(this, _stopWatchesPool.Pop());
    }

    internal void EndScope(Stopwatch stopwatch) 
    {
        _nodesStack.Pop().AddTime(stopwatch.Elapsed.TotalMilliseconds);

        _stopWatchesPool.Push(stopwatch);
    }

    internal void Draw(SpriteBatch spriteBatch)
    {

    }
}
