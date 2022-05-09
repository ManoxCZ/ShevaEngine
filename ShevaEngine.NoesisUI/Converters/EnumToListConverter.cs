using Noesis;
using System;
using System.Globalization;

namespace ShevaEngine.NoesisUI.Converters;

public class EnumToListConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Enum)
            return Enum.GetValues(value.GetType());

        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null!;
    }
}
