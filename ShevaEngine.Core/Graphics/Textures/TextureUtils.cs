using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace ShevaEngine.Core
{
    public class TextureUtils
    {
        private const int WHITE_TEXTURE_SIZE = 32;

        public static Texture2D WhiteTexture;


        /// <summary>
        /// Prepare texture utils.
        /// </summary>        
        public static void Prepare(GraphicsDevice device)
        {
            WhiteTexture = new Texture2D(device, WHITE_TEXTURE_SIZE, WHITE_TEXTURE_SIZE, false, SurfaceFormat.Color);
            WhiteTexture.SetData(Enumerable.Repeat(Color.White, WHITE_TEXTURE_SIZE * WHITE_TEXTURE_SIZE).ToArray());
        }
    }
}
