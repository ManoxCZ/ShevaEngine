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
        public BehaviorSubject<string> Source { get; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Image()
			: base()
		{			
            Brush = CreateProperty<Brush>(nameof(Brush), null);
            Source = CreateProperty<string>(nameof(Source), null);

            Disposables.Add(Source.Subscribe(item =>
            {
                if (string.IsNullOrEmpty(item))
                    Brush.OnNext(null);
                else
                    Brush.OnNext(new ImageBrush(System.IO.Path.ChangeExtension(item, null)));
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
