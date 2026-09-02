using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EpicBattle.ViewModels
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isLearned && isLearned)
            {
                return new SolidColorBrush(Colors.Gold);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
