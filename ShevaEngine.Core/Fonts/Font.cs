using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Font class.
    /// </summary>
    public class Font
    {
        public static string DefaultFontName = @"Content\Fonts\Poppins-Italic";
        public static Font Default { get; private set; } = ShevaGame.Instance.Content.Load<Font>(DefaultFontName);

        public string Name { get; }
        public SpriteFont[] _sprites;

        public SpriteFont this [FontSize size]
        {
            get => _sprites[(int)size];
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Font(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Load content.
        /// </summary>        
        public void LoadContent(ContentManager content)
        {
            _sprites = Enum.GetValues(typeof(FontSize)).OfType<FontSize>().Select(
                item => content.Load<SpriteFont>(Name + "-" + item.ToString())).ToArray();
        }
    }
}