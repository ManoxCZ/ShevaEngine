using Microsoft.Xna.Framework;

namespace ShevaEngine.Core.Graphics;

internal struct DrawCall
{
    public readonly int MaterialIndex;
    public readonly int MeshPartIndex;
    public readonly Matrix Transform;


    public DrawCall(int materialIndex, int meshPartIndex, in Matrix transform)
    {
        MaterialIndex = materialIndex;
        MeshPartIndex = meshPartIndex;
        Transform = transform;
    }
}
