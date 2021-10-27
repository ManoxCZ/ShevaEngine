using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShevaEngine.Core
{
    public class ClipReader : ContentTypeReader<Clip>
    {
        protected override Clip Read(ContentReader input, Clip existingInstance)
        {
            Clip animationClip = existingInstance;

            if (existingInstance == null)
            {
                double duration = input.ReadDouble();
                Matrix[][] keyframes = ReadKeyframes(input, null);
                animationClip = new Clip(duration, keyframes);
            }
            else
            {
                animationClip.Duration = input.ReadDouble();
                ReadKeyframes(input, animationClip.Keyframes);
            }

            return animationClip;
        }

        private Matrix[][] ReadKeyframes(ContentReader input, Matrix[][] existingInstance)
        {
            Matrix[][] keyframes = existingInstance;

            int keyframesCount = input.ReadInt32();
            int channelsCount = input.ReadInt32();

            if (keyframes == null)
                keyframes = new Matrix[keyframesCount][];

            for (int i = 0; i < keyframesCount; i++)
            {
                keyframes[i] = new Matrix[channelsCount];

                for (int j = 0; j < channelsCount; j++)
                    keyframes[i][j] = input.ReadMatrix();
            }

            return keyframes;
        }

    }

}
