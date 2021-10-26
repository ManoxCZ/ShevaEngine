using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;

namespace ShevaEngine.NoesisUI
{
    public class TextureProvider : Noesis.TextureProvider
    {
        public override void GetTextureInfo(Uri filename, out uint width, out uint height)
        {
            string contentPath = GetContentFilename(filename.OriginalString);

            Texture2D texture = ShevaGame.Instance.Content.Load<Texture2D>(contentPath);

            if (texture != null)
            {
                width = (uint)texture.Width;
                height = (uint)texture.Height;
            }
            else
            {
                width = 0;
                height = 0;
            }
        }

        public override Noesis.Texture LoadTexture(Uri filename)
        {
            string contentPath = GetContentFilename(filename.OriginalString);

            Texture2D texture = ShevaGame.Instance.Content.Load<Texture2D>(contentPath);

            System.Reflection.FieldInfo info = typeof(Texture2D).GetField("_texture", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            SharpDX.Direct3D11.Resource handle = info.GetValue(texture) as SharpDX.Direct3D11.Resource;

            return Noesis.RenderDeviceD3D11.WrapTexture(texture, handle.NativePointer, texture.Width, texture.Height, texture.LevelCount, false, true);
        }

        /// <summary>
        /// Get content filename.
        /// </summary>
        public static string GetContentFilename(string filename)
        {
            return filename
                .Replace(".png", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace(".tif", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace(".tiff", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace(".webp", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace(".jpg", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace(".jpeg", string.Empty, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
