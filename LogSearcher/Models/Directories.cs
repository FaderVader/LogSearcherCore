using System.Collections.Generic;

namespace LogSearcher.Models
{
    public class SourceDirectory: LogDirectory
    {
        private List<HitFile> foundFileList;

        public List<HitFile> FoundFileList
        {
            get { return foundFileList; }
            set { foundFileList = value; }
        }

        public SourceDirectory(string path) : base(path)
        {
            foundFileList = new List<HitFile>();
        }

    }

    public class TargetDirectory : LogDirectory
    {
        private string targetDir;

        public string TargetDir
        {
            get { return targetDir; }
            set { targetDir = value; }
        }

        public TargetDirectory(string path) : base(path)
        {

        }

    }
}
