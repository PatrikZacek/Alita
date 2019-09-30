using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Alita.Services
{
    public class BooleanToWindowStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? state = (bool?)value;
            switch (state)
            {
                case null: return WindowState.Minimized;
                case true: return WindowState.Maximized;
                case false: return WindowState.Normal;
                default: throw new Exception("Invalid input type " + value.GetType());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is WindowState)
            {
                var state = (WindowState)value;
                switch (state)
                {
                    case WindowState.Normal: return false;
                    case WindowState.Maximized: return true;
                    case WindowState.Minimized: return null;
                    default: throw new Exception("Invalid input type " + value.GetType());
                }
            }
            throw new Exception("Invalid input type " + value.GetType());
        }
    }
}
