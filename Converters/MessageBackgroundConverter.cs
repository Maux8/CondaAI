using Conda_AI.Model.Enums;
using System.Globalization;

namespace Conda_AI.Converters
{
    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageAuthor author)
            {
                return author == MessageAuthor.User ? Colors.LightBlue : Colors.LightGray;
            }
            return Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
