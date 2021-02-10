using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.PostProcesses
{
    public class BrightPass : BasePostProcess
    {
        protected override string EffectFilename => "BrightPass";
        public float BloomThreshold
        {
            get => Effect.Parameters[nameof(BloomThreshold)].GetValueSingle();
            set => Effect.Parameters[nameof(BloomThreshold)].SetValue(value);
        }
        
    }
}
