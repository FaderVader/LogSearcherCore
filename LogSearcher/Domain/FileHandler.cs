using LogSearcher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace LogSearcher.Domain
{
    public static class FileHandler  // Copy the specified files to the specified location
    {
        public static async void CopyHits(List<HitFile> hitList)
        {
            // copy all files in list-parameter
            // flag files as copied when done (change color in Found List?)
            // allow to copy single file, selection or all (overwrite @ destination ?)
        }
        public static string BrowseForFolder()
        {        
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            string selectedFolder = null;

            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var fileName = fileDialog.FileName;
                selectedFolder = new FileInfo(fileName)?.Directory.ToString();
            }
            return selectedFolder;
        }

        public static void SendToNotePadPP(HitFile hitFile)
        {
            if (hitFile == null) return;
            if (!File.Exists(hitFile?.FilePathAndName)) return;

            try
            {
                var notePadPath = AppSettings.NotePadPP_Path; 
                var notePadExe = AppSettings.NotePadPP_Exe; 

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
                OpenWithFile(hitFile);  // fallback to default application for file.
            }

        }

        public static void OpenWithFile(HitFile hitFile)
        {
            if (!File.Exists(hitFile?.FilePathAndName)) return;

            var startInfo = new ProcessStartInfo(hitFile.FilePathAndName) { UseShellExecute = true };
            using (var p = new Process())
            {                
                p.StartInfo = startInfo;
                p.Start();
            }
        }
    }
}
