using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.PostProcesses
{
    public class BleachByPass : BasePostProcess
    {
        protected override string EffectFilename => "BleachByPass";
        public float Opacity
        {
            get => Effect.Parameters[nameof(Opacity)].GetValueSingle();
            set => Effect.Parameters[nameof(Opacity)].SetValue(value);
        }
    }
}
