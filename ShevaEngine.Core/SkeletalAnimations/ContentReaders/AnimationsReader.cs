using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Animations reader.
    /// </summary>
    public class AnimationsReader : ContentTypeReader<Animations>
    {
        /// <summary>
        /// Read.
        /// </summary>
        protected override Animations Read(ContentReader input, Animations existingInstance)
        {
            if (existingInstance == null)
            {
                Dictionary<string, Clip> clips = ReadAnimationClips(input);
                List<Matrix> bindPose = ReadBindPose(input);
                List<Matrix> invBindPose = ReadInvBindPose(input);
                List<int> skeletonHierarchy = ReadSkeletonHierarchy(input);
                Dictionary<string, int> boneMap = ReadBoneMap(input);

                return new Animations(bindPose, invBindPose, skeletonHierarchy, boneMap, clips);
            }

            return new Animations(
                existingInstance.BindPose,
                existingInstance.InvBindPose,
                existingInstance.SkeletonHierarchy,
                existingInstance.BoneMap,
                existingInstance.Clips);
        }

        /// <summary>
        /// Read animation clips.
        /// </summary>
        private Dictionary<string, Clip> ReadAnimationClips(ContentReader input)
        {
            Dictionary<string, Clip> animationClips = new Dictionary<string, Clip>();

            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
                animationClips.Add(input.ReadString(), input.ReadObject<Clip>());

            return animationClips;
        }

        /// <summary>
        /// Read bind pose.
        /// </summary>
        private List<Matrix> ReadBindPose(ContentReader input)
        {
            List<Matrix> bindPose = new List<Matrix>();

            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
                bindPose.Add(input.ReadMatrix());

            return bindPose;
        }

        /// <summary>
        /// Read inv bind pose.
        /// </summary>
        private List<Matrix> ReadInvBindPose(ContentReader input)
        {
            List<Matrix> invBindPose = new List<Matrix>();

            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
                invBindPose.Add(input.ReadMatrix());

            return invBindPose;
        }

        /// <summary>
        /// Read skeleton hierarchy.
        /// </summary>
        private List<int> ReadSkeletonHierarchy(ContentReader input)
        {
            List<int> skeletonHierarchy = new List<int>();

            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
                skeletonHierarchy.Add(input.ReadInt32());

            return skeletonHierarchy;
        }

        /// <summary>
        /// Read bone map.
        /// </summary>
        private Dictionary<string, int> ReadBoneMap(ContentReader input)
        {
            Dictionary<string, int> boneMap = new Dictionary<string, int>();

            int count = input.ReadInt32();

            for (int boneIndex = 0; boneIndex < count; boneIndex++)
                boneMap.Add(input.ReadString(), boneIndex);

            return boneMap;
        }
    }
}
