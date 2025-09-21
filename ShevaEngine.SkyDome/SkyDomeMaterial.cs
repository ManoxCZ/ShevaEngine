using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.SkyDomes;

public class SkyDomeMaterial : Material
{
    private EffectParameter? _mieTextureParameter;
    private EffectParameter? _rayleighTextureParameter;

    public Texture2D? MieTexture
    {
        get => _mieTextureParameter?.GetValueTexture2D();
        set => _mieTextureParameter?.SetValue(value);
    }

    public Texture2D? RayleighTexture
    {
        get => _rayleighTextureParameter?.GetValueTexture2D();
        set => _rayleighTextureParameter?.SetValue(value);
    }

    public SkyDomeMaterial() 
        : base(ShevaGame.Instance.Content.Load<Effect>("Content\\SkyDome\\Effects\\SkyDome"))
    {
        _mieTextureParameter = GetParameter("MieTexture");
        _rayleighTextureParameter = GetParameter("Rayleigh");
    }
}
