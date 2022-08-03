using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace ModuleLauncher.NET.Example.Utils;

public class ComboBoxItemToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ComboBoxItem comboBoxItem)
        {
            return comboBoxItem.Content;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ComboBoxItem comboBoxItem)
        {
            return comboBoxItem.Content;
        }
        
        return null;
    }
}