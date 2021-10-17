using Microsoft.Xna.Framework.Graphics;
using Noesis;
using ShevaEngine.Core;
using System;
using System.IO;

namespace ShevaEngine.NoesisUI
{
    public class TextureProvider : Noesis.FileTextureProvider
    {
        private readonly ShevaGame _game;


        /// <summary>
        /// Constructor.
        /// </summary>        
        public TextureProvider(ShevaGame game)
        {
            _game = game;
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

        /// <summary>
        /// Get texture info.
        /// </summary>
        public override void GetTextureInfo(Uri uri, out uint width, out uint height)
        {
            width = 0;
            height = 0;

            string contentName = GetContentFilename(uri.OriginalString);

            Texture2D texture = _game.Content.Load<Texture2D>(contentName);

            if (texture == null)
                return;

            width = (uint)texture.Width;
            height = (uint)texture.Height;
        }

        /// <summary>
        /// Load texture.
        /// </summary>
        public override Noesis.Texture LoadTexture(Uri uri)
        {
            string contentName = GetContentFilename(uri.OriginalString);

            Texture2D texture = _game.Content.Load<Texture2D>(contentName);

            System.Reflection.FieldInfo info = typeof(Texture2D).GetField("_texture", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            SharpDX.Direct3D11.Resource handle = info.GetValue(texture) as SharpDX.Direct3D11.Resource;

            return Noesis.RenderDeviceD3D11.WrapTexture(texture, handle.NativePointer, texture.Width, texture.Height, texture.LevelCount, false, true);            
        }

        /// <summary>
        /// Open stream.
        /// </summary>
        public override Stream OpenStream(Uri filename)
        {
            string contentName = GetContentFilename(filename.OriginalString);

            Texture2D texture = _game.Content.Load<Texture2D>(contentName);

            Stream stream = new MemoryStream();
            
            texture.SaveAsPng(stream, texture.Width, texture.Height);

            stream.Position = 0;

            return stream;
        }
    }
}
