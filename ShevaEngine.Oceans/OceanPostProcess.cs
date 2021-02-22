using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;

namespace ShevaEngine.Oceans
{
	/// <summary>
	/// Post process.
	/// </summary>
	public class OceanPostProcess : PostProcess
	{
		private SpriteBatch _spriteBatch;
		private RenderingPipeline _pipeline;
		private Ocean _ocean;
		private Model _oceanModel;
		private OceanMaterial _oceanMaterial;				


		/// <summary>
		/// Constructor.
		/// </summary>		
		public OceanPostProcess(Ocean ocean)
		{
			_ocean = ocean;
			_spriteBatch = new SpriteBatch(ShevaGame.Instance.GraphicsDevice);					
		}

		/// <summary>
		/// Load content.
		/// </summary>		
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

			_oceanMaterial = new OceanMaterial(_ocean)
			{
				NormalTexture = content.Load<Texture2D>(@"Content\Graphics\OceanNormalMap"),				
			};

			_oceanModel = PlaneMesh.GenerateModel(120, 60);
			_oceanModel.Meshes[0].MeshParts[0].Effect = _oceanMaterial;

            _pipeline = new RenderingPipeline("Ocean post process");
			_pipeline.AddObject(_oceanModel, Matrix.Identity);	
        }

        /// <summary>
        /// Apply.
        /// </summary>
        

		/// <summary>
		/// Apply.
		/// </summary>
		public override void Apply(Camera camera, GameTime time, IScene scene)
		{				
			_pipeline.GameTime = time;
			_pipeline.SetCamera(camera);

			_pipeline.ClearLights();

			foreach (Light light in scene.GetLights())
				_pipeline.AddLight(light);

			_oceanMaterial.RefractionTexture = InputTexture;
			_oceanMaterial.DepthTexture = DepthTexture;

            _pipeline.Draw();

			_oceanMaterial.RefractionTexture = null;
			_oceanMaterial.DepthTexture = null;
		}

#if DEBUG_UI
		/// <summary>
        /// DebugUI.
        /// </summary>
        public override void DebugUI()
		{
			Vector3 temp = _oceanMaterial.OceanColor.ToVector3();				
			ImGuiNET.ImGui.ColorEdit3("Ocean color", ref temp);
			_oceanMaterial.OceanColor = new Color(temp);

			temp = _oceanMaterial.SkyColor.ToVector3();				
			ImGuiNET.ImGui.ColorEdit3("Sky color", ref temp);
			_oceanMaterial.SkyColor = new Color(temp);

			float tempSingle = _oceanMaterial.DepthFactor;
			ImGuiNET.ImGui.SliderFloat("Depth factor", ref tempSingle, 0.01f, 5.0f);
			_oceanMaterial.DepthFactor = tempSingle;

			tempSingle = _oceanMaterial.LightFactor;
			ImGuiNET.ImGui.SliderFloat("Light factor", ref tempSingle, 0.00f, 1.0f);
			_oceanMaterial.LightFactor = tempSingle;
		}
#endif
	}
}
