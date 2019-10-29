using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LogSearcher.Domain
{
    public static class Validate
    {
        public static bool ValidateDirectory(string path)
        {
            if (path == null) return false;
            if (path == "") return false;

            DirectoryInfo DirInfo = new DirectoryInfo(path);
            if (DirInfo.Exists == false) return false;
            return true;
        }

        public static DirectoryInfo GetDirInfo(this string path)
        {            
            DirectoryInfo DirInfo = new DirectoryInfo(path);
            if (DirInfo.Exists == false) return null; 
            return DirInfo;
        }

    }

}
