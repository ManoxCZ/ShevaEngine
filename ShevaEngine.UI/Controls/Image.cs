using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Image.
    /// </summary>
    public class Image : Control
	{		
		public BehaviorSubject<Brush> Brush { get; }
        public BehaviorSubject<Texture2D> Source { get; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Image()
			: base()
		{			
            Brush = CreateProperty<Brush>(nameof(Brush), null);
            Source = CreateProperty<Texture2D>(nameof(Source), null);

            Disposables.Add(Source.Subscribe(item =>
            {
                if (item == null)
                    Brush.OnNext(null);
                else
                    Brush.OnNext(new ImageBrush(item));
            }));
		}

		/// <summary>
		/// Method Draw().
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
            Brush.Value?.Draw(spriteBatch, LocationSize - Margin.Value);

			base.Draw(spriteBatch, gameTime);
		}
	}
}
