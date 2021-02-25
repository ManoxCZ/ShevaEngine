using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;


namespace ShevaEngine.PostProcesses
{
    /// <summary>
    /// Gaussian blur horizontal.
    /// </summary>
    public class GaussBlurH : BasePostProcess
    {
        protected override string EffectFilename => "GaussianBlur";
        public Vector2[] SampleOffsets
        {
            get => Effect.Parameters[nameof(SampleOffsets)].GetValueVector2Array();
            set => Effect.Parameters[nameof(SampleOffsets)].SetValue(value);
        }
        public float[] SampleWeights
        {
            get => Effect.Parameters[nameof(SampleWeights)].GetValueSingleArray();
            set => Effect.Parameters[nameof(SampleWeights)].SetValue(value);
        }
        private bool _blurRadiusChanged = true;
        private float _blurRadius = 5;
        public float BlurRadius
        {
            get { return _blurRadius; }
            set
            {
                _blurRadius = value;

                _blurRadiusChanged = true; 
            }
        }

        /// <summary>
        /// Prepare effect.
        /// </summary>
        protected override void PrepareEffect(Camera camera, GameTime time, IScene scene)
        {
            if (_blurRadiusChanged)
            {
                SetBlurEffectParameters(camera.Size.Width);

                _blurRadiusChanged = false;
            }
        }

        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        private void SetBlurEffectParameters(int resolution)
        {        
            SampleWeights = new float[] { 0.2270270270f, 0.1945945946f, 0.1216216216f, 0.0540540541f, 0.0162162162f };

            SampleOffsets = new Vector2[]
            {
                new Vector2(0,0),
                new Vector2((0.25f * _blurRadius) / resolution,0),
                new Vector2((0.5f * _blurRadius) / resolution,0),
                new Vector2((0.75f * _blurRadius) / resolution,0),
                new Vector2(_blurRadius / resolution,0),
            };
        }
    }
}
