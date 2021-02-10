using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Post process.
	/// </summary>
	public abstract class PostProcess
	{
		public bool Enabled { get; set; } = true;
		public Texture2D InputTexture { get; internal set; }
		public Texture2D DepthTexture { get; internal set; }
				

		/// <summary>
		/// Load content.
		/// </summary>		
		public virtual void LoadContent(ContentManager content)
		{

		}

		/// <summary>
		/// Apply post process.
		/// </summary>				
		public virtual void Apply(Camera camera, GameTime time, Scene scene)
		{

		}

#if DEBUG_UI
		/// <summary>
        /// DebugUI.
        /// </summary>
        public virtual void DebugUI()
        {
			bool tempBool = Enabled;
			ImGuiNET.ImGui.Checkbox("Enabled", ref tempBool);
			Enabled = tempBool;
		}	
#endif
	}
}
