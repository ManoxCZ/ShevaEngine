using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.PostProcesses
{
    public class Bloom : BasePostProcess
    {
        protected override string EffectFilename => nameof(Bloom);
        public float BloomIntensity
        {
            get => Effect.Parameters[nameof(BloomIntensity)].GetValueSingle();
            set => Effect.Parameters[nameof(BloomIntensity)].SetValue(value);
        }
        public float BloomSaturation
        {
            get => Effect.Parameters[nameof(BloomSaturation)].GetValueSingle();
            set => Effect.Parameters[nameof(BloomSaturation)].SetValue(value);
        }
        public float BaseIntensity
        {
            get => Effect.Parameters[nameof(BaseIntensity)].GetValueSingle();
            set => Effect.Parameters[nameof(BaseIntensity)].SetValue(value);
        }
        public float BaseSaturation
        {
            get => Effect.Parameters[nameof(BaseSaturation)].GetValueSingle();
            set => Effect.Parameters[nameof(BaseSaturation)].SetValue(value);
        }
        public bool Glare { get; set; }


        /// <summary>
        /// Prepare effect.
        /// </summary>
        protected override void PrepareEffect(Camera camera, GameTime time, IScene scene)
        {
            if (Glare)
                Effect.CurrentTechnique = Effect.Techniques["GlareComposite"];
            else
                Effect.CurrentTechnique = Effect.Techniques["BloomComposite"];
        }
    }
}
