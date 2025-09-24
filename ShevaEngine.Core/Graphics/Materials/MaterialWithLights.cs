using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ShevaEngine.Core
{
    public abstract class MaterialWithLights : Material
    {
        public const int MaxLightsCount = 4;
        private EffectParameter _ambientLightParameter;
        private EffectParameter? _lightCountParameter;
        private EffectParameter? _lightTypesParameter;
        private EffectParameter? _lightPositionsParameter;
        private EffectParameter? _lightColorsParameter;
        private EffectParameter? _lightViewProjMatricesParameter;
        private EffectParameter? _lightShadowMapSizesParameter;
        private EffectParameter? _light1ShadowMapParameter;

        public Color AmbientLight
        {
            get => Color.FromNonPremultiplied(new Vector4(_ambientLightParameter.GetValueVector3(), 1));
            set => _ambientLightParameter?.SetValue(value.ToVector3());
        }
        public IEnumerable<Light> Lights
        {
            set
            {
                float[] types = new float[MaxLightsCount];
                Vector3[] positions = new Vector3[MaxLightsCount];
                Vector4[] colors = new Vector4[MaxLightsCount];
                Matrix[] viewProjs = new Matrix[MaxLightsCount];
                Vector2[] rtSizes = new Vector2[MaxLightsCount];

                int indexer = 0;

                foreach (var light in value)
                {
                    types[indexer] = GetLightType(light);
                    positions[indexer] = GetLightPosition(light);
                    colors[indexer] = new Vector4(
                        light.Color.R / 255.0f,
                        light.Color.G / 255.0f,
                        light.Color.B / 255.0f,
                        light.Color.A);
                    viewProjs[indexer] = GetLightViewProjPosition(light);
                    rtSizes[indexer] = GetShadowMapSize(light);

                    switch (indexer)
                    {
                        case 0:
                            _light1ShadowMapParameter?.SetValue(GetShadowMap(light));
                            break;
                    }

                    indexer++;
                }

                _lightCountParameter?.SetValue(value.Count());
                _lightTypesParameter?.SetValue(types);
                _lightPositionsParameter?.SetValue(positions);
                _lightColorsParameter?.SetValue(colors);
                _lightViewProjMatricesParameter?.SetValue(viewProjs);
                _lightShadowMapSizesParameter?.SetValue(rtSizes);
            }
        }


        /// <summary>
        /// Material with material.
        /// </summary>
        protected MaterialWithLights(Effect effect)
            : base(effect)
        {
            _ambientLightParameter = GetParameter("AmbientLight")!;
            _lightCountParameter = GetParameter("LightsCount");
            _lightTypesParameter = GetParameter("LightTypes");
            _lightPositionsParameter = GetParameter("LightPositions");
            _lightColorsParameter = GetParameter("LightColors");
            _lightViewProjMatricesParameter = GetParameter("LightViewProjs");
            _lightShadowMapSizesParameter = GetParameter("LightShadowMapSizes");
            _light1ShadowMapParameter = GetParameter("Light1ShadowMap");            
        }

        /// <summary>
        /// Get light type.
        /// </summary>
        private int GetLightType(Light light)
        {
            if (ReceiveShadows)
            {
                if (light is DirectionalLight)
                    return 1;
            }

            return 0;
        }

        /// <summary>
        /// Get light position.
        /// </summary>
        private Vector3 GetLightPosition(Light light)
        {
            if (light is DirectionalLight directionalLight)
                return directionalLight.Direction;

            return Vector3.Zero;
        }

        /// <summary>
        /// Get light viewProj matrix.
        /// </summary>
        private Matrix GetLightViewProjPosition(Light light)
        {
            if (light.Shadow != null)
                return light.Shadow.GetLightViewProjMatrix();

            return Matrix.Identity;
        }

        /// <summary>
        /// Get shadow map size.
        /// </summary>
        private Vector2 GetShadowMapSize(Light light)
        {
            if (light.Shadow is SingleMapShadow singleMapShadow)
                return new Vector2(singleMapShadow.Size.Width, singleMapShadow.Size.Height);

            return Vector2.Zero;
        }

        /// <summary>
        /// Get shadow map size.
        /// </summary>
        private Texture2D? GetShadowMap(Light light)
        {
            if (light.Shadow is SingleMapShadow singleMapShadow)
            {
                return singleMapShadow.GetShadowMap();
            }

            return null;
        }
    }
}
