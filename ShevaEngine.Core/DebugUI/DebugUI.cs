using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace ShevaEngine.Core
{
    /// <summary>
    /// Debug UI class.
    /// </summary>
    public class DebugUI
    {
        private object _pagesLock = new object();
        private List<IDebugUIPage> _pages;        


        /// <summary>
        /// Constructor.
        /// </summary>        
        public DebugUI(ShevaGame game)
        {
            _pages = new List<IDebugUIPage>();
        }

        /// <summary>
        /// Add debug page.
        /// </summary>        
        public void AddDebugPage(IDebugUIPage page)
        {
            lock (_pagesLock)
            {
                if (!_pages.Contains(page))
                    _pages.Add(page);
            }
        }

        /// <summary>
        /// Remove debug page.
        /// </summary>        
        public void RemoveDebugPage(IDebugUIPage page)
        {
            lock (_pagesLock)
            {
                if (_pages.Contains(page))
                    _pages.Remove(page);
            }
        }
        
        /// <summary>
        /// Draw.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
         
        }
    }
}
