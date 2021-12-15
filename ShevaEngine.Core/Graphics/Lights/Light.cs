using Microsoft.Xna.Framework;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Light.
    /// </summary>
    public abstract class Light
    {
        public Color Color { get; set; } = Color.White;
        public bool Enabled { get; set; } = true;
        public Shadow? Shadow { get; set; }
                

        /// <summary>
        /// Get light camera position.
        /// </summary>
        public abstract Vector3 GetLightCameraPosition(Camera camera);
    }
}
