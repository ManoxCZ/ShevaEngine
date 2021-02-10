using Microsoft.Xna.Framework;

namespace ShevaEngine.Core
{
	public class Clip
    {
        public double Duration { get; internal set; }
        public Matrix[][] Keyframes { get; private set; }

        internal Clip(double duration, Matrix[][] keyframes)
        {
            Duration = duration;
            Keyframes = keyframes;
        }
    }
}
