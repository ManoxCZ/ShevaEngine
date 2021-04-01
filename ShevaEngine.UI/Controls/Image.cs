using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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


		/// <summary>
		/// Constructor.
		/// </summary>
		public Image()
			: base()
		{			
            Brush = CreateProperty<Brush>(nameof(Brush), null);
		}

		/// <summary>
		/// Load content.
		/// </summary>        
		public override void LoadContent(ContentManager contentManager)
		{
			base.LoadContent(contentManager);

            Disposables.Add(Brush.Subscribe(item =>
            {
                item?.LoadContent(contentManager);
            }));            
		}

		/// <summary>
		/// Method Draw().
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
            Brush.Value?.Draw(spriteBatch, LocationSize - Margin);

			base.Draw(spriteBatch, gameTime);
		}
	}
}
