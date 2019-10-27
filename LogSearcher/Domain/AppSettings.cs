using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace LogSearcher.Domain
{
    public static class AppSettings
    {
        private static IConfiguration configuration { get; set; }

        public static string NotePadPP_Path { get; private set; }
        public static string NotePadPP_Exe { get; private set; }
        public static bool UseNPP { get; private set; }

        static AppSettings()
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        public static void Acquire()
        {
            NotePadPP_Path = configuration.GetSection("AppSettings").GetValue<string>("NotePadPP_Path");
            NotePadPP_Exe = configuration.GetSection("AppSettings").GetValue<string>("NotePadPP_Exe");

            Boolean.TryParse(configuration.GetSection("AppSettings").GetValue<string>("UseNPP"), out bool useit);
            UseNPP = useit;

        }
    }
}
