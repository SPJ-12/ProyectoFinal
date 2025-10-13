using System.Globalization;
using Microsoft.Maui.Controls;

namespace ProyectoFinal.Converters;

public class CountToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            int minCount = 2; // Default minimum
            if (parameter != null && int.TryParse(parameter.ToString(), out int paramMin))
            {
                minCount = paramMin;
            }
            return count > minCount;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

