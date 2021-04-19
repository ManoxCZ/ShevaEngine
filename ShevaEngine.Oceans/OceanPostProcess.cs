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
        private Color _oceanColor;
        public Color OceanColor
        {
            get => _oceanColor;
            set
            {
                _oceanColor = value;

                if (_oceanMaterial != null)
                    _oceanMaterial.OceanColor = value;
            }
        }
        private Color _skyColor;
        public Color SkyColor
        {
            get => _skyColor;
            set
            {
                _skyColor = value;

                if (_oceanMaterial != null)
                    _oceanMaterial.SkyColor = value;
            }
        }
        private float _depthFactor;
        public float DepthFactor
        {
            get => _depthFactor;
            set
            {
                _depthFactor = value;

                if (_oceanMaterial != null)
                    _oceanMaterial.DepthFactor = value;
            }
        }
        private float _lightFactor;
        public float LightFactor
        {
            get => _lightFactor;
            set
            {
                _lightFactor = value;

                if (_oceanMaterial != null)
                    _oceanMaterial.LightFactor = value;
            }
        }

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
                OceanColor = OceanColor,
                SkyColor = SkyColor,
                DepthFactor = DepthFactor,
                LightFactor = LightFactor
			};

			_oceanModel = PlaneMesh.GenerateModel(120, 60);
			_oceanModel.Meshes[0].MeshParts[0].Effect = _oceanMaterial;

            _pipeline = new RenderingPipeline("Ocean post process");
			_pipeline.AddObject(_oceanModel, Matrix.Identity);	
        }
        
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

#if !WINDOWS_UAP
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
