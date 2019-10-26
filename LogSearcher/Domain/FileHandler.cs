using LogSearcher.Models;
using System;
using System.Diagnostics;
using System.IO;
//using System.Windows.Forms;

namespace LogSearcher.Domain
{
    public static class FileHandler  // Copy the specified files to the specified location
    {
        public static void OpenWithFile(HitFile file)
        {
            if (!File.Exists(file?.FilePathAndName)) return;

            Process.Start(file.FilePathAndName);
        }

        public static string BrowseForFolder()
        {
            // TODO: find dialog-alternative in Core3.0
            //OpenFileDialog fileDialog = new OpenFileDialog();
            //fileDialog.Multiselect = false;
            //string selectedFolder = null;

            //DialogResult result = fileDialog.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    var fileName = fileDialog.FileName;
            //    selectedFolder = new FileInfo(fileName)?.Directory.ToString();
            //}
            //return selectedFolder;
            return "";
        }

        public static void SendToNotePadPP(HitFile hitfile)
        {
            if (hitfile == null) return;

            try
            {
                var notePadPath = @"C:\Program Files (x86)\Notepad++"; // TODO: where to store settings ? //Properties.Settings.Default.NotePadPP_Path;
                var notePadExe = @"notepad++"; //Properties.Settings.Default.NotePadPP_Exe;
                string npp = $"{notePadPath}\\{notePadExe}";
                string args = $"-n{hitfile?.SearchPosition.Line} -c{hitfile?.SearchPosition.Column} {hitfile?.FilePathAndName}";

                Process.Start(npp, args);
            }
            catch (Exception)
            {
                // fallback to default application for file.
                OpenWithFile(hitfile);
            }

        }
    }
}
