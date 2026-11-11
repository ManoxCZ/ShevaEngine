using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Graphics;
using System;
using System.Collections.Generic;

namespace ShevaEngine.Renderer
{
    /// <summary>
    /// Interface for the renderer.
    /// This will replace the legacy rendering system in the future.
    /// </summary>
    public interface IRenderer : IDisposable
    {
        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Gets the current camera.
        /// </summary>
        Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the current material profile.
        /// </summary>
        MaterialProfile Profile { get; set; }

        /// <summary>
        /// Gets the current game time.
        /// </summary>
        GameTime GameTime { get; set; }

        /// <summary>
        /// Gets the list of lights in the scene.
        /// </summary>
        IList<Light> Lights { get; }

        /// <summary>
        /// Gets or sets the blend state.
        /// </summary>
        BlendState BlendState { get; set; }

        /// <summary>
        /// Gets or sets the rasterizer state.
        /// </summary>
        RasterizerState RasterizerState { get; set; }

        /// <summary>
        /// Gets or sets the depth stencil state.
        /// </summary>
        DepthStencilState DepthStencilState { get; set; }

        /// <summary>
        /// Initializes the renderer.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Clears the renderer state.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears all lights from the scene.
        /// </summary>
        void ClearLights();

        /// <summary>
        /// Adds a light to the scene.
        /// </summary>
        void AddLight(Light light);

        /// <summary>
        /// Adds an object to be rendered.
        /// </summary>
        void AddObject(Model model, Matrix worldMatrix, AnimationsController? controller = null);

        /// <summary>
        /// Adds an object to be rendered.
        /// </summary>
        void AddObject(ModelMesh mesh, Matrix worldMatrix, AnimationsController? controller = null);

        /// <summary>
        /// Adds an object to be rendered.
        /// </summary>
        void AddObject(ModelMeshPart meshPart, Matrix worldMatrix, AnimationsController? controller = null);

        /// <summary>
        /// Adds multiple instances of an object to be rendered.
        /// </summary>
        void AddObject(Model model, IReadOnlyCollection<Matrix> worldMatrices);

        /// <summary>
        /// Adds multiple instances of an object to be rendered.
        /// </summary>
        void AddObject(ModelMesh mesh, IReadOnlyCollection<Matrix> worldMatrices);

        /// <summary>
        /// Adds multiple instances of an object to be rendered.
        /// </summary>
        void AddObject(ModelMeshPart meshPart, IReadOnlyCollection<Matrix> worldMatrices);

        /// <summary>
        /// Renders all queued objects.
        /// </summary>
        void Draw();

        /// <summary>
        /// Begins a render pass.
        /// </summary>
        void Begin();

        /// <summary>
        /// Ends the current render pass.
        /// </summary>
        void End();
    }
}
