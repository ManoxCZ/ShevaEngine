using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Image brush.
    /// </summary>
    public class ImageBrush : Brush
    {
        private string _imageFilename;
        private Stretch _stretch;
        private Color _colorTint;
        private Texture2D _texture;


        /// <summary>
        /// Constructor.
        /// </summary>        
        public ImageBrush(string imageFilename)
            : this(imageFilename, Stretch.Uniform)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>        
        public ImageBrush(string imageFilename, Stretch stretch)
            : this (imageFilename, stretch, Color.White)
        {
           
        }

        /// <summary>
        /// Constructor.
        /// </summary>        
        public ImageBrush(string imageFilename, Stretch stretch, Color colorTint)
        {
            _imageFilename = imageFilename;
            _stretch = stretch;
            _colorTint = colorTint;
        }


        /// <summary>
        /// Load content.
        /// </summary>        
        public override void LoadContent(ContentManager contentManager)
        {
            if (_texture == null && !string.IsNullOrEmpty(_imageFilename))
                _texture = contentManager.Load<Texture2D>(_imageFilename);
        }

        /// <summary>
        /// Draw method.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, Rectangle locationSize)
        {
            switch (_stretch)
            {
                case Stretch.None:
                    {
                        locationSize = new Rectangle(
                            locationSize.Center.X - _texture.Width / 2,
                            locationSize.Center.Y - _texture.Height / 2,
                            _texture.Width,
                            _texture.Height);
                    }
                    break;
                case Stretch.Fill:
                    {
                        // locationSize is the same
                    }
                    break;
                case Stretch.Uniform:
                    {
                        double imageAspectRatio = _texture.Width / (double)_texture.Height;
                        double destinationRatio = locationSize.Width / (double)locationSize.Height;

                        if (imageAspectRatio > destinationRatio)
                        {
                            double scale = locationSize.Width / (double)_texture.Width;

                            locationSize = new Rectangle(
                                locationSize.X,
                                locationSize.Center.Y - (int)((_texture.Height * scale) / 2),
                                locationSize.Width,
                                (int)(_texture.Height * scale));
                        }
                        else
                        {
                            double scale = locationSize.Height / (double)_texture.Height;

                            locationSize = new Rectangle(
                                locationSize.Center.X - (int)((_texture.Width * scale) / 2),
                                locationSize.Y,
                                (int)(_texture.Width * scale),
                                locationSize.Height);
                        }
                    }
                    break;
                case Stretch.UniformToFill:
                    {
                        double imageAspectRatio = _texture.Width / (double)_texture.Height;
                        double destinationRatio = locationSize.Width / (double)locationSize.Height;

                        if (imageAspectRatio < destinationRatio)
                        {
                            double scale = locationSize.Width / (double)_texture.Width;

                            locationSize = new Rectangle(
                                locationSize.X,
                                locationSize.Center.Y - (int)((_texture.Height * scale) / 2),
                                locationSize.Width,
                                (int)(_texture.Height * scale));
                        }
                        else
                        {
                            double scale = locationSize.Height / (double)_texture.Height;

                            locationSize = new Rectangle(
                                locationSize.Center.X - (int)((_texture.Width * scale) / 2),
                                locationSize.Y,
                                (int)(_texture.Width * scale),
                                locationSize.Height);
                        }
                    }
                    break;
            }

            spriteBatch.Draw(_texture, locationSize, null, _colorTint);
        }
    }
}
