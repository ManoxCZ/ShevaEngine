using ShevaEngine.Core;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Properties class.
    /// </summary>
    public class PropertiesClass : IDisposable
    {
        protected readonly Log Log;
        private List<IDisposable> _disposables;
        private SortedDictionary<string, BehaviorSubject<object>> _properties;
        private SortedDictionary<string, Type> _propertyTypes;


        /// <summary>
        /// Constructor.
        /// </summary>
        public PropertiesClass()
        {
            Log = new Log(GetType());

            _disposables = new List<IDisposable>();
            _properties = new SortedDictionary<string, BehaviorSubject<object>>();
            _propertyTypes = new SortedDictionary<string, Type>();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (IDisposable disposable in _disposables)
                disposable?.Dispose();

            _disposables.Clear();
        }

        /// <summary>
        /// Method creates new member.
        /// </summary>
        protected BehaviorSubject<T> CreateProperty<T>(string propertyName, T value)
        {
            BehaviorSubject<T> instance = new BehaviorSubject<T>(value);

            string propertyNameLower = propertyName.ToLower();

            BehaviorSubject<object> temp = new BehaviorSubject<object>(value);
            _disposables.Add(temp);

            _disposables.Add(temp.DistinctUntilChanged().Subscribe(item =>
            {
                instance.OnNext((T)item);
            }));

            _disposables.Add(instance.DistinctUntilChanged().Subscribe(item =>
            {
                temp.OnNext(item);
            }));

            _properties.Add(propertyNameLower, temp);
            _propertyTypes.Add(propertyNameLower, typeof(T));

            _disposables.Add(instance);

            return instance;
        }

        /// <summary>
        /// Has property.
        /// </summary>        
        public bool HasProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName.ToLower());
        }

        /// <summary>
        /// Set property value.
        /// </summary>
        public bool SetPropertyValue(string propertyName, object value)
        {
            string propertyNameLower = propertyName.ToLower();

            if (!HasProperty(propertyNameLower))
                return false;

            _properties[propertyNameLower].OnNext(value);

            return true;
        }

        /// <summary>
        /// Subscribe property value.
        /// </summary>
        public IDisposable Subscribe(string propertyName, Action<object> onNext)
        {
            string propertyNameLower = propertyName.ToLower();

            if (!HasProperty(propertyNameLower))
                return default;

            return _properties[propertyNameLower]
                .DistinctUntilChanged()
                .Subscribe(onNext);
        }

        /// <summary>
        /// Get property type.
        /// </summary>
        /// <returns></returns>
        public Type GetPropertyType(string propertyName)
        {
            string propertyNameLower = propertyName.ToLower();

            if (!HasProperty(propertyNameLower))
                return default;

            return _propertyTypes[propertyNameLower];
        }
    }
}
