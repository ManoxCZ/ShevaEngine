using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Material profile.
    /// </summary>
    public enum MaterialProfile
    {
        Default = 0,
        Shadows = 1
    }

    /// <summary>
    /// Material.
    /// </summary>
    public abstract class Material : Effect
    {
        protected readonly ILogger Log;
        public const int SKINNED_EFFECT_MAX_BONES = 72;
        public static Matrix[] BonesIdentity;

        private EffectParameter? _viewParameter;
        private EffectParameter? _projParameter;
        private EffectParameter? _cameraPositionParameter;
        private EffectParameter? _gameTimeParameter;
        private EffectParameter? _bonesParameter;
        public bool Transparent { get; set; } = false;
        public bool CastShadows { get; set; } = false;
        public bool ReceiveShadows { get; set; } = true;
        public bool Animated { get; set; } = false;
        public Matrix[] Bones
        {
            get
            {
                if (!Animated)
                {
                    return Array.Empty<Matrix>();
                }

                if (_bonesParameter == null)
                {
                    Log.LogWarning("Bones parameter is null");

                    return Array.Empty<Matrix>();
                }
                else
                {
                    return _bonesParameter.GetValueMatrixArray(SKINNED_EFFECT_MAX_BONES);
                }
            }
            set
            {
                if (!Animated)
                {
                    return;
                }

                if (_bonesParameter == null)
                {
                    Log.LogWarning("Bones parameter is null");
                }
                else
                {
                    _bonesParameter.SetValue(value);
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>		
        protected Material(Effect effect)
            : base(effect)
        {
            Log = ShevaServices.GetService<ILoggerFactory>().CreateLogger(GetType());

            _viewParameter = GetParameter("ViewMatrix");
            _projParameter = GetParameter("ProjMatrix");
            _cameraPositionParameter = GetParameter("CameraPosition");
            _gameTimeParameter = GetParameter("GameTime");
            _bonesParameter = GetParameter("Bones");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        static Material()
        {
            BonesIdentity = new Matrix[SKINNED_EFFECT_MAX_BONES];

            for (int i = 0; i < SKINNED_EFFECT_MAX_BONES; i++)
                BonesIdentity[i] = Matrix.Identity;
        }

        /// <summary>
        /// Get parameter.
        /// </summary>
        protected EffectParameter? GetParameter(string name)
        {
            EffectParameter? parameter = Parameters.FirstOrDefault(item => item.Name == name);

            if (parameter == null)
            {
                Log.LogError($"Effect parameter {name} doesn't exist");
            }

            return parameter;
        }

        /// <summary>
        /// Apply.
        /// </summary>		
        public virtual void Apply(MaterialProfile matProfile, Camera camera, float gameTime, VertexDeclaration declaration)
        {
            _viewParameter?.SetValue(camera.ViewMatrix);
            _projParameter?.SetValue(camera.ProjectionMatrix);
            _cameraPositionParameter?.SetValue(camera.Position);
            _gameTimeParameter?.SetValue(gameTime);

            if (Animated)
                Techniques[(int)matProfile].Passes[declaration.Name + @"Animated"].Apply();
            else
                Techniques[(int)matProfile].Passes[declaration.Name].Apply();
        }
    }
}
