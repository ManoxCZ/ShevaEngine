using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Game settings.
    /// </summary>
    public class ShevaGameSettings : IDisposable
    {        
        protected List<IDisposable> Disposables;


        /// <summary>
        /// Constructor.
        /// </summary>
        public ShevaGameSettings()
        {
            Disposables = new List<IDisposable>();            
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public virtual void Initialize()
        {
            
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
                disposable?.Dispose();

            Disposables.Clear();
            Disposables = null!;
        }

        /// <summary>
        /// Create new property.
        /// </summary>
        public BehaviorSubject<T> Create<T>(T value)
        {
            BehaviorSubject<T> result = new BehaviorSubject<T>(value);
            Disposables.Add(result);

            return result;
        }

        /// <summary>
        /// Method loads settings.
        /// </summary>
        public static T Load<T>() where T : ShevaGameSettings, new()
        {
            ILogger log = ShevaServices.GetService<ILoggerFactory>().CreateLogger(typeof(T));
            string fileContent = ShevaServices.GetService<IFileSystemService>().ReadFileContent($"{typeof(T).Name}.settings");
            
            if (string.IsNullOrEmpty(fileContent))
            {
                log.LogWarning($"File {typeof(T).Name}.settings not found");
                
                return new T();
            }

            try
            {
                T result = Serializer.Deserialize<T>(fileContent);

                result.Initialize();

                return result;
            }
            catch (Exception exception)
            {
                log.LogError(exception, $"Settings deserialization error");                
            }

            return new T();
        }

        /// <summary>
        /// Method saves settings.
        /// </summary>
        public static void Save<T>(T settings)
        {
            ShevaServices.GetService<IFileSystemService>().WriteFileContent($"{typeof(T).Name}.settings", Serializer.Serialize(settings));
        }
    }
}
