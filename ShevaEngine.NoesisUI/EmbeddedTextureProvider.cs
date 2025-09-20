using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using Noesis;
using Noesis.MonoGame;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShevaEngine.NoesisUI;

internal class EmbeddedTextureProvider : TextureProvider
{
    private readonly ILogger _log;
    private readonly Dictionary<string, WeakReference<Texture2D>> _cache = new(StringComparer.OrdinalIgnoreCase);
    
    public EmbeddedTextureProvider()
    {
        _log = ShevaGame.Instance.Services.GetService<ILoggerFactory>().CreateLogger(GetType());        
    }

    public new void Dispose()
    {
        foreach (KeyValuePair<string, WeakReference<Texture2D>> entry in _cache)
        {
            if (entry.Value.TryGetTarget(out Texture2D? texture))
            {
                texture?.Dispose();
            }
        }

        _cache.Clear();

        base.Dispose();
    }
   
    public override TextureInfo GetTextureInfo(Uri uri)
    {
        if (GetTexture(uri.OriginalString) is Texture2D texture)
        {
            return new TextureInfo(texture.Width, texture.Height);
        }

        return new TextureInfo();
    }

    public override Noesis.Texture LoadTexture(Uri uri)
    {
        if (GetTexture(uri.OriginalString) is Texture2D texture)
        {
            return new NoesisTexture(uri.OriginalString, texture);
        }

        return null!;
    }

    private Texture2D LoadTextureFromStream(Stream fileStream)
    {
        Texture2D texture = Texture2D.FromStream(ShevaGame.Instance.GraphicsDevice, fileStream);

        if (texture.Format != SurfaceFormat.Color)
        {
            return texture;
        }

        // unfortunately, MonoGame loads textures as non-premultiplied alpha
        // need to premultiply alpha for correct rendering with NoesisGUI
        var buffer = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];
        texture.GetData(buffer);

        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
        }

        texture.SetData(buffer);

        return texture;
    }

    private Texture2D? GetTexture(string filename)
    {
        if (_cache.TryGetValue(filename, out var weakReference) &&
            weakReference.TryGetTarget(out var cachedTexture) &&
            !cachedTexture.IsDisposed)
        {
            return cachedTexture;
        }

        if (ShevaServices.GetService<IEmbeddedFilesService>().TryGetStream(filename, out Stream stream))
        {
            Texture2D texture = LoadTextureFromStream(stream);

            _cache[filename] = new WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>(texture);

            return texture;
        }

        return null;
    }
}
