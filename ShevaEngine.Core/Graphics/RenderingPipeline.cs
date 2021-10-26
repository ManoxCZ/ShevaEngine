using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ShevaEngine.Core
{
    /// <summary>
    /// Rendering pipeline.
    /// </summary>
    public class RenderingPipeline
    {
		private readonly ILogger _log;
		public GraphicsDevice Device => ShevaGame.Instance.GraphicsDevice;
		public string Name { get; }
		public Camera Camera { get; private set; }
        public MaterialProfile Profile { get; set; }
        public GameTime GameTime { get; set; }
		public List<Light> Lights { get; private set; } = new List<Light>();
        private readonly Dictionary<Effect, Dictionary<ModelMeshPart, List<Matrix>>> OpaqueDrawCalls = new Dictionary<Effect, Dictionary<ModelMeshPart, List<Matrix>>>();
		private readonly List<Tuple<Material, ModelMeshPart, Matrix>> TransparentDrawCalls = new List<Tuple<Material, ModelMeshPart, Matrix>>();
		private VertexBuffer _instancesBuffer;
		private int _instancesCount = 0;
		private int _instancesAllocatedCount = 0;
		public BlendState BlendState { get; set; } = BlendState.Opaque;
		public RasterizerState RasterizerState { get; set; } = RasterizerState.CullNone;
		public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.Default;



		/// <summary>
		/// Constructor.
		/// </summary>
		public RenderingPipeline(string name)
		{
			Name = name;
            
			_log = ShevaGame.Instance.LoggerFactory.CreateLogger($"{typeof(RenderingPipeline)} - {Name}");

            Profile = MaterialProfile.Default;
        }

		/// <summary>
		/// Clear method.
		/// </summary>
		public void Clear()
		{
			_log.LogDebug("Clear");

			ClearLights();

			TransparentDrawCalls.Clear();

			foreach (KeyValuePair<Effect, Dictionary<ModelMeshPart, List<Matrix>>> model in OpaqueDrawCalls)
			{
				foreach (KeyValuePair<ModelMeshPart, List<Matrix>> modelPart in model.Value)
				{
					modelPart.Value.Clear();
				}
			}
		}

		/// <summary>
		/// Clear lights.
		/// </summary>
		public void ClearLights()
		{
			_log.LogDebug("Lights cleared");

			Lights.Clear();
		}

		/// <summary>
		/// Set camera.
		/// </summary>		
		public void SetCamera(Camera camera)
		{
			_log.LogDebug("Set camera");
			Camera = camera;
		}		

		/// <summary>
		/// Add light
		/// </summary>		
		public void AddLight(Light light)
		{
			_log.LogDebug("Light added");

			Lights.Add(light);
		}

		/// <summary>
		/// Add object.
		/// </summary>
		public void AddObject(Model model, Matrix worldMatrix, AnimationsController? controller = null)
		{
			foreach (ModelMesh mesh in model.Meshes)
				AddObject(mesh, worldMatrix, controller);
		}

		/// <summary>
		/// Add object.
		/// </summary>
		public void AddObject(ModelMesh modelPart, Matrix worldMatrix, AnimationsController? controller = null)
		{
			int count = modelPart.MeshParts.Count;

			for (int i = 0; i < count; i++)
				AddObject(modelPart.MeshParts[i], worldMatrix, controller);							
		}

		/// <summary>
		/// Add object.
		/// </summary>
		public void AddObject(ModelMeshPart modelMeshPart, Matrix worldMatrix, AnimationsController? controller = null)
		{
			if (modelMeshPart.Effect is Material material)
			{
				if (Profile == MaterialProfile.Shadows && !material.CastShadows)
				{
					return;
				}

				if (material.Animated)
				{
					if (controller != null)
					{
						material.Bones = controller.GetTransforms(GameTime);
					}
					else
					{
						_log.LogError($"Material {material} is animated, but Animation Controller is null");

						if (material is TexturedMaterial textured)
							_log.LogError($"Used texture: {textured.Texture.Name}");
					}
				}
				
				if (material.Transparent)
				{
					TransparentDrawCalls.Add(new Tuple<Material, ModelMeshPart, Matrix>(
						material, modelMeshPart, worldMatrix));
				}
				else
				{
					if (!OpaqueDrawCalls.ContainsKey(modelMeshPart.Effect))
						OpaqueDrawCalls.Add(modelMeshPart.Effect, new Dictionary<ModelMeshPart, List<Matrix>>());

					Dictionary<ModelMeshPart, List<Matrix>> meshes = OpaqueDrawCalls[modelMeshPart.Effect];

					if (!meshes.ContainsKey(modelMeshPart))
						meshes.Add(modelMeshPart, new List<Matrix>());

					meshes[modelMeshPart].Add(worldMatrix);
				}
			}
		}

		/// <summary>
		/// Add object.
		/// </summary>
		public void AddObject(Model model, IEnumerable<Matrix> worldMatrices)
		{
			foreach (ModelMesh mesh in model.Meshes)
				AddObject(mesh, worldMatrices);
		}

		/// <summary>
		/// Add object.
		/// </summary>
		public void AddObject(ModelMesh modelPart, IEnumerable<Matrix> worldMatrices)
		{
			int count = modelPart.MeshParts.Count;

			for (int i = 0; i < count; i++)
				AddObject(modelPart.MeshParts[i], worldMatrices);
		}

		/// <summary>
		/// Add object.
		/// </summary>
		public void AddObject(ModelMeshPart modelMeshPart, IEnumerable<Matrix> worldMatrices)
		{
			if (modelMeshPart.Effect is ColoredMaterial material)
			{
                if (Profile == MaterialProfile.Shadows && !material.CastShadows)
                    return;

                if (material.Transparent)
				{
					foreach (Matrix worldMatrix in worldMatrices)
					{
						TransparentDrawCalls.Add(new Tuple<Material, ModelMeshPart, Matrix>(
							material, modelMeshPart, worldMatrix));
					}
				}
				else
				{
					if (!OpaqueDrawCalls.ContainsKey(modelMeshPart.Effect))
						OpaqueDrawCalls.Add(modelMeshPart.Effect, new Dictionary<ModelMeshPart, List<Matrix>>());

					Dictionary<ModelMeshPart, List<Matrix>> meshes = OpaqueDrawCalls[modelMeshPart.Effect];

					if (!meshes.ContainsKey(modelMeshPart))
						meshes.Add(modelMeshPart, new List<Matrix>());

					meshes[modelMeshPart].AddRange(worldMatrices);
				}
			}
		}

		/// <summary>
		/// Draw method.
		/// </summary>		
		public void Draw()
		{
			_log.LogDebug($"Draw started with {Profile} profile");

			try
			{
				Device.BlendState = BlendState;
				Device.RasterizerState = RasterizerState;
				Device.DepthStencilState = DepthStencilState;			

				foreach (KeyValuePair<Effect, Dictionary<ModelMeshPart, List<Matrix>>> effectMeshes in OpaqueDrawCalls)
				{
					foreach (KeyValuePair<ModelMeshPart, List<Matrix>> modelPart in effectMeshes.Value)
					{
						if (modelPart.Value.Count > 0)
						{
							if (effectMeshes.Key is MaterialWithLights material)
							{
								if (Profile != MaterialProfile.Shadows || material.CastShadows)
								{
									_log.LogDebug($"Applying material {material}");

									if (Profile != MaterialProfile.Shadows)
										material.Lights = Lights;

									material.Apply(Profile, Camera, (float)GameTime.TotalGameTime.TotalSeconds, modelPart.Key.VertexBuffer.VertexDeclaration);

									UpdateInstancesData(Device, modelPart.Value);

									Device.SetVertexBuffers(
											new VertexBufferBinding(modelPart.Key.VertexBuffer, 0, 0),
											new VertexBufferBinding(_instancesBuffer, 0, 1));
									Device.Indices = modelPart.Key.IndexBuffer;
									
									Device.DrawInstancedPrimitives(
										PrimitiveType.TriangleList, modelPart.Key.VertexOffset, modelPart.Key.StartIndex, modelPart.Key.PrimitiveCount, _instancesCount);
								}
							}
							else
								_log.LogError($"Invalid material: {effectMeshes.Key.GetType()}");						
						}
					}
				}

				Device.BlendState = BlendState.AlphaBlend;
				Device.DepthStencilState = DepthStencilState.DepthRead;				

				// Transparent materials, don't render to the shadow map.
				if (Profile != MaterialProfile.Shadows)
				{
					foreach (var transparentMesh in
						TransparentDrawCalls.OrderByDescending(item => Vector3.DistanceSquared(item.Item3.Translation, Camera.Position)))
					{
						transparentMesh.Item1.Apply(Profile, Camera, GameTime.TotalGameTime.Seconds, transparentMesh.Item2.VertexBuffer.VertexDeclaration);

						UpdateInstancesData(Device, new[] { transparentMesh.Item3 });

						Device.SetVertexBuffers(
								new VertexBufferBinding(transparentMesh.Item2.VertexBuffer, 0, 0),
								new VertexBufferBinding(_instancesBuffer, 0, 1));
						Device.Indices = transparentMesh.Item2.IndexBuffer;

						Device.DrawInstancedPrimitives(
							PrimitiveType.TriangleList, transparentMesh.Item2.VertexOffset, transparentMesh.Item2.StartIndex, transparentMesh.Item2.PrimitiveCount, _instancesCount);
					}
				}
			}
			catch (Exception ex)
			{
				_log.LogError(ex.Message, ex);
			}

			_log.LogDebug($"Draw ended");
		}

		/// <summary>
		/// Update instances buffer.
		/// </summary>        
		private void UpdateInstancesData(GraphicsDevice graphicsDevice, IList<Matrix> worldMatrices)
		{
			if (_instancesAllocatedCount < worldMatrices.Count)
			{
				_instancesAllocatedCount = worldMatrices.Count;

				_instancesBuffer = new VertexBuffer(graphicsDevice, typeof(VertexMatrix), _instancesAllocatedCount, BufferUsage.WriteOnly);				
			}

			_instancesBuffer.SetData(worldMatrices.ToArray());
			_instancesCount = worldMatrices.Count;
		}
	}   
}