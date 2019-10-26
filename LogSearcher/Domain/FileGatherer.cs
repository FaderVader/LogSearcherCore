using LogSearcher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LogSearcher.Domain
{
    public class FileGatherer  // Traverse files in specified location, get any of matching type
    {
        
        public FileGatherer(IList<SourceDirectory> sourceDirectories, SearchProfile search)
        {
            this.sourceDirectories = sourceDirectories;
            this.searchProfile = search;
        }


        private IList<SourceDirectory> sourceDirectories;
        public IList<SourceDirectory> SourceDirectories
        {
            get { return sourceDirectories; }
            private set { sourceDirectories = value; }
        }

        private SearchProfile searchProfile;
        public SearchProfile SearchProfile
        {
            get { return searchProfile; }
            set { searchProfile = value; }
        }

        
        public async Task TraverseSourceDirs()
        {
            foreach (var directory in SourceDirectories)
            {
                if (!Directory.Exists(directory.DirectoryName)) { continue; }

                var pattern = $"{SearchProfile.FileExt}";                
                var list = await GetFiles(directory.DirectoryName, pattern);

                directory.FoundFileList.Clear(); // ensure list is cleared before populating

                FindInFile findFile = new FindInFile(searchProfile);
                await findFile.SearchInList(list);          
                directory.FoundFileList = findFile.HitList;
            }
        }

        public List<HitFile> GetFoundFiles()
        {
            List<HitFile> foundFiles = new List<HitFile>();

            foreach (var dir in SourceDirectories)
            {
                dir.FoundFileList.ForEach(foundFile => foundFiles.Add(foundFile));
            }

            return foundFiles;
        }

        private async Task<List<string>> GetFiles(string directory, string searchPattern)
        {
            List<string> list = new List<string>();
            try
            {
                list = Directory.GetFiles(directory, searchPattern).ToList();
            }
            catch (Exception)
            {
             // just ignore error
            }
            return list;
        }

    }
}
