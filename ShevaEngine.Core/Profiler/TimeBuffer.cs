using System.Linq;

namespace ShevaEngine.Core.Profiler;

internal class TimeBuffer
{
    private double[] _buffer;
    private int _actualId;


    public TimeBuffer(int bufferSize)
    {
        _buffer= new double[bufferSize];
    }

    public void Add(double value) 
    {
        _buffer[_actualId] = value;

        _actualId = _actualId++ % _buffer.Length;
    }


    public double Average()
    {
        return _buffer.Average();
    }
}
