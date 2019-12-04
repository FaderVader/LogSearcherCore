using System;
using System.Collections.Generic;

namespace LogSearcher.Models
{
    [Serializable]
    public class SourceDirectory: LogDirectory
    {
        public SourceDirectory(string path) : base(path)
        {
            foundFileList = new List<HitFile>();
        }

        private List<HitFile> foundFileList;
        public List<HitFile> FoundFileList
        {
            get { return foundFileList; }
            set { foundFileList = value; }
        }
    }

    public class TargetDirectory : LogDirectory
    {
        public TargetDirectory(string path) : base(path)
        {

        }
        
        private string targetDir;
        public string TargetDir
        {
            get { return targetDir; }
            set { targetDir = value; }
        }

    }
}
