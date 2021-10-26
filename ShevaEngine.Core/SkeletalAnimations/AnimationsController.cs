using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Reactive.Subjects;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Animations controller.
	/// </summary>
	public class AnimationsController : IDisposable
	{
		private readonly ILogger _log = ShevaGame.Instance.LoggerFactory.CreateLogger<AnimationsController>();
		private readonly Animations _animations;
		public Clip CurrentClip { get; private set; }
		private Matrix[] _boneTransforms;
		private Matrix[] _worldTransforms;
		private Matrix[] _animationTransforms;
        private int _previousFrame;
        public Subject<AnimationEvent> Events { get; } = new Subject<AnimationEvent>();

		/// <summary>
		/// Constructor.
		/// </summary>		
		internal AnimationsController(Animations animations)
		{
			_animations = animations;

			_boneTransforms = new Matrix[_animations.BindPose.Count];
			_worldTransforms = new Matrix[_animations.BindPose.Count];
			_animationTransforms = new Matrix[_animations.BindPose.Count];
		}

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Events?.Dispose();
        }

		/// <summary>
		/// Set clip.
		/// </summary>		
		public void SetClip(string clipName)
		{
			if (!_animations.Clips.ContainsKey(clipName))
			{
				_log.LogError($"Can't find clip {clipName}");

				return;
			}

			SetClip(_animations.Clips[clipName]);
		}

		/// <summary>
		/// Set clip.
		/// </summary>		
		public void SetClip(Clip clip)
		{
			if (clip == null)
			{
				_log.LogError("Clip is null");

				return;
			}

			CurrentClip = clip;

			_animations.BindPose.CopyTo(_boneTransforms, 0);
		}

        /// <summary>
		/// Get transforms.
		/// </summary>
		public void UpdateEvents(GameTime time)
        {
            double currentTime = time.TotalGameTime.TotalSeconds % CurrentClip.Duration;

            int frame = (int)(currentTime * 30);
            
            foreach (AnimationEvent animEvent in CurrentClip.Events)
                if (animEvent.KeyFrameId > _previousFrame && animEvent.KeyFrameId <= frame)
                    Events.OnNext(animEvent);

            _previousFrame = frame;            
        }

        /// <summary>
        /// Get transforms.
        /// </summary>
        public Matrix[] GetTransforms(GameTime time)
		{
			double currentTime = time.TotalGameTime.TotalSeconds % CurrentClip.Duration;            

			int frame = (int)(currentTime * 30) % CurrentClip.Keyframes.Length;

			CurrentClip.Keyframes[frame].CopyTo(_boneTransforms, 0);

			_worldTransforms[0] = _boneTransforms[0];

			for (int bone = 1; bone < _worldTransforms.Length; bone++)
				_worldTransforms[bone] = Matrix.Multiply(_boneTransforms[bone], _worldTransforms[_animations.SkeletonHierarchy[bone]]);

			for (int bone = 0; bone < _animationTransforms.Length; bone++)
				_animationTransforms[bone] = Matrix.Multiply(_animations.InvBindPose[bone], _worldTransforms[bone]);            

			return _animationTransforms;
		}
	}
}
