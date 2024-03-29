﻿using System;

namespace LogSearcher.Models
{
    public class HitFile : IFoundFile 
    {
        public HitFile(string fullPath)
        {           
            if (fullPath.Length < 1) throw new Exception("path is too short");

            if (!fullPath.Contains("\\")) throw new Exception("path seems illegit");

            // if last char is \ remove
            if (fullPath.EndsWith("\\")) { fullPath = fullPath.Substring(0, fullPath.Length - 1); };

            int lastDelim = fullPath.LastIndexOf('\\');

            fileName = fullPath.Substring(lastDelim + 1);
            filePath = fullPath.Substring(0, lastDelim);

        }
        
        private string fileName;
        public string FileName
        {
            get { return fileName; }
        }
        
        private string filePath;
        public string FilePath
        {
            get { return filePath; }

        }

        public string FilePathAndName
        {
            get { return filePath + "\\" + fileName; }
        }

        private bool fileIsCopied;
        public bool FileIsCopied
        {
            get { return fileIsCopied; }
            set { fileIsCopied = value; Marked = true; }  
        }       

        private bool marked;
        public bool Marked
        {
            get { return marked; }
            set { marked = value; }
        }
        
        private TextPosition searchPosition;
        public TextPosition SearchPosition
        {
            get { return searchPosition; }
            set { searchPosition = value; }
        }
    }
}
