using Microsoft.Xna.Framework;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Change color animation.
	/// </summary>
	public class ChangeTextAnimation : Animation<object>
	{
		public object Text { get; private set; }

		/// <summary>
		/// Change color animation.
		/// </summary>
		public ChangeTextAnimation(SubjectBase<object> item, object text)
			: base(item)
		{
			Text = text;
		}

		/// <summary>
		/// Update.
		/// </summary>		
		public override void Update(GameTime time)
		{
			base.Update(time);

			Item?.OnNext(Text);
		}
	}
}
