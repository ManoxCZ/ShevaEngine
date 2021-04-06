﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.UI;
using System;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Content manager extended.
	/// </summary>
	public class ContentManagerEx : ContentManager
	{
		private readonly Log _log = new Log(typeof(ContentManagerEx));
        private readonly ShevaGame _game;
        private object _lock = new object();
		private SortedDictionary<string,Font> _fonts = new SortedDictionary<string, Font>();


		/// <summary>
		/// Constructor.
		/// </summary>		
		public ContentManagerEx(ShevaGame game, IServiceProvider serviceProvider) 
			: base(serviceProvider)
		{
            _game = game;
		}

		/// <summary>
		/// Constructor.
		/// </summary>		
		public ContentManagerEx(IServiceProvider serviceProvider, string rootDirectory)
			: base(serviceProvider, rootDirectory)
		{
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
                    _log.Info($"Loading: {assetName}, type: {typeof(T)}");

                    if (typeof(T) == typeof(Font))
                    {
                        if (!_fonts.ContainsKey(assetName))
                        {
                            Font newFont = new Font(assetName);
                            newFont.LoadContent(this);

                            _fonts.Add(assetName, newFont);
                        }

                        return (T)(object)_fonts[assetName];
                    }

                    if (typeof(T) == typeof(Control))
                    {
                        string data = base.Load<string>(assetName);

                        return (T)(object)XamlImporter.Import(_game.UIStyle, data);
                    }

                    T output = base.Load<T>(assetName);

                    if ((object)output is Model model)
                    {
                        _log.Info($"Updating materials");

                        MaterialsManager.UpdateMaterials(model);
                    }

                    if (output == null)
                        _log.Warning($"Can't load");

                    _log.Info($"Successfully loaded");

                    return output;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Can't load content item: {assetName}", ex);
            }

            return default;
		}
	}
}
