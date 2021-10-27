using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Content manager extended.
    /// </summary>
    public class ContentManagerEx : ContentManager
	{
        private readonly ILogger _log;
        private readonly ShevaGame _game;
        private object _lock = new object();

		/// <summary>
		/// Constructor.
		/// </summary>		
		public ContentManagerEx(ShevaGame game, IServiceProvider serviceProvider) 
			: base(serviceProvider)
		{
            _log = game.LoggerFactory.CreateLogger<ContentManagerEx>();
            _game = game;            
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
                        _log.LogWarning($"Can't load");

                    _log.LogInformation($"Successfully loaded");

                    return output;
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Can't load content item: {assetName}", ex);
            }

            return default;
		}

        /// <summary>
        /// Open stream method.
        /// </summary>
        public Stream OpenStream(string assetName) => base.OpenStream(assetName);
    }
}
