using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
		/// Method Draw().
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
            Brush.Value?.Draw(spriteBatch, LocationSize - Margin.Value);

			base.Draw(spriteBatch, gameTime);
		}
	}
}
