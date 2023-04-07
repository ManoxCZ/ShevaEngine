using System;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Scene base class.
    /// </summary>
    public interface IScene : IDisposable
    {
        /// <summary>
        /// Get visible objects method.
        /// </summary>
        void GetVisibleObjects(RenderingPipeline pipeline);

        /// <summary>
        /// Get lights
        /// </summary>
        IReadOnlyCollection<Light> GetLights();
    }
}
