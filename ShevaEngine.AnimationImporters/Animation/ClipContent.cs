using Microsoft.Xna.Framework;

namespace AnimationImporters
{
	/// <summary>
	/// Clip content.
	/// </summary>
	public class ClipContent
    {
        public double Duration { get; set; }
        public Matrix[][] Keyframes { get; set; }	
    }
}
