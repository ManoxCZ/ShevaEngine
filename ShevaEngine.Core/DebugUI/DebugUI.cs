#if DEBUG_UI
using ImGuiNET;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace ShevaEngine.Core
{
    /// <summary>
    /// Debug UI class.
    /// </summary>
    public class DebugUI
    {
        private readonly ImGuiRenderer _renderer;
        private object _pagesLock = new object();
        private List<IDebugUIPage> _pages;        


        /// <summary>
        /// Constructor.
        /// </summary>        
        public DebugUI(Microsoft.Xna.Framework.Game game)
        {
            _renderer = new ImGuiRenderer(game);
            _renderer.RebuildFontAtlas();            

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
            // Call BeforeLayout first to set things up
            _renderer.BeforeLayout(gameTime);

            // Draw our UI
            ImGuiLayout();

            // Call AfterLayout now to finish up and draw all the things
            _renderer.AfterLayout();
        }

        /// <summary>
        /// ImGui layout.
        /// </summary>
        protected virtual void ImGuiLayout()
        {
            ImGui.Begin("Debug Inspector");

            ImGui.BeginTabBar("Pages");

            lock (_pagesLock)
            {
                for (int i = 0; i < _pages.Count; i++)
                {
                    if (ImGui.BeginTabItem(_pages[i].DebugUIPageName))
                    {
                        _pages[i].DebugUI();
                    
                        ImGui.EndTabItem();
                    }
                }
            }

            ImGui.EndTabBar();

            ImGui.End();
        }
    }
}
#endif
