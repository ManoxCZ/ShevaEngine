using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core
{
	public static class Extensions
    {
        public static Animations GetAnimations(this Model model)
        {
            var animations = model.Tag as Animations;
            return animations;
        }        
    }
}
