namespace ShevaEngine.Terrain;

public struct SplatMaterial
{
    public static SplatMaterial None = new(string.Empty, string.Empty);

    public readonly string Albedo;
    public readonly string Normal;


    /// <summary>
    /// Constructor.
    /// </summary>
    public SplatMaterial(string albedo, string normal)
    {
        Albedo = albedo;
        Normal = normal;
    }
}
