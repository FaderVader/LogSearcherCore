using LogSearcher.Domain;
using System.IO;

namespace LogSearcher.Models
{
    public abstract class LogDirectory
    {
        private string directory;
        private DirectoryInfo dirInfo;

        public LogDirectory(string path)
        {
            this.directory = path;
            this.dirInfo = path.GetDirInfo();


        }

        public string DirectoryName
        {
            get { return DirInfo.FullName; }
        }



        public DirectoryInfo DirInfo
        {
            get { return dirInfo; }
            private set { dirInfo = value; }
        }



        public string ParentDirectory
        {
            get { return DirInfo.Parent.ToString(); }            
        }






    }
}
