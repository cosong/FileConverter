// <copyright file="ThemeManager.cs" company="AAllard">License: http://www.gnu.org/licenses/gpl.html GPL version 3.</copyright>

namespace FileConverter.Services
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    public static class ThemeManager
    {
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private static bool initialized = false;

        public static ApplicationTheme CurrentTheme { get; private set; } = ApplicationTheme.Light;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            EventManager.RegisterClassHandler(
                typeof(Window),
                FrameworkElement.LoadedEvent,
                new RoutedEventHandler((sender, args) =>
                {
                    if (sender is Window window)
                    {
                        ApplyWindowTheme(window, CurrentTheme);
                    }
                }));
        }

        public static void ApplyTheme(ApplicationTheme theme)
        {
            Initialize();
            CurrentTheme = theme;

            string source = theme == ApplicationTheme.Dark
                ? "Views/Resources/Colors.Dark.xaml"
                : "Views/Resources/Colors.xaml";

            var app = System.Windows.Application.Current;
            if (app == null)
            {
                return;
            }

            var mergedDicts = app.Resources.MergedDictionaries;
            var newDict = new ResourceDictionary
            {
                Source = new Uri(source, UriKind.RelativeOrAbsolute)
            };

            if (mergedDicts.Count > 0)
            {
                mergedDicts[0] = newDict;
            }
            else
            {
                mergedDicts.Add(newDict);
            }

            foreach (Window window in app.Windows)
            {
                ApplyWindowTheme(window, theme);
            }
        }

        public static void ApplyWindowTheme(Window window, ApplicationTheme theme)
        {
            if (window == null)
            {
                return;
            }

            try
            {
                window.SetResourceReference(Window.BackgroundProperty, "Window_BackgroundBrush");
                window.SetResourceReference(Window.ForegroundProperty, "DarkForegroundBrush");

                var helper = new WindowInteropHelper(window);
                IntPtr hwnd = helper.Handle;
                if (hwnd != IntPtr.Zero)
                {
                    int useDarkMode = (theme == ApplicationTheme.Dark) ? 1 : 0;
                    if (DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int)) != 0)
                    {
                        DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref useDarkMode, sizeof(int));
                    }
                }
                else
                {
                    EventHandler onSourceInitialized = null;
                    onSourceInitialized = (s, e) =>
                    {
                        window.SourceInitialized -= onSourceInitialized;
                        ApplyWindowTheme(window, CurrentTheme);
                    };
                    window.SourceInitialized += onSourceInitialized;
                }
            }
            catch
            {
                // Ignore DWM errors on unsupported platforms
            }
        }
    }
}
