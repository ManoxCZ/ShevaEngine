using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.PostProcesses
{
    public class ColorFilter : BasePostProcess
    {
        protected override string EffectFilename => nameof(ColorFilter);  
        public float Burn 
        { 
            get => Effect.Parameters[nameof(Burn)].GetValueSingle();
            set => Effect.Parameters[nameof(Burn)].SetValue(value);
        }
        public float Saturation
        { 
            get => Effect.Parameters[nameof(Saturation)].GetValueSingle();
            set => Effect.Parameters[nameof(Saturation)].SetValue(value);
        }
        public float Bright
        { 
            get => Effect.Parameters[nameof(Bright)].GetValueSingle();
            set => Effect.Parameters[nameof(Bright)].SetValue(value);
        }
        public Color Color 
        { 
            get => Color.FromNonPremultiplied(Effect.Parameters[nameof(Color)].GetValueVector4());
			set => Effect.Parameters[nameof(Color)].SetValue(value.ToVector4());            
        }

#if DEBUG_UI
        /// <summary>
        /// DebugUI.
        /// </summary>
        public override void DebugUI()
        {
            base.DebugUI();

            float tempSingle = Burn;
			ImGuiNET.ImGui.SliderFloat(nameof(Burn), ref tempSingle, 0.00000001f, 10);
            Burn = tempSingle;

            tempSingle = Saturation;
			ImGuiNET.ImGui.SliderFloat(nameof(Saturation), ref tempSingle, 0.00000001f, 10);
            Saturation = tempSingle;

            tempSingle = Bright;
			ImGuiNET.ImGui.SliderFloat(nameof(Bright), ref tempSingle, 0.00000001f, 10);
            Bright = tempSingle;

            Vector3 temp = Color.ToVector3();				
			ImGuiNET.ImGui.ColorEdit3(nameof(Color), ref temp);
			Color = new Color(temp);
		}	 
#endif
    }
}
