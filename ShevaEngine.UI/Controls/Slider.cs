using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Slider class.
    /// </summary>	
    public class Slider : Control
	{        
        public BehaviorSubject<double> Value { get; }
        public BehaviorSubject<double> Minimum { get; }
        public BehaviorSubject<double> Maximum { get; }


        /// <summary>
		/// Constructor.
		/// </summary>
		public Slider()
		{
            IsSelectAble = true;

            Value = CreateProperty<double>(nameof(Value), 0);
            Minimum = CreateProperty<double>(nameof(Minimum), 0);
            Maximum = CreateProperty<double>(nameof(Maximum), 1);                       
		}

        /// <summary>
        /// Draw method.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            Foreground.Value?.Draw(spriteBatch,
                new Rectangle(
                    LocationSize.X,
                    LocationSize.Y,
                    (int)(LocationSize.Width * GetRatio()),
                    LocationSize.Height));
        }

        /// <summary>
        /// Get ratio.
        /// </summary>
        private double GetRatio()
        {
            return Value.Value / (Maximum.Value - Minimum.Value);
        }       
    }
}
