using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EpicBattle.ViewModels
{
    public class ClassToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.ToString() == parameter?.ToString())
            {
                return new SolidColorBrush(Color.FromRgb(255, 170, 0)); // Активный (#FFAA00)
            }
            return new SolidColorBrush(Color.FromRgb(51, 51, 51)); // Неактивный (#333333)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
