using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public BehaviorSubject<Brush> Foreground { get; }
        public BehaviorSubject<T> Value { get; set; }
        public BehaviorSubject<T> Min { get; set; }
        public BehaviorSubject<T> Max { get; set; }


        /// <summary>
		/// Constructor.
		/// </summary>
		public Slider()
		{
            IsSelectAble = true;

            Foreground = new BehaviorSubject<Brush>(null);
			Disposables.Add(Foreground);

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

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            Disposables.Add(Foreground.Subscribe(item =>
            {
                item?.LoadContent(contentManager);
            }));
        }

        /// <summary>
        /// Draw method.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Foreground.Value?.Draw(spriteBatch, new Rectangle(
                        LocationSize.X,
                        LocationSize.Y,
                        (int)(LocationSize.Width * GetRatio()),
                        LocationSize.Height));            
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
