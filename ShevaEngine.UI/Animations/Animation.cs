using Microsoft.Xna.Framework;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Animation.
	/// </summary>
	public abstract class Animation<T> : IAnimation
	{
		public SubjectBase<T> Item { get; private set; }
		public double StartTime { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>		
		public Animation(SubjectBase<T> item)
		{
			Item = item;
		}

		/// <summary>
		/// Start.
		/// </summary>		
		public virtual void Start(GameTime time)
		{
			StartTime = time.TotalGameTime.TotalSeconds;
		}

		/// <summary>
		/// Update.
		/// </summary>		
		public virtual void Update(GameTime time)
		{

		}
	}
}
