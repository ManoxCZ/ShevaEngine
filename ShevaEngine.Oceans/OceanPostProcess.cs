using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System.Linq;

namespace ShevaEngine.Oceans
{
    /// <summary>
    /// Post process.
    /// </summary>
    public class OceanPostProcess : PostProcess
    {
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
                CausticsTexture = content.Load<Texture2D>(@"Content\Graphics\Caustics_001"),
                CausticsTextures = Enumerable.Range(1, 32).Select(item => content.Load<Texture2D>($@"Content\Graphics\Caustics_{item:000}")).ToArray(),
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

            _oceanMaterial.RefractionTexture = null!;
            _oceanMaterial.DepthTexture = null!;
        }
    }
}
