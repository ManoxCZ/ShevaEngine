using System;
using System.Reactive.Linq;
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

            Initialize();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            Disposables.Add(Resolution.DistinctUntilChanged().Subscribe(item => Save(this)));
            Disposables.Add(Fullscreen.DistinctUntilChanged().Subscribe(item => Save(this)));
            Disposables.Add(MusicVolume.DistinctUntilChanged().Subscribe(item => Save(this)));
            Disposables.Add(GraphicsQuality.DistinctUntilChanged().Subscribe(item => Save(this)));
        }
	}
}
