using LogSearcher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace LogSearcher.Utils
{
    public class PersistHistory
    {
        public string FilePath { get; set; }
        private string fileName = "history.json";

        public PersistHistory(string path = @"C:\temp\Settings\")
        {
            FilePath = path;
        }

        public async Task SaveHistory(IEnumerable<LogDirectory> history)
        {
            // must discard LogDirectory, because DirectoryInfo cannot be serialized
            var cleanedHistory = history.Select(item => new string(item.DirectoryName)).ToList();
            var json = JsonSerializer.Serialize(cleanedHistory);

            try
            {
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                using (StreamWriter destinationStream = File.CreateText(Path.Combine(FilePath, fileName)))
                {
                    await destinationStream.WriteAsync(json);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to save history:{Environment.NewLine}{e.Message}", "LogSearcher - Save History Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task<IEnumerable<LogDirectory>> GetHistory()
        {
            string json;
            IEnumerable<SourceDirectory> history;

            try
            {
                using (StreamReader sourceStream = File.OpenText(Path.Combine(FilePath, fileName)))
                {
                    json = await sourceStream.ReadToEndAsync();
                }

                var result = JsonSerializer.Deserialize<List<String>>(json);

                history = result.Select(line => new SourceDirectory(line));
            }
            catch (Exception)
            {
                throw new Exception("Fatal error while reading history-file.");
            }        
            return history;
        }

    }
}
