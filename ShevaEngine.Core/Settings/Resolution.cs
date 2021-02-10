
namespace ShevaEngine.Core
{
    /// <summary>
    /// Resolution class.
    /// </summary>
    public struct Resolution
    {
        public int Width {get;set;}
        public int Height {get;set;}
       

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resolution(int size)
        {
            Width = size;
            Height = size;
        }
    }
}