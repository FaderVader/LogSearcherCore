using LogSearcher.Models;
using LogSearcher.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogSearcher.Domain
{
    public static class FileHandler  // Copy the specified files to the specified location
    {
        public static async Task CopyHits(ObservableCollection<HitFile> hitList, string destination)
        {
            // copy all files in list-parameter
            // flag files as copied when done (change color in Found List?)
            // allow to copy single file, selection or all (overwrite @ destination ?)

            if (hitList == null || hitList.Count < 1) return;
            if (destination == null || destination.Length < 1) return;
            if (!Directory.Exists(destination)) return;

            try
            {
                foreach (var hit in hitList)
                {
                    if (!hit.FileIsCopied) // do not copy already processed files
                    {
                        using (FileStream sourceStream = File.Open(hit.FilePathAndName, FileMode.Open))
                        {
                            var newFile = Path.Combine(destination, hit.FileName);
                            using (FileStream destinationStream = File.Create(newFile))
                            {
                                await sourceStream.CopyToAsync(destinationStream);
                                hit.FileIsCopied = true;
                            }
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to Copy Files:{Environment.NewLine}{ex.Message}", "Error on Copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public static string BrowseForFolder(string initialDir = "")
        {        
            string selectedFolder = initialDir;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;

            if (!string.IsNullOrEmpty(initialDir) && initialDir.ValidateDirectory())
            {
                fileDialog.InitialDirectory = initialDir;
            }

            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var fileName = fileDialog.FileName;
                selectedFolder = new FileInfo(fileName)?.Directory.ToString();
            }
            return selectedFolder;
        }

        public static void OpenWithNPP(HitFile hitFile)
        {
            if (hitFile == null) return;
            if (!File.Exists(hitFile?.FilePathAndName)) return;

            try
            {
                var settings = AppSettings.GetSettings();
                var notePadPath = settings.NotePadPP_Path; 
                var notePadExe = settings.NotePadPP_Exe; 

                var startInfo = new ProcessStartInfo(hitFile.FilePathAndName) { UseShellExecute = false };
                startInfo.FileName = $"{notePadPath}\\{notePadExe}";
                startInfo.Arguments = $"-n{hitFile?.SearchPosition.Line} -c{hitFile?.SearchPosition.Column} {hitFile?.FilePathAndName}";
                using (var p = new Process())
                {
                    p.StartInfo = startInfo;
                    p.Start();
                }
            }
            catch (Exception)
            {               
                OpenWithDefault(hitFile);  // fallback to default application for file.
            }

        }

        public static void OpenWithDefault(HitFile hitFile)
        {
            if (!File.Exists(hitFile?.FilePathAndName)) return;

            var startInfo = new ProcessStartInfo(hitFile.FilePathAndName) { UseShellExecute = true };
            using (var p = new Process())
            {                
                p.StartInfo = startInfo;
                p.Start();
            }
        }

        public static async Task OpenExplorer(LogDirectory directory)
        {
            if (!directory.DirInfo.Exists) return;

            var startInfo = new ProcessStartInfo(directory.DirInfo.FullName) { UseShellExecute = true };
            using (var p = new Process())
            {
                p.StartInfo = startInfo;
                p.Start();
            }
        }
    }
}
