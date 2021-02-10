using System.Reactive.Subjects;


namespace ShevaEngine.Core
{	
	/// <summary>
	/// Game settings.
	/// </summary>
	public class GameSettings : ShevaGameSettings
	{		
		public BehaviorSubject<Resolution> Resolution { get; set; }
		public BehaviorSubject<bool> Fullscreen { get; set; }
		public BehaviorSubject<float> MusicVolume { get; set; }
		public BehaviorSubject<GraphicsQuality> GraphicsQuality { get; set; }
				

		/// <summary>
		/// Constructor.
		/// </summary>
		public GameSettings()
		{
			Resolution = Create(new Resolution(800,600));			
			Fullscreen = Create(false);						
			MusicVolume = Create(1.0f);	
			GraphicsQuality = Create(Core.GraphicsQuality.Medium);
		}
	}
}
