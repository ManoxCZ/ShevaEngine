using Microsoft.Xna.Framework.Graphics;
using Noesis;
using System;
using System.Globalization;
using System.Reflection;

namespace ShevaEngine.NoesisUI
{
    public class Texture2DToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Texture2D texture)
            {
                if (typeof(Texture2D).GetField("_texture", BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo info &&
                    info.GetValue(texture) as SharpDX.Direct3D11.Resource is SharpDX.Direct3D11.Resource handle)
                {
                    return new TextureSource(
                        RenderDeviceD3D11.WrapTexture(
                            texture, 
                            handle.NativePointer, 
                            texture.Width, 
                            texture.Height, 
                            texture.LevelCount, 
                            false,
                            true));
                }
            }

            return null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
