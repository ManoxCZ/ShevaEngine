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
            string fileContent = Filesystem.ReadFileContent($"{typeof(T).Name}.settings");

            if (string.IsNullOrEmpty(fileContent))
            {
                //LogManager.Instance.AddLogMessage(new LogMessage()
                //{
                //    DateTime = DateTime.Now,
                //    Exception = null,
                //    Message = $"File {typeof(T).Name}.settings not found",
                //    Origin = nameof(ShevaGameSettings),
                //    Severity = LogSeverity.Warning
                //});

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
                //LogManager.Instance.AddLogMessage(new LogMessage()
                //{
                //    DateTime = DateTime.Now,
                //    Exception = exception,
                //    Message = "Settings deserialization error",
                //    Origin = nameof(ShevaGameSettings),
                //    Severity = LogSeverity.Error
                //});
            }

            return new T();
        }

        /// <summary>
        /// Method saves settings.
        /// </summary>
        public static void Save<T>(T settings)
        {
            Filesystem.WriteFileContent($"{typeof(T).Name}.settings", Serializer.Serialize(settings));
        }
    }
}
