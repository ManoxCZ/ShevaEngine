using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
    public class Clip
    {
        public double Duration { get; internal set; }
        public Matrix[][] Keyframes { get; private set; }
        public List<AnimationEvent> Events { get; private set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        internal Clip(double duration, Matrix[][] keyframes)
        {
            Duration = duration;
            Keyframes = keyframes;
            Events = new List<AnimationEvent>();
        }
    }
}
