using System;


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
    }
}
