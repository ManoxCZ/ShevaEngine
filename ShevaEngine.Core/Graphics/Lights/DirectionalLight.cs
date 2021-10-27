using Microsoft.Xna.Framework;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Directional light.
    /// </summary>
    public class DirectionalLight : Light
    {
        private Vector3 _direction;
        public Vector3 Direction
        {
            get => _direction;
            set => _direction = Vector3.Normalize(value);
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public DirectionalLight()
            : base()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DirectionalLight(Vector3 direction, Color color)
            : base(color)
        {
        }

        /// <summary>
        /// Get light camera position.
        /// </summary>
        public override Vector3 GetLightCameraPosition(Camera camera)
        {
            return camera.Target - (Direction * Vector3.Distance(camera.Position, camera.Target));
        }
    }
}
