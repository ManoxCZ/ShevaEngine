using Microsoft.Xna.Framework;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Change color animation.
	/// </summary>
	public class ChangeColorAnimation : Animation<Color>
	{
		public Color Color { get; private set; }

		/// <summary>
		/// Change color animation.
		/// </summary>
		public ChangeColorAnimation(SubjectBase<Color> item, Color color)
			: base(item)
		{
			Color = color;
		}

		/// <summary>
		/// Update.
		/// </summary>		
		public override void Update(GameTime time)
		{
			base.Update(time);

			Item.OnNext(Color);
		}
	}
}
