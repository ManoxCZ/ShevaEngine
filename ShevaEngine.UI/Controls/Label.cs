using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using ShevaEngine.UI.Brushes;
using System;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Label.
    /// </summary>	
    public class Label : Control
    {		
		protected BehaviorSubject<Font> Font { get; private set; }
		public BehaviorSubject<FontSize> FontSize { get; set; }		
		public BehaviorSubject<string> Text { get; set; }		
		private Vector2 _textPosition;


		/// <summary>
		/// Constructor.
		/// </summary>
		public Label()
		{
			Text = CreateProperty(nameof(Text), string.Empty);
            Font = CreateProperty(nameof(Font), Core.Font.Default);
            FontSize = CreateProperty(nameof(FontSize), Core.FontSize.Size12);						
			
			Disposables.Add(Text.Subscribe(item =>
			{
				Resize(LocationSize - Margin.Value);
			}));			
		}		

		/// <summary>
		/// Draw method.
		/// </summary>        
		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			base.Draw(spriteBatch, gameTime);

			if (Font.Value != null && Text.Value != null)
			{
                if (Foreground.Value is SolidColorBrush brush)
                {
                    spriteBatch.DrawString(Font.Value[FontSize.Value], Text.Value.ToString(), _textPosition + new Vector2(1, 1),
                        new Color(Color.DarkGray.ToVector4() * brush.Color.Value.ToVector4()));
                    spriteBatch.DrawString(Font.Value[FontSize.Value], Text.Value.ToString(), _textPosition, brush.Color.Value);
                }
			}
		}

		/// <summary>
		/// Method resize control.
		/// </summary>
		public override void Resize(Rectangle locationSize)
        {
			base.Resize(locationSize);

			Vector2 size = GetTextSize();
            Margin margin = Margin.Value;

            int newX = locationSize.X + margin.Left;
            int newY = locationSize.Y + margin.Bottom;

            switch (HorizontalAlignment.Value)
            {
                case UI.HorizontalAlignment.Left:
                    newX = locationSize.X + margin.Left;
                    break;
                case UI.HorizontalAlignment.Center:
                case UI.HorizontalAlignment.Stretch:
                    newX = (int)(locationSize.X + margin.Left + (locationSize.Width - size.X - margin.Left - margin.Right) / 2);
                    break;
                case UI.HorizontalAlignment.Right:
                    newX = (int)(locationSize.X + margin.Left + (locationSize.Width - size.X - margin.Left - margin.Right));
                    break;
            }

            switch (VerticalAlignment.Value)
            {
                case UI.VerticalAlignment.Top:
                    newY = locationSize.Y + margin.Bottom;
                    break;
                case UI.VerticalAlignment.Center:
                case UI.VerticalAlignment.Stretch:
                    newY = (int)(locationSize.Y + margin.Bottom + (locationSize.Height - size.Y - margin.Bottom - margin.Top) / 2);
                    break;
                case UI.VerticalAlignment.Bottom:
                    newY = (int)(locationSize.Y + margin.Bottom + (locationSize.Height - size.Y - margin.Bottom - margin.Top));
                    break;
            }

			_textPosition = new Vector2(newX, newY);
        }

        /// <summary>
        /// Get text size.
        /// </summary>        
        public Vector2 GetTextSize()
        {
			if (Font.Value == null || Text.Value == null)
				return Vector2.Zero;

            return Font.Value[FontSize.Value].MeasureString(Text.Value.ToString());
        }
    }
}
