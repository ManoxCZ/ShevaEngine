#if !WINDOWS_UAP
using ImGuiNET;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.PostProcesses
{
    public class Vignette : BasePostProcess
    {
        protected override string EffectFilename => nameof(Vignette);  
        public float Slope 
        { 
            get => Effect.Parameters[nameof(Slope)].GetValueSingle();
            set => Effect.Parameters[nameof(Slope)].SetValue(value);
        }
        public float Radius 
        { 
            get => Effect.Parameters[nameof(Radius)].GetValueSingle();
            set => Effect.Parameters[nameof(Radius)].SetValue(value);
        }
        public float Amount
        { 
            get => Effect.Parameters[nameof(Amount)].GetValueSingle();
            set => Effect.Parameters[nameof(Amount)].SetValue(value);
        }


        /// <summary>
        /// Prepare effect.
        /// </summary>
        protected override void PrepareEffect(Camera camera, GameTime time, IScene scene)
        {
            Effect.Parameters["ScreenSize"]?.SetValue(new Vector2(InputTexture.Width, InputTexture.Height));
            Effect.Parameters["PixelSize"]?.SetValue(new Vector2(1.0f / InputTexture.Width, 1.0f / InputTexture.Height));
        }

#if !WINDOWS_UAP
        /// <summary>
        /// DebugUI.
        /// </summary>
        public override void DebugUI()
        {
            base.DebugUI();

            float tempSingle = Slope;
			ImGui.SliderFloat(nameof(Slope), ref tempSingle, 2, 16);
            Slope = tempSingle;

            tempSingle = Radius;
			ImGui.SliderFloat(nameof(Radius), ref tempSingle, -1, 3);
            Radius = tempSingle;

            tempSingle = Amount;
			ImGui.SliderFloat(nameof(Amount), ref tempSingle, -2, 1);
            Amount = tempSingle;
		}	 
#endif
    }
}
