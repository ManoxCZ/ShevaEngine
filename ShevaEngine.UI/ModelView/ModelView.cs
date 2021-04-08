using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Model view.
    /// </summary>
    public class ModelView : IDisposable
    {
        protected List<IDisposable> Disposables { get; }
        private SortedDictionary<string, object> _properties;
        private SortedDictionary<string, Type> _propertyTypes;


        /// <summary>
        /// Constructor.
        /// </summary>
        public ModelView()
        {
            Disposables = new List<IDisposable>();
            _properties = new SortedDictionary<string, object>();
            _propertyTypes = new SortedDictionary<string, Type>();
        }

        /// <summary>
        /// Method creates new member.
        /// </summary>
        protected BehaviorSubject<T> CreateProperty<T>(string propertyName, T value)
        {
            BehaviorSubject<T> instance = new BehaviorSubject<T>(value);
            
            string propertyNameLower = propertyName.ToLower();
            _properties.Add(propertyNameLower, instance);
            _propertyTypes.Add(propertyNameLower, typeof(T));

            Disposables.Add(instance);

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

            Type propertyType = _propertyTypes[propertyNameLower];

            if (propertyType.IsAssignableFrom(value.GetType()))
            {
                object test = _properties[propertyNameLower];
                propertyType.GetMethod("OnNext").Invoke(test, new[] { value });
            }

            return true;
        }

        /// <summary>
        /// Subscribe property value.
        /// </summary>
        public IDisposable Subscribe(string propertyName, Action<object> function)
        {
            string propertyNameLower = propertyName.ToLower();

            if (!HasProperty(propertyNameLower))
                return default;            
            
            return (IDisposable)_propertyTypes[propertyNameLower]
                .GetMethod("Subscribe")
                .Invoke(_properties[propertyNameLower], new Action<object>[] { item =>
            {
                function(item);
            } });
        }

        /// <summary>
        /// Get property type.
        /// </summary>
        public Type GetPropertyType(string propertyName)
        {
            string propertyNameLower = propertyName.ToLower();

            if (!HasProperty(propertyNameLower))
                return null;

            return _propertyTypes[propertyNameLower];
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (IDisposable item in Disposables)
                item.Dispose();

            Disposables.Clear();            
        }
    }
}
