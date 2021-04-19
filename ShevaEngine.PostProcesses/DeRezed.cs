#if !WINDOWS_UAP
using ImGuiNET;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.PostProcesses
{
    public class DeRezed : BasePostProcess
    {
        protected override string EffectFilename => nameof(DeRezed);  
        public int NumberOfTiles 
        { 
            get => Effect.Parameters[nameof(NumberOfTiles)].GetValueInt32();
            set => Effect.Parameters[nameof(NumberOfTiles)].SetValue(value);
        }


#if !WINDOWS_UAP
        /// <summary>
        /// DebugUI.
        /// </summary>
        public override void DebugUI()
        {
            base.DebugUI();

            int tempInt32 = NumberOfTiles;
			ImGui.SliderInt("Number of tiles", ref tempInt32, 1, 2048);
            NumberOfTiles = tempInt32;
		}
#endif
    }
}
