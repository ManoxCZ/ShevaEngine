using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using ShevaEngine.AnimationImporters.Animation;


namespace ShevaEngine.AnimationImporters.Writers;

[ContentTypeWriter]
class ClipWriter : ContentTypeWriter<ClipContent>
{
    protected override void Write(ContentWriter output, ClipContent value)
    {
        output.Write(value.Duration);
        output.Write(value.Keyframes.Length);
        output.Write(value.Keyframes[0].Length);

        foreach (Matrix[] matrices in value.Keyframes)
        {
            for (int i = 0; i < matrices.Length; i++)
            {
                output.Write(matrices[i]);
            }
        }
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return "ShevaEngine.Core.Clip, ShevaEngine.Core";
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return "ShevaEngine.Core.ClipReader, ShevaEngine.Core";
    }
}
