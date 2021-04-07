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
        public bool SetPropertyValue<T>(string propertyName, T value)
        {
            string propertyNameLower = propertyName.ToLower();

            if (!HasProperty(propertyNameLower))
                return false;

            Type propertyType = _properties[propertyNameLower].GetType().GenericTypeArguments[0];

            if (propertyType.IsAssignableFrom(value.GetType()))
            {
                object test = _properties[propertyNameLower];
                test.GetType().GetMethod("OnNext").Invoke(test, new[] { (object)value });
            }

            return true;
        }

        /// <summary>
        /// Set property value.
        /// </summary>
        public BehaviorSubject<T> GetPropertyValue<T>(string propertyName)
        {
            string propertyNameLower = propertyName.ToLower();

            if (!HasProperty(propertyNameLower))
                return default;

            return (BehaviorSubject<T>)_properties[propertyNameLower];
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
