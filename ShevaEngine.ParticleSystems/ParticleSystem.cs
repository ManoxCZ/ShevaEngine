using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;

namespace ShevaEngine.ParticleSystems
{
    /// <summary>
    /// Particle system.
    /// </summary>
    public class ParticleSystem
    {
        private int _actualParticle = 0;
        private double _spawningTime = 0;
        private readonly List<Vector3> _particlePositions;
        private readonly List<float> _particleLifetimes;
        private readonly List<float> _particleRandoms;
        private ParticleMaterial _graphicsMaterial;
        private ModelMesh _particleModel = null!;
        private readonly List<Matrix> _matrices;

        public Vector3 Position { get; set; }
        public float ParticleLifetime
        {
            get => _graphicsMaterial.Lifetime;
            set => _graphicsMaterial.Lifetime = value;
        }
        public Color StartColor
        {
            get => _graphicsMaterial.StartColor;
            set => _graphicsMaterial.StartColor = value;
        }
        public Color EndColor
        {
            get => _graphicsMaterial.EndColor;
            set => _graphicsMaterial.EndColor = value;
        }
        public float StartSize
        {
            get => _graphicsMaterial.StartSize;
            set => _graphicsMaterial.StartSize = value;
        }
        public float EndSize
        {
            get => _graphicsMaterial.EndSize;
            set => _graphicsMaterial.EndSize = value;
        }
        public Texture2D? Texture
        {
            get => _graphicsMaterial.Texture;
            set => _graphicsMaterial.Texture = value;
        }
        public float SpawnRatio { get; set; }
        public Vector3 StartVelocity { get; set; }
        public Vector3 EndVelocity { get; set; }
        public Func<Vector3>? GetStartPositionFunction { get; set; }




        /// <summary>
        /// Constructor.
        /// </summary>        
        public ParticleSystem(int maxParticles)
        {
            _particlePositions = new List<Vector3>(maxParticles);
            _particleLifetimes = new List<float>(maxParticles);
            _particleRandoms = new List<float>(maxParticles);

            Random random = new Random();

            for (int i = 0; i < maxParticles; i++)
            {
                _particlePositions.Add(Vector3.Zero);
                _particleLifetimes.Add(-1.0f);
                _particleRandoms.Add((float)random.NextDouble() * .5f + 0.8f);
            }

            _matrices = new List<Matrix>();

            _graphicsMaterial = new ParticleMaterial();
        }

        /// <summary>
        /// Load content.
        /// </summary>
        public void LoadContent(ShevaGame game)
        {
            _particleModel = ModelMeshExtensions.CreateModelMesh(
                new VertexPositionTexture[]
                {
                    new VertexPositionTexture() {Position = new Vector3(-0.5f, -0.5f, 0), TextureCoordinate = new Vector2(0,0) },
                    new VertexPositionTexture() {Position = new Vector3(-0.5f, 0.5f, 0), TextureCoordinate = new Vector2(0,1) },
                    new VertexPositionTexture() {Position = new Vector3(0.5f, 0.5f, 0), TextureCoordinate = new Vector2(1,1) },
                    new VertexPositionTexture() {Position = new Vector3(0.5f, -0.5f, 0), TextureCoordinate = new Vector2(1,0) }
                },
                new ushort[] { 0, 1, 2, 0, 2, 3 },
                _graphicsMaterial);
        }

        /// <summary>
        /// Update method.
        /// </summary>        
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _spawningTime += deltaTime;

            float spawnTime = 1.0f / SpawnRatio;

            int spawnParticles = (int)(_spawningTime / spawnTime);

            if (spawnParticles > 0)
            {
                SpawnParticles(spawnParticles);

                _spawningTime -= spawnParticles * spawnTime;
            }

            for (int i = 0; i < _particleLifetimes.Count; i++)
            {
                if (_particleLifetimes[i] > 0.0f)
                {
                    _particleLifetimes[i] -= deltaTime;

                    Vector3 velocity = Vector3.Lerp(EndVelocity, StartVelocity, _particleLifetimes[i] / ParticleLifetime);
                    velocity *= _particleRandoms[i];

                    _particlePositions[i] += velocity * deltaTime;
                }
            }
        }

        /// <summary>
        /// Spawn particles.
        /// </summary>        
        public void SpawnParticles(int count)
        {
            for (int i = 0; i < count; i++)
                if (_particleLifetimes[_actualParticle] <= 0.0f)
                {
                    _particleLifetimes[_actualParticle] = ParticleLifetime;
                    _particlePositions[_actualParticle] = GetStartPositionFunction == null ? Vector3.Zero : GetStartPositionFunction.Invoke();

                    _actualParticle = (_actualParticle + 1) % _particleLifetimes.Count;
                }
        }

        /// <summary>
        /// Get visible objects.
        /// </summary>
        public void GetVisibleObjects(RenderingPipeline pipeline)
        {
            _matrices.Clear();

            for (int i = 0; i < _particleLifetimes.Count; i++)
                if (_particleLifetimes[i] > 0.0f)
                {
                    Matrix matrix = Matrix.CreateBillboard(_particlePositions[i] + Position, pipeline.Camera.Position, pipeline.Camera.Up, null);
                    matrix.M44 = _particleLifetimes[i];

                    _matrices.Add(matrix);
                }

            pipeline.AddObject(_particleModel, _matrices);
        }
    }
}
