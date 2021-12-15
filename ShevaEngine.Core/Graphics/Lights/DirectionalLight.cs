using Microsoft.Xna.Framework;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Directional light.
    /// </summary>
    public class DirectionalLight : Light
    {
        private Vector3 _direction = Vector3.Down;
        public Vector3 Direction
        {
            get => _direction;
            set => _direction = Vector3.Normalize(value);
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
