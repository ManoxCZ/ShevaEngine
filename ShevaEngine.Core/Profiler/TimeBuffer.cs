using System.Linq;

namespace ShevaEngine.Core.Profiler;

internal class TimeBuffer
{
    private double[] _buffer;
    private int _actualId;
    public double Time;


    public TimeBuffer(int bufferSize)
    {
        _buffer = new double[bufferSize];
    }

    public void Add(double value)
    {
        _buffer[_actualId] = value;

        _actualId = (_actualId + 1) % _buffer.Length;

        if (_actualId == 0)
        {
            Time = _buffer.Average();
        }
    }
}
