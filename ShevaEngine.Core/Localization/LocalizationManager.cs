using System;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Localization manager.
    /// </summary>
    public class LocalizationManager
    {
        public static readonly LocalizationManager Instance = new LocalizationManager();

#if WINDOWS_UAP
        private Windows.ApplicationModel.Resources.ResourceLoader _resourceManager;
#else
        private readonly System.Resources.ResourceManager _resourceManager;
#endif


        /// <summary>
        /// Constructor.
        /// </summary>		
        public LocalizationManager()
        {
#if WINDOWS_UAP
            _resourceManager = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
#elif WINDOWS
            _resourceManager = new System.Resources.ResourceManager("Project00-Windows.Resources", System.Reflection.Assembly.GetEntryAssembly());
#elif DESKTOPGL
			_resourceManager = new System.Resources.ResourceManager("Project00-DesktopGL.Resources", System.Reflection.Assembly.GetEntryAssembly());
#endif
        }

        /// <summary>
        /// Get value.
        /// </summary>		
        public string GetValue(string key)
        {
            string localizedString = _resourceManager.GetString(key);

            if (localizedString != null)
                return localizedString;

            return $"${key}";
        }
    }
}
