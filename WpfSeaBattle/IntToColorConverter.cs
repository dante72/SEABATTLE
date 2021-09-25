using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SeaBattleLib;

namespace WpfSeaBattle
{
    public class IntToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Textures)value)
            {

                case Textures.Ship: return Brushes.White;
                case Textures.Miss: return Brushes.LightCoral;
                case Textures.Destroyed: return Brushes.DarkGray;
                default: return Brushes.LightBlue;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
