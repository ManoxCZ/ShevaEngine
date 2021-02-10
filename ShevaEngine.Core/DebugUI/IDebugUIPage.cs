

namespace ShevaEngine.Core
{
    /// <summary>
    /// DebugUI interface.
    /// </summary>
    public interface IDebugUIPage
    {        
        /// <summary>
        /// Name.
        /// </summary>        
        string DebugUIPageName { get; }

        /// <summary>
        /// DebugUI.
        /// </summary>
        void DebugUI();
    }
}