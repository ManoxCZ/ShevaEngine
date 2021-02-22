using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShevaEngine.Core
{

	public enum CameraType
	{
		Perspective,
		Orthographic
	}

	/// <summary>
	/// Camera.
	/// </summary> 
	public class Camera
#if DEBUG_UI
        : IDebugUIPage
#endif
	{
		private readonly Log _log = new Log(typeof(Camera));
		public string DebugUIPageName { get; private set; }
		public CameraType _cameraType;
		public CameraType CameraType
		{
			get => _cameraType;
			set 
			{
				_cameraType = value;

				CreateProjectionMatrix();
			}
		}
		public Matrix ViewMatrix { get; private set; }
		public Matrix ProjectionMatrix { get; private set; }

		private float _fieldOfView = (float)(Math.PI / 4);
		public float FieldOfView
		{
			get => _fieldOfView;
			set
			{
				_fieldOfView = value;

				CreateProjectionMatrix();
			}
		}
		private Resolution _size = new Resolution(512,512);
		public Resolution Size
		{
			get => _size;
			set
			{
				_size = value;

				CreateProjectionMatrix();
			}
		}
		private (float Width, float Height) _orthographicSize = (10, 10);
		public (float Width, float Height) OrthographicSize
		{
			get => _orthographicSize;
			set
			{
				_orthographicSize = value;

				CreateProjectionMatrix();
			}
		}
		private float _nearPlane = 1f;
		public float NearPlane
		{
			get => _nearPlane;
			set
			{
				_nearPlane = value;

				CreateProjectionMatrix();
			}
		}
		private float _farPlane = 100f;
		public float FarPlane
		{
			get => _farPlane;
			set
			{
				_farPlane = value;

				CreateProjectionMatrix();
			}
		}

		public Color ClearColor { get; set; } = Color.Black;
        private readonly RenderingPipeline _pipeline;

		public Vector3 Position { get; set; }
		public Vector3 Target { get; set; }
		public Vector3 Up { get; set; }
		public List<PostProcess> PostProcesses { get; private set; } = new List<PostProcess>();
		public BlendState BlendState
		{
			get => _pipeline?.BlendState;
			set
			{
				if (_pipeline.BlendState != null)
					_pipeline.BlendState = value;
				else
					_log.Error("Pipeline is null");
			}
		}
		public RasterizerState RasterizerState
		{
			get => _pipeline?.RasterizerState;
			set
			{
				if (_pipeline.RasterizerState != null)
					_pipeline.RasterizerState = value;
				else
					_log.Error("Pipeline is null");
			}
		}
		public DepthStencilState DepthStencilState
		{
			get => _pipeline?.DepthStencilState;
			set
			{
				if (_pipeline.DepthStencilState != null)
					_pipeline.DepthStencilState = value;
				else
					_log.Error("Pipeline is null");
			}
		}
		private RenderTarget2D _postProcessTarget;
		private SpriteBatch _spriteBatch;
		private bool _saveScreen = false;


		/// <summary>
		/// Constructor.
		/// </summary>
		public Camera(string name, MaterialProfile matProfile = MaterialProfile.Default)
			: base()
		{
			ViewMatrix = Matrix.Identity;
			CameraType = CameraType.Perspective;

#if DEBUG_UI
			DebugUIPageName = $"Camera: {name}";
			ShevaGame.Instance.DebugUI.AddDebugPage(this);	
#endif

			_spriteBatch = new SpriteBatch(ShevaGame.Instance.GraphicsDevice);
            _pipeline = new RenderingPipeline("Camera pipeline")
            {
                Profile = matProfile,
            };
        }
		
		/// <summary>
		/// Look at.
		/// </summary>
		public void LookAt(Vector3 position, Vector3 target, Vector3 up)
		{
			Position = position;
			Target = target;
			Up = up;

			ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
		}		

		/// <summary>
		/// Create matrices.
		/// </summary>        
		private void CreateProjectionMatrix()
		{
			switch (CameraType)
			{
				case CameraType.Perspective:
					ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, Size.Width / (float)Size.Height, NearPlane, FarPlane);
					break;
				case CameraType.Orthographic:
					ProjectionMatrix = Matrix.CreateOrthographic(OrthographicSize.Width, OrthographicSize.Height, NearPlane, FarPlane);
					break;				
			}		
		}

		/// <summary>
		/// Draw.
		/// </summary>
		public void Draw(IScene scene, GameTime gameTime, RenderTarget2D renderTarget, RenderTarget2D depthTarget = null)
		{
			// Get visible objects.
			_pipeline.Clear();
			_pipeline.GameTime = gameTime;
			_pipeline.SetCamera(this);

			scene.GetVisibleObjects(_pipeline);

			// Update shadows.
			if (_pipeline.Profile == MaterialProfile.Default)
				foreach (Light light in _pipeline.Lights)
					light.Shadow?.Update(gameTime, scene, light, this);

			// Attach render targets.			
			ShevaGame.Instance.GraphicsDevice.SetRenderTarget(renderTarget);
			
			ShevaGame.Instance.GraphicsDevice.Clear(
				ClearOptions.Target | ClearOptions.DepthBuffer, ClearColor, 1, 0);

			if (depthTarget != null)
			{
				ShevaGame.Instance.GraphicsDevice.SetRenderTarget(depthTarget);
			
				ShevaGame.Instance.GraphicsDevice.Clear(
					ClearOptions.Target, new Color(0,0,0,0), 1, 0);

				ShevaGame.Instance.GraphicsDevice.SetRenderTargets(new[]
				{
					new RenderTargetBinding(renderTarget),
					new RenderTargetBinding(depthTarget)
				});
			}			
			
			// Render scene.
			_pipeline.Draw();

			ShevaGame.Instance.GraphicsDevice.SetRenderTarget(null);

			// Apply post processes.
			if (PostProcesses.Count > 0)
			{					
				if (_postProcessTarget == null ||
					_postProcessTarget.Width != renderTarget.Width ||
					_postProcessTarget.Height != renderTarget.Height)
				{
					_postProcessTarget?.Dispose();

					_postProcessTarget = new RenderTarget2D(
						ShevaGame.Instance.GraphicsDevice,
						renderTarget.Width,
						renderTarget.Height,
						false,
						renderTarget.Format,
						DepthFormat.None);						
				}

				bool postProcessAsTarget = true;

				if (_saveScreen)
					SaveImage(renderTarget, "Pure image");

				foreach (PostProcess postProcess in PostProcesses)
				{
					if (postProcess.Enabled)
					{
						if (postProcessAsTarget)
						{
							postProcess.InputTexture = renderTarget;
							postProcess.DepthTexture = depthTarget;
						
							ShevaGame.Instance.GraphicsDevice.SetRenderTarget(_postProcessTarget);
						}
						else
						{
							postProcess.InputTexture = _postProcessTarget;
							postProcess.DepthTexture = depthTarget;
						
							ShevaGame.Instance.GraphicsDevice.SetRenderTarget(renderTarget);						
						}

						ShevaGame.Instance.GraphicsDevice.Clear(
							ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1, 0);

						postProcess.Apply(this, gameTime, scene);
											
						ShevaGame.Instance.GraphicsDevice.SetRenderTarget(null);

						if (_saveScreen)
						{
							if (postProcessAsTarget)
								SaveImage(_postProcessTarget, postProcess.GetType().Name.ToString());
							else
								SaveImage(renderTarget, postProcess.GetType().Name.ToString());
						}

						postProcessAsTarget = !postProcessAsTarget;
					}
				}

				if (PostProcesses.Where(item => item.Enabled).Count() % 2 != 0)
				{
					ShevaGame.Instance.GraphicsDevice.SetRenderTarget(renderTarget);
					
					_spriteBatch.Begin();
					_spriteBatch.Draw(_postProcessTarget, new Rectangle(0,0,renderTarget.Width, renderTarget.Height), Color.White);
					_spriteBatch.End();

					ShevaGame.Instance.GraphicsDevice.SetRenderTarget(null);
				}
			}		

			if (_saveScreen)
			{				
				_saveScreen = false;

				SaveImage(renderTarget);				
			}
		}

		/// <summary>
		/// Save image.
		/// </summary>
		private void SaveImage(RenderTarget2D target, string name = null)
		{
			string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                "Screens");

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
				
			string realName = name == null ? $"screen{DateTime.Now.Ticks.ToString()}.png" : name + ".png";

			using (Stream stream = File.Create(Path.Combine(directory, realName))) 
				target.SaveAsPng(stream, target.Width, target.Height );							
		}

#if DEBUG_UI
        /// <summary>
        /// DebugUI.
        /// </summary>
        public virtual void DebugUI()
        {		
			if (ImGuiNET.ImGui.TreeNode($"Settings"))
            {   				
				Vector3 temp = ClearColor.ToVector3();				
				ImGuiNET.ImGui.ColorEdit3("Clear color", ref temp);
				ClearColor = new Color(temp);

				float tempSingle = FieldOfView;
				ImGuiNET.ImGui.SliderFloat("Field of view", ref tempSingle, 0.01f, 3.14f);
				FieldOfView = tempSingle;

				tempSingle = NearPlane;
				ImGuiNET.ImGui.SliderFloat("Near", ref tempSingle, 0.01f, FarPlane - 0.01f);
				NearPlane = tempSingle;

				tempSingle = FarPlane;
				ImGuiNET.ImGui.SliderFloat("Far", ref tempSingle, NearPlane + 0.01f, 10000);
				FarPlane = tempSingle;

				ImGuiNET.ImGui.TreePop();
			}
			

			if (ImGuiNET.ImGui.TreeNode($"Post Processes"))
            {   				
				foreach (PostProcess postProcess in PostProcesses)	
					if (ImGuiNET.ImGui.TreeNode(postProcess.GetType().Name))
            		{   				
						postProcess.DebugUI();

						ImGuiNET.ImGui.TreePop();
	        		}

				ImGuiNET.ImGui.TreePop();
	        }

			if (ImGuiNET.ImGui.TreeNode($"Tools"))
            {
				if (ImGuiNET.ImGui.Button("Save image"))
					_saveScreen = true;	

				ImGuiNET.ImGui.TreePop();
			}   				
		}
#endif
    }    
}
