using System;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Button class.
    /// </summary>	
    public class Button : Control
	{
        public BehaviorSubject<ICommand> Command { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public Button()
			: base()
		{
			IsSelectAble = true;

            Command = CreateProperty<ICommand>(nameof(Command), null);

            Click.Subscribe(item =>
            {
                Command.Value?.Execute(null);
            });  
		}
	}
}
