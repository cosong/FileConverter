// <copyright file="ApplicationThemeToString.cs" company="AAllard">License: http://www.gnu.org/licenses/gpl.html GPL version 3.</copyright>

namespace FileConverter.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ApplicationThemeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ApplicationTheme theme)
            {
                string lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
                switch (theme)
                {
                    case ApplicationTheme.Light:
                        if (lang == "uk") return "Світла";
                        if (lang == "ru") return "Светлая";
                        return "Light";
                    case ApplicationTheme.Dark:
                        if (lang == "uk") return "Темна";
                        if (lang == "ru") return "Тёмная";
                        return "Dark";
                }
            }

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
