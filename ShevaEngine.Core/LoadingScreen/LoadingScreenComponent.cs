using ShevaEngine.Core.UI;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Loading screen component.
    /// </summary>
    public class LoadingScreenComponent<T> : ShevaGameComponent where T : ILayer, new()
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void LoadContent(ShevaGame game)
        {
            base.LoadContent(game);

            Layers.Add(new T());
            //game.UISystem.GetLayer(XamlFilename).ContinueWith(task => Layers.Add(task.Result));
        }
    }
}