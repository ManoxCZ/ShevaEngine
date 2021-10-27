using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace ShevaEngine.Terrain
{
    /// <summary>
    /// Terrain object.
    /// </summary>
    public class TerrainObject : IDisposable
    {
        private const int SEGMENT_SIZE = 32;

        private List<IDisposable> _disposables;
        private bool _heightmapDirty { get; set; } = true;
        private float[] _heightmap;
        private Color[] _splatMap;
        private ModelMesh[,] _segments;
        private TerrainMaterial _renderMaterial;
        public float Scale { get; set; } = 1;
        public int HeightmapWidth { get; private set; }
        public int HeightmapHeight { get; private set; }
        public int SplatmapScale => 8;
        public int SplatmapWidth => HeightmapWidth * SplatmapScale;
        public int SplatmapHeight => HeightmapHeight * SplatmapScale;
        public float Width => HeightmapWidth * Scale;
        public float Height => HeightmapHeight * Scale;

        public BehaviorSubject<SplatMaterial> Splatmap1Material { get; }
        public BehaviorSubject<SplatMaterial> Splatmap2Material { get; }
        public BehaviorSubject<SplatMaterial> Splatmap3Material { get; }
        public BehaviorSubject<SplatMaterial> Splatmap4Material { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TerrainObject()
        {
            _disposables = new List<IDisposable>();

            Splatmap1Material = new BehaviorSubject<SplatMaterial>(SplatMaterial.None);
            _disposables.Add(Splatmap1Material);

            Splatmap2Material = new BehaviorSubject<SplatMaterial>(SplatMaterial.None);
            _disposables.Add(Splatmap2Material);

            Splatmap3Material = new BehaviorSubject<SplatMaterial>(SplatMaterial.None);
            _disposables.Add(Splatmap3Material);

            Splatmap4Material = new BehaviorSubject<SplatMaterial>(SplatMaterial.None);
            _disposables.Add(Splatmap4Material);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            foreach (IDisposable disposable in _disposables)
                disposable?.Dispose();

            _disposables = null;
        }

        /// <summary>
        /// Initialize.
        /// </summary>        
        public void Initialize(int heightmapWidth, int heightmapHeight, float[] data)
        {
            HeightmapWidth = heightmapWidth;
            HeightmapHeight = heightmapHeight;

            _heightmap = data;

            _splatMap = new Color[SplatmapWidth * SplatmapHeight];
        }

        /// <summary>
        /// Load content.
        /// </summary>        
        public void LoadContent(ShevaGame game)
        {
            _renderMaterial = new TerrainMaterial
            {
                HeightmapTexture = new Texture2D(
                    game.GraphicsDevice,
                    HeightmapWidth, HeightmapHeight,
                    false,
                    SurfaceFormat.Single),

                SplatMapTexture = new Texture2D(
                    game.GraphicsDevice,
                    SplatmapWidth, SplatmapHeight,
                    false,
                    SurfaceFormat.Color)
            };

            _disposables.Add(Splatmap1Material.Subscribe(item =>
            {
                if (!string.IsNullOrEmpty(item.Albedo))
                    _renderMaterial.TextureChannel0 = game.Content.Load<Texture2D>(item.Albedo);
                else
                    _renderMaterial.TextureChannel0 = TextureUtils.WhiteTexture;

                if (!string.IsNullOrEmpty(item.Normal))
                    _renderMaterial.TextureChannel0Norm = game.Content.Load<Texture2D>(item.Normal);
                else
                    _renderMaterial.TextureChannel0Norm = TextureUtils.WhiteTexture;
            }));

            _disposables.Add(Splatmap2Material.Subscribe(item =>
            {
                if (!string.IsNullOrEmpty(item.Albedo))
                    _renderMaterial.TextureChannel1 = game.Content.Load<Texture2D>(item.Albedo);
                else
                    _renderMaterial.TextureChannel1 = TextureUtils.WhiteTexture;

                if (!string.IsNullOrEmpty(item.Normal))
                    _renderMaterial.TextureChannel1Norm = game.Content.Load<Texture2D>(item.Normal);
                else
                    _renderMaterial.TextureChannel1Norm = TextureUtils.WhiteTexture;
            }));

            _disposables.Add(Splatmap3Material.Subscribe(item =>
            {
                if (!string.IsNullOrEmpty(item.Albedo))
                    _renderMaterial.TextureChannel2 = game.Content.Load<Texture2D>(item.Albedo);
                else
                    _renderMaterial.TextureChannel2 = TextureUtils.WhiteTexture;

                if (!string.IsNullOrEmpty(item.Normal))
                    _renderMaterial.TextureChannel2Norm = game.Content.Load<Texture2D>(item.Normal);
                else
                    _renderMaterial.TextureChannel2Norm = TextureUtils.WhiteTexture;
            }));

            _disposables.Add(Splatmap4Material.Subscribe(item =>
            {
                if (!string.IsNullOrEmpty(item.Albedo))
                    _renderMaterial.TextureChannel3 = game.Content.Load<Texture2D>(item.Albedo);
                else
                    _renderMaterial.TextureChannel3 = TextureUtils.WhiteTexture;

                if (!string.IsNullOrEmpty(item.Normal))
                    _renderMaterial.TextureChannel3Norm = game.Content.Load<Texture2D>(item.Normal);
                else
                    _renderMaterial.TextureChannel3Norm = TextureUtils.WhiteTexture;
            }));

            int columnsCount = HeightmapWidth / SEGMENT_SIZE;
            int rowsCount = HeightmapHeight / SEGMENT_SIZE;

            _segments = new ModelMesh[columnsCount, rowsCount];

            for (int iRow = 0; iRow < rowsCount; iRow++)
                for (int iCol = 0; iCol < columnsCount; iCol++)
                    _segments[iCol, iRow] = CreateMesh(iCol * SEGMENT_SIZE, iRow * SEGMENT_SIZE);
        }

        /// <summary>
        /// Create mesh.
        /// </summary>
        public ModelMesh CreateMesh(int startX, int startZ, float scale = 1.0f)
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[(SEGMENT_SIZE + 1) * (SEGMENT_SIZE + 1)];
            
            for (int z = 0; z < SEGMENT_SIZE + 1; z++)
                for (int x = 0; x < SEGMENT_SIZE + 1; x++)
                    vertices[x + z * (SEGMENT_SIZE + 1)].Position = new Vector3((startX + x) * scale, 0, (startZ + z) * scale);

            List<int> indices = new List<int>();

            for (int z = 0; z < SEGMENT_SIZE; z++)
                for (int x = 0; x < SEGMENT_SIZE; x++)
                {
                    int lowerLeft = x + z * (SEGMENT_SIZE + 1);
                    int lowerRight = (x + 1) + z * (SEGMENT_SIZE + 1);
                    int topLeft = x + (z + 1) * (SEGMENT_SIZE + 1);
                    int topRight = (x + 1) + (z + 1) * (SEGMENT_SIZE + 1);

                    indices.Add(lowerLeft);
                    indices.Add(topLeft);
                    indices.Add(topRight);

                    indices.Add(lowerLeft);
                    indices.Add(topRight);
                    indices.Add(lowerRight);
                }

            return ModelMeshExtensions.CreateModelMesh(vertices, indices, _renderMaterial);
        }

        /// <summary>
        /// Get visible segments.
        /// </summary>
        private IEnumerable<ModelMesh> GetVisibleSegments(RenderingPipeline pipeline)
        {
            return _segments.Cast<ModelMesh>();
        }
    }
}
