﻿using LogSearcher.Domain;
using System.IO;

namespace LogSearcher.Models
{
    public abstract class LogDirectory
    {
        public LogDirectory(string path)
        {
            if (!path.ValidateDirectory())
            {
                throw new FileNotFoundException("The directory is not valid");
            }
            this.directory = path;
            this.dirInfo = path.GetDirInfo();
        }

        private string directory;
        private DirectoryInfo dirInfo;

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
