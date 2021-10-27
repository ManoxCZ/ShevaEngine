using Microsoft.Xna.Framework;
using System;

namespace ShevaEngine.Core
{
    public struct Keyframe
    {
        internal int _bone;
        internal double _time;
        internal Matrix _transform;

        public int Bone
        {
            get { return _bone; }
            internal set { _bone = value; }
        }

        public double Time
        {
            get { return _time; }
            internal set { _time = value; }
        }

        public Matrix Transform
        {
            get { return _transform; }
            internal set { _transform = value; }
        }

        public Keyframe(int bone, double time, Matrix transform)
        {
            _bone = bone;
            _time = time;
            _transform = transform;
        }

        public override string ToString()
        {
            return string.Format("{{Time:{0} Bone:{1}}}",
                new object[] { Time, Bone });
        }
    }
}
