using Microsoft.Xna.Framework;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Animations controller.
	/// </summary>
	public class AnimationsController
	{
		private readonly Log _log = new Log(typeof(Animations));
		private readonly Animations _animations;
		public Clip CurrentClip { get; private set; }
		private Matrix[] _boneTransforms;
		private Matrix[] _worldTransforms;
		private Matrix[] _animationTransforms;

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
		/// Set clip.
		/// </summary>		
		public void SetClip(string clipName)
		{
			if (!_animations.Clips.ContainsKey(clipName))
			{
				_log.Error($"Can't find clip {clipName}");

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
				_log.Error("Clip is null");

				return;
			}

			CurrentClip = clip;

			_animations.BindPose.CopyTo(_boneTransforms, 0);
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
