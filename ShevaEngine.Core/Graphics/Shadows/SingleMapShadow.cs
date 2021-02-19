using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Single map shadow.
	/// </summary>
	public class SingleMapShadow : Shadow
	{
		public Resolution Size { get; set; } = new Resolution(512, 512);		
		public RenderTarget2D ShadowMap { get; private set; }
		private Camera _camera;


		/// <summary>
		/// Get light viewProj matrix.
		/// </summary>		
		public override Matrix GetLightViewProjMatrix()
		{
			return _camera.ViewMatrix * _camera.ProjectionMatrix;
		}

		/// <summary>
		/// Update method.
		/// </summary>
		public override void Update(GameTime gameTime, IScene scene, Light light, Camera camera)
		{
			if (ShadowMap == null || ShadowMap.Width != Size.Width || ShadowMap.Height != Size.Height)
			{
				ShadowMap?.Dispose();

				ShadowMap = new RenderTarget2D(ShevaGame.Instance.GraphicsDevice,
					Size.Width, Size.Height, false, SurfaceFormat.Single, DepthFormat.Depth16);

				_camera = new Camera("ShadowMap")
				{
					CameraType = CameraType.Orthographic,
					Size = Size,
					ClearColor = Color.White,
					NearPlane = camera.NearPlane,
					FarPlane = camera.FarPlane,
					FieldOfView = camera.FieldOfView,
					RasterizerState = RasterizerState.CullCounterClockwise
				};
			}

			ShevaGame.Instance.GraphicsDevice.SetRenderTarget(ShadowMap);

			_camera.LookAt(light.GetLightCameraPosition(camera), camera.Target, camera.Up);

			float dim = 2 * (float)(Vector3.Distance(camera.Position, camera.Target) * Math.Tan(camera.FieldOfView));

			_camera.OrthographicSize = (dim, dim);

			_camera.Draw(MaterialProfile.Shadows, scene, gameTime, ShadowMap);

			ShevaGame.Instance.GraphicsDevice.SetRenderTarget(null);
		}
	}
}
