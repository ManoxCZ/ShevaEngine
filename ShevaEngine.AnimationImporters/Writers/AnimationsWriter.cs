using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace AnimationImporters
{
    [ContentTypeWriter]
    class AnimationsDataWriter : ContentTypeWriter<AnimationsContent>
    {
        protected override void Write(ContentWriter output, AnimationsContent value)
        {
            WriteClips(output, value.Clips);
            WriteBindPose(output, value.BindPose);
            WriteInvBindPose(output, value.InvBindPose);
            WriteSkeletonHierarchy(output, value.SkeletonHierarchy);
            WriteBoneNames(output, value.BoneNames);
        }

        private void WriteClips(ContentWriter output, Dictionary<string, ClipContent> clips)
        {
            int count = clips.Count;
            output.Write(count);

            foreach (var clip in clips)
            {
                output.Write(clip.Key);
                output.WriteObject(clip.Value);
            }

            return;
        }

        private void WriteBindPose(ContentWriter output, List<Microsoft.Xna.Framework.Matrix> bindPoses)
        {
            int count = bindPoses.Count;
            output.Write((int)count);

            for (int i = 0; i < count; i++)
                output.Write(bindPoses[i]);

            return;
        }

        private void WriteInvBindPose(ContentWriter output, List<Microsoft.Xna.Framework.Matrix> invBindPoses)
        {
            int count = invBindPoses.Count;
            output.Write(count);

            for (int i = 0; i < count; i++)
                output.Write(invBindPoses[i]);

            return;
        }

        private void WriteSkeletonHierarchy(ContentWriter output, List<int> skeletonHierarchy)
        {
            int count = skeletonHierarchy.Count;
            output.Write(count);

            for (int i = 0; i < count; i++)
                output.Write(skeletonHierarchy[i]);

            return;
        }

        private void WriteBoneNames(ContentWriter output, List<string> boneNames)
        {
            int count = boneNames.Count;
            output.Write(count);

            for (int boneIndex = 0; boneIndex < count; boneIndex++)
            {
                var boneName = boneNames[boneIndex];
                output.Write(boneName);
            }

            return;
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "ShevaEngine.Core.Animations, ShevaEngine.Core";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "ShevaEngine.Core.AnimationsReader, ShevaEngine.Core";
        }
    }
}
