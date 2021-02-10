using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Animations.
	/// </summary>
	public class Animations
    {
		private readonly Log _log = new Log(typeof(Animations));

        public List<Matrix> BindPose;
		public List<Matrix> InvBindPose;
		public List<int> SkeletonHierarchy;
		public Dictionary<string, int> BoneMap;				
        public Dictionary<string, Clip> Clips { get; private set; }
        

		/// <summary>
		/// Constructor.
		/// </summary>
		public Animations(List<Matrix> bindPose, List<Matrix> invBindPose, 
			List<int> skeletonHierarchy, Dictionary<string, int> boneMap, Dictionary<string, Clip> clips)
        {
            BindPose = bindPose;
            InvBindPose = invBindPose;
            SkeletonHierarchy = skeletonHierarchy;
            BoneMap = boneMap;
			Clips = clips;
        }

		/// <summary>
		/// Add clips.
		/// </summary>		
		public void AddClips(Animations animations)
		{
			foreach (KeyValuePair<string, Clip> item in animations.Clips)
				Clips.Add(item.Key, item.Value);
		}

		/// <summary>
		/// Get new controller.
		/// </summary>
		/// <returns></returns>
		public AnimationsController GetNewController()
		{
			return new AnimationsController(this);
		}			
    }
}
