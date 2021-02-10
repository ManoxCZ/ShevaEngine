using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Label.
	/// </summary>	
	public class Label : Control
    {
		public string FontName { get; set; } = "Poppins-Italic";
		protected Font Font { get; private set; }
		public BehaviorSubject<FontSize> FontSize { get; set; }
		public BehaviorSubject<Color> ForeColor { get; set; }
		public BehaviorSubject<object> Text { get; set; }		
		private Vector2 _textPosition;


		/// <summary>
		/// Constructor.
		/// </summary>
		public Label()
		{
			Text = new BehaviorSubject<object>(String.Empty);
			Disposables.Add(Text);

			FontSize = new BehaviorSubject<FontSize>(ShevaEngine.Core.FontSize.Size12);
			Disposables.Add(FontSize);

			ForeColor = new BehaviorSubject<Color>(Color.Black);
			Disposables.Add(ForeColor);

			Disposables.Add(Text.Subscribe(item =>
			{
				Resize(LocationSize - Margin);
			}));			
		}

		/// <summary>
		/// Load content.
		/// </summary>		
		public override void LoadContent(ContentManager contentManager)
		{
			if (!string.IsNullOrEmpty(FontName))
				Font = contentManager.Load<Font>($@"Content\Fonts\{FontName}");

			base.LoadContent(contentManager);
		}

		/// <summary>
		/// Draw method.
		/// </summary>        
		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			base.Draw(spriteBatch, gameTime);

			if (Text.Value != null)
			{
				spriteBatch.DrawString(Font[FontSize.Value], Text.Value.ToString(), _textPosition + new Vector2(1,1), 
					new Color(Color.DarkGray.ToVector4() * ForeColor.Value.ToVector4()));
				spriteBatch.DrawString(Font[FontSize.Value], Text.Value.ToString(), _textPosition, ForeColor.Value);
			}
		}

		/// <summary>
		/// Method resize control.
		/// </summary>
		public override void Resize(Rectangle locationSize)
        {
			base.Resize(locationSize);

			Vector2 size = GetTextSize();

            int newX = locationSize.X + Margin.Left;
            int newY = locationSize.Y + Margin.Bottom;

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    newX = locationSize.X + Margin.Left;
                    break;
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    newX = (int)(locationSize.X + Margin.Left + (locationSize.Width - size.X - Margin.Left - Margin.Right) / 2);
                    break;
                case HorizontalAlignment.Right:
                    newX = (int)(locationSize.X + Margin.Left + (locationSize.Width - size.X - Margin.Left - Margin.Right));
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    newY = locationSize.Y + Margin.Bottom;
                    break;
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    newY = (int)(locationSize.Y + Margin.Bottom + (locationSize.Height - size.Y - Margin.Bottom - Margin.Top) / 2);
                    break;
                case VerticalAlignment.Bottom:
                    newY = (int)(locationSize.Y + Margin.Bottom + (locationSize.Height - size.Y - Margin.Bottom - Margin.Top));
                    break;
            }

			_textPosition = new Vector2(newX, newY);
        }

        /// <summary>
        /// Get text size.
        /// </summary>        
        public Vector2 GetTextSize()
        {
			if (Font == null || Text.Value == null)
				return Vector2.Zero;

            return Font[FontSize.Value].MeasureString(Text.Value.ToString());
        }
    }
}
