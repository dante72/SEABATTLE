using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SeaBattleLib;

namespace WpfSeaBattle {
    public class IntToColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is null)
                return null;
            if (!(value is Textures))
                throw new ArgumentException($"Исходное значение должно иметь тип {nameof(value)}");

            if (targetType != typeof(Brush))
                throw new InvalidCastException();

            switch ((Textures)value) {
                case Textures.Deck: return Brushes.White;
                case Textures.Water: return Brushes.LightBlue;
                case Textures.Destroyed: return Brushes.DarkGray;
                case Textures.Miss: return Brushes.LightCoral;
                default: throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
