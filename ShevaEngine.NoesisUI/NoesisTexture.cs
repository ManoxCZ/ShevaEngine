using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShevaEngine.NoesisUI;

internal sealed class NoesisTexture : Noesis.Texture
{
    public string Label { get; }
    public Texture2D Texture { get; }


    public NoesisTexture(string label, Texture2D texture)
    {
        Label = label;
        Texture = texture;
    }

    public override uint Width => (uint)Texture.Width;

    public override uint Height => (uint)Texture.Height;

    public override bool HasMipMaps => Texture.LevelCount > 1;

    public override bool IsInverted => false;

    public override bool HasAlpha => Texture.Format == SurfaceFormat.Color;


    public static SurfaceFormat GetSurfaceFormat(Noesis.TextureFormat format)
    {
        switch (format)
        {
            case Noesis.TextureFormat.RGBA8:
                return SurfaceFormat.Color;                
            
            case Noesis.TextureFormat.R8:
                return SurfaceFormat.Alpha8;

            default:
                throw new NotSupportedException($"Format ${format} is not supported");
        }        
    }

    public static int GetPixelSizeForSurfaceFormat(SurfaceFormat format)
    {
        switch (format)
        {
            case SurfaceFormat.Color:
                return 4;

            case SurfaceFormat.Alpha8:
                return 1;

            default:
                throw new NotSupportedException($"Format ${format} is not supported");
        }
    }
}
