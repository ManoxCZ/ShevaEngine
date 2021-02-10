using Microsoft.Xna.Framework;
using System;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	public class PulseColorAnimation : Animation<Color>
	{
		public Color Color1 { get; private set; }
		public Color Color2 { get; private set; }
		public float Speed { get; private set; }


		/// <summary>
		/// Constructor.
		/// </summary>		
		public PulseColorAnimation(SubjectBase<Color> color, Color color1, Color color2, float speed)
			: base(color)
		{
			Color1 = color1;
			Color2 = color2;
			Speed = speed;
		}

		/// <summary>
		/// Update method.
		/// </summary>		
		public override void Update(GameTime time)
		{
			float amount = (float)(Math.Sin((time.TotalGameTime.TotalSeconds - StartTime) * Speed) + 1.0f) / 2.0f;

			Color color = Color.Lerp(Color1, Color2, amount);

			Item.OnNext(color);
		}
	}
}
