using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Light.
	/// </summary>
	public abstract class Light
    {
        public Color Color { get; set; }
		public bool Enabled { get; set; }
		public Shadow Shadow { get; set; }
		        

        /// <summary>
        /// Constructor.
        /// </summary>
        public Light()
        {            
            Color = Color.White;
            Enabled = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Light(Color color)
			: this()
        {			
			Color = color;
        }

		/// <summary>
		/// Get light camera position.
		/// </summary>
		public abstract Vector3 GetLightCameraPosition(Camera camera);        
    }
}
