using System;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Checkbox.
	/// </summary>
	public class Checkbox : Button
	{
		public BehaviorSubject<bool> IsChecked { get; private set; }


		/// <summary>
		/// Constructor.
		/// </summary>
		public Checkbox()
		{
            IsChecked = CreateProperty(nameof(IsChecked), false);
			
			Disposables.Add(Click.Subscribe(item =>
			{
				bool isChecked = (Flags & ControlFlag.Checked) == ControlFlag.Checked;
			
				IsChecked.OnNext(!isChecked);
			}));

			Disposables.Add(IsChecked.Subscribe(item =>
			{
				if (!item)
					Flags &= ~ControlFlag.Checked;
				else
					Flags |= ControlFlag.Checked;				
			}));			
		}
	}
}
