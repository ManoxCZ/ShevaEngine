using ShevaEngine.Core.UI;
using System.Threading.Tasks;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Loading screen component.
    /// </summary>
    public class LoadingScreenComponent : ShevaGameComponent
    {
        public string XamlFilename { get; set; }

        /// <summary>
        /// Initialize.
        /// </summary>
        public override void LoadContent(ShevaGame game)
        {
            base.LoadContent(game);

            Task<ILayer> task = game.UISystem.GetLayer(XamlFilename);
            task.Wait();

            Layers.Add(task.Result);
        }
    }
}