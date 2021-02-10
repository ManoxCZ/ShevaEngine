using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Button class.
	/// </summary>	
	public abstract class Slider<T> : Control
	{
        public BehaviorSubject<Color> ForeColor { get; set; }
        public BehaviorSubject<T> Value { get; set; }
        public BehaviorSubject<T> Min { get; set; }
        public BehaviorSubject<T> Max { get; set; }


        /// <summary>
		/// Constructor.
		/// </summary>
		public Slider()
		{
            IsSelectAble = true;

			ForeColor = new BehaviorSubject<Color>(Color.Black);
			Disposables.Add(ForeColor);

            Value = new BehaviorSubject<T>(default(T));
			Disposables.Add(Value);

            Min = new BehaviorSubject<T>(default(T));
			Disposables.Add(Min);

            Max = new BehaviorSubject<T>(default(T));
			Disposables.Add(Max);

            Disposables.Add(Click.Subscribe(item => 
            {
                SetRatio(item.X / (float)LocationSize.Width);
            }));
		}

        /// <summary>
        /// Draw method.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (BackColor.Value != Color.Transparent)
				DrawRectangle(spriteBatch, LocationSize, BackColor.Value);

            if (ForeColor.Value != Color.Transparent)
				DrawRectangle(spriteBatch, 
                    new Rectangle(
                        LocationSize.X, 
                        LocationSize.Y, 
                        (int)(LocationSize.Width * GetRatio()),
                        LocationSize.Height), ForeColor.Value);
        }

        /// <summary>
        /// Get ratio.
        /// </summary>
        protected abstract float GetRatio();      

        /// <summary>
        /// Set ratio.
        /// </summary>
        protected abstract void SetRatio(float ratio);
	}
}
