//#define EXPORT_TEXTURES

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;

namespace ShevaEngine.NoesisUI;

public sealed class MonogameNoesisRenderDevice : Noesis.RenderDevice
{
    private readonly GraphicsDevice _device;
    
    private byte[] _vertices = Array.Empty<byte>();
    private Memory<byte> _verticesMemory = new();
    private MemoryHandle? _verticesMemoryHandle;
    
    private byte[] _indices = Array.Empty<byte>();
    private Memory<byte> _indicesMemory = new();
    private MemoryHandle? _indicesMemoryHandle;

    public MonogameNoesisRenderDevice(GraphicsDevice device)
    {
        _device = device;

    }

    public override Noesis.DeviceCaps Caps => new ();

    public override void BeginOffscreenRender()
    {

    }

    public override void BeginOnscreenRender()
    {

    }

    public override void BeginTile(Noesis.RenderTarget surface, Noesis.Tile tile)
    {
        throw new NotImplementedException();
    }

    public override Noesis.RenderTarget CloneRenderTarget(string label, Noesis.RenderTarget surface)
    {
        if (surface is NoesisRenderTarget renderTarget)
        {
            return new NoesisRenderTarget(
                label,
                new RenderTarget2D(_device,
                    renderTarget.RenderTarget2D.Width,
                    renderTarget.RenderTarget2D.Height,
                    renderTarget.RenderTarget2D.LevelCount != 1,
                    renderTarget.RenderTarget2D.Format,
                    renderTarget.RenderTarget2D.DepthStencilFormat,
                    renderTarget.RenderTarget2D.MultiSampleCount,
                    renderTarget.RenderTarget2D.RenderTargetUsage));
        }

        throw new ArgumentException($"Surface is not a correct type, should be {nameof(NoesisRenderTarget)} but it is {surface.GetType().FullName}");
    }

    public override Noesis.RenderTarget CreateRenderTarget(string label, uint width, uint height, uint sampleCount, bool needsStencil)
    {
        return new NoesisRenderTarget(label, new RenderTarget2D(_device, (int)width, (int)height));
    }

    public override Noesis.Texture CreateTexture(string label, uint width, uint height, uint numLevels, Noesis.TextureFormat format, nint data)
    {
        NoesisTexture noesisTexture = new(
            label,
            new Texture2D(_device, (int)width, (int)height, numLevels > 1, NoesisTexture.GetSurfaceFormat(format), (int)numLevels));

        if (data != 0)
        {            
            nint[] dataArray = new nint[numLevels];
            Marshal.Copy(data, dataArray, 0, (int)numLevels);

            for (int i = 0; i < numLevels; i++)
            {
                if (dataArray[i] != 0)
                {
                    UpdateTexture(noesisTexture, (uint)i, 0, 0, width, height, dataArray[i]);
                }
                width >>= 1;
                height >>= 1;

                width = Math.Max(width, 1);
                height = Math.Max(height, 1);
            }
        }

        return noesisTexture;
    }

    public override void DrawBatch(ref Noesis.Batch batch)
    {
        
    }

    public override void EndOffscreenRender()
    {

    }

    public override void EndOnscreenRender()
    {

    }

    public override void EndTile(Noesis.RenderTarget surface)
    {
        throw new NotImplementedException();
    }

    public unsafe override nint MapIndices(uint bytes)
    {
        if (_indices.Length < bytes)
        {
            _indices = new byte[bytes];

            _indicesMemory = new Memory<byte>(_indices);
        }

        _indicesMemoryHandle = _indicesMemory.Pin();

        return new nint(_indicesMemoryHandle.Value.Pointer);
    }

    public unsafe override nint MapVertices(uint bytes)
    {
        if (_vertices.Length < bytes)
        {
            _vertices = new byte[bytes];

            _verticesMemory = new Memory<byte>(_vertices);
        }

        _verticesMemoryHandle = _verticesMemory.Pin();

        return new nint(_verticesMemoryHandle.Value.Pointer);
    }

    public override void ResolveRenderTarget(Noesis.RenderTarget surface, Noesis.Tile[] tiles)
    {
        throw new NotImplementedException();
    }

    public override void SetRenderTarget(Noesis.RenderTarget surface)
    {
        if (surface is NoesisRenderTarget renderTarget)
        {
            _device.SetRenderTarget(renderTarget.RenderTarget2D);
        }
        else
        {
            throw new ArgumentException($"Surface is not a correct type, should be {nameof(NoesisRenderTarget)} but it is {surface.GetType().FullName}");
        }
    }

    public override void UnmapIndices()
    {
        _indicesMemoryHandle?.Dispose();
        _indicesMemoryHandle = null;
    }

    public override void UnmapVertices()
    {
        _verticesMemoryHandle?.Dispose();
        _verticesMemoryHandle = null;
    }

    public override void UpdateTexture(Noesis.Texture texture, uint level, uint x, uint y, uint width, uint height, nint data)
    {
        if (texture is NoesisTexture noesisTexture)
        {
            int pixelSize = NoesisTexture.GetPixelSizeForSurfaceFormat(noesisTexture.Texture.Format);
            int size = (int)(width * height * pixelSize);

            byte[] dataArray = new byte[size];
            Marshal.Copy(data, dataArray, 0, size);

            noesisTexture.Texture.SetData((int)level, new Rectangle((int)x, (int)y, (int)width, (int)height), dataArray, 0, size);

#if EXPORT_TEXTURES
            using StreamWriter writer = new StreamWriter(@$"D:\Projects\textures\{noesisTexture.Label}");

            noesisTexture.Texture.SaveAsPng(writer.BaseStream, noesisTexture.Texture.Width, noesisTexture.Texture.Height);
#endif
        }
    }
}
