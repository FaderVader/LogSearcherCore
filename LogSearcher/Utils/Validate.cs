using System.IO;

namespace LogSearcher.Domain
{
    public static class Validate
    {
        public static bool ValidateDirectory(this string path)
        {
            if (string.IsNullOrEmpty(path)) return false;            

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
