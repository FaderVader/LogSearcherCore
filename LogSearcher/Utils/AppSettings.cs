using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows;

namespace LogSearcher.Utils
{
    public static class AppSettings
    {
        private static IConfiguration configRoot { get; set; }

        static AppSettings()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                configRoot = builder.Build();
                var appSettings = configRoot.GetSection("AppSettings");
            }
            catch (Exception)
            {
                MessageBox.Show($"Failed to find settings.{Environment.NewLine}The application will exit.", "LogSearcher Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // workaround for false-flagging settings as missing when editing MainWindow.xaml
                var application = Application.Current;                
                if (application.MainWindow != null)
                {
                    Environment.Exit(1);
                }

            }
        }

        public static Settings GetSettings()
        {
            var settings = configRoot.GetSection("AppSettings").Get<Settings>();
            return settings;
        }
    }

    public class Settings
    {
        public string NotePadPP_Path { get; set; }
        public string NotePadPP_Exe { get; set; }
        public bool UseNPP { get; set; }
        public string HistoryPath { get; set; }
    }
}
