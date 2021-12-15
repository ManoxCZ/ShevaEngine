using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Content manager extended.
    /// </summary>
    public class ContentManagerEx : ContentManager
    {
        private readonly ILogger _log;        
        private object _lock = new object();

        /// <summary>
        /// Constructor.
        /// </summary>		
        public ContentManagerEx(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _log = ShevaServices.GetService<ILoggerFactory>().CreateLogger<ContentManagerEx>();            
        }

        /// <summary>
        /// Constructor.
        /// </summary>		
        public ContentManagerEx(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load.
        /// </summary>
        public override T Load<T>(string assetName)
        {
            try
            {
                lock (_lock)
                {
                    _log.LogInformation($"Loading: {assetName}, type: {typeof(T)}");

                    T output = base.Load<T>(assetName);

                    if ((object)output is Model model)
                    {
                        _log.LogInformation($"Updating materials");

                        MaterialsManager.UpdateMaterials(model);
                    }

                    if (output == null)
                    {
                        _log.LogWarning($"Can't load");
                    }
                    else
                    {
                        _log.LogInformation($"Successfully loaded");
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Can't load content item: {assetName}", ex);
            }

            return default!;
        }        
    }
}
