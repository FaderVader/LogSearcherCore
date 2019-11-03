using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace LogSearcher.Utils
{
    public static class AppSettings
    {
        private static IConfiguration configRoot { get; set; }

        static AppSettings()
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configRoot = builder.Build();
            var appSettings = configRoot.GetSection("AppSettings");
            
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
    }
}
