using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace ModuleLauncher.NET.Example.Utils;

public class BoolToGrayColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FileInfo file)
        {
            return file.Exists ? "Gray" : "Black";
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}