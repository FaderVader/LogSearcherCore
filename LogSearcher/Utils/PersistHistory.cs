using LogSearcher.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LogSearcher.Utils
{
    public class PersistHistory
    {
        //string path = @"C:\temp\settings\history.json";
        public string FilePath { get; set; }
        private string fileName = "history.json";

        public PersistHistory(string path = @"C:\temp\settings\")
        {
            FilePath = path;
        }

        public async Task SaveHistory(IEnumerable<LogDirectory> history)
        {
            // must discard LogDirectory, because DirectoryInfo cannot be serialized
            var cleanedHistory = history.Select(item => new string(item.DirectoryName)).ToList();
            var json = JsonSerializer.Serialize(cleanedHistory);           

            using (StreamWriter destinationStream = File.CreateText(Path.Combine(FilePath, fileName)))
            {
                await destinationStream.WriteAsync(json);
            }
        }

        public async Task<IEnumerable<LogDirectory>> GetHistory()
        {
            //string json = File.ReadAllText(path);
            string json;
            using (StreamReader sourceStream = File.OpenText(Path.Combine(FilePath, fileName)))
            {
                json = await sourceStream.ReadToEndAsync();
            }

            var result = JsonSerializer.Deserialize<List<String>>(json);

            var history = result.Select(line => new SourceDirectory(line));            
            return history;
        }

    }
}
