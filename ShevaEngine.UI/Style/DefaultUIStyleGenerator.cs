using Microsoft.Xna.Framework;
using ShevaEngine.UI.Brushes;
using System;

namespace ShevaEngine.UI
{
    public class DefaultUIStyleGenerator : IUIStyleGenerator
    {
        /// <summary>
        /// Create method.
        /// </summary>
        public T Create<T>() where T : Control
        {
            T instance = Activator.CreateInstance<T>();

            if (instance is Button button)
                StyleButton(button);

            return instance;
        }   
        
        /// <summary>
        /// Style button.
        /// </summary>        
        private void StyleButton(Button button)
        {
            button.Background.OnNext(new SolidColorBrush(Color.Gray));
        }
    }
}
