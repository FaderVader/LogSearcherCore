using LogSearcher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static LogSearcher.Domain.Validate;

namespace LogSearcher.Domain
{
    public class FindInFile // Look for specified string in specified file, flag if found
    {
        private SearchProfile searchProfile;
        private List<HitFile> hitList;
        
        public List<HitFile> HitList
        {
            get { return hitList; }
            private set { hitList = value; }
        }
        
        public FindInFile(SearchProfile profile)
        {
            this.searchProfile = profile;
            HitList = new List<HitFile>();
        }

        public async Task SearchInList(List<string> list, CancellationToken cancel)
        {
            foreach (var file in list)
            {
                try
                {
                    // check if cancellation is requested 
                    if (cancel.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    using (var reader = File.OpenText(file))
                    {
                        var fileContent = await reader.ReadToEndAsync();

                        if (fileContent.Contains(searchProfile.SearchString))
                        {
                            // locate the line and col of first occurence                      
                            var hit = new HitFile(file);
                            hit.SearchPosition = await FindLine(fileContent);
                            HitList.Add(hit);
                        }
                    }
                }
                catch (TaskCanceledException e)
                {
                    MessageBox.Show("User Cancelled Search!");
                    return;
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }

        private async Task<TextPosition> FindLine(string file)
        {
            // in log-files, each line ends with [cr-lf]
            // split into array @ cr-lf
            // traverse this array until match is found
            // return index (offset+1 because arrayindex->linenumber)

            TextPosition position = new TextPosition();
            var delim = Environment.NewLine;
            var lines = file.Split(delim); 

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(searchProfile.SearchString))
                {
                    position.Text = lines[i];
                    position.Line = i +1; // +1 -> convert from array-pos to line num
                    break;
                }
            }

            //var result = lines.ToList().Where(lines => lines.Contains(searchProfile.SearchString)); //TODO: check if LINQ can be used instead of for-loop?

            position.Column = position.Text.IndexOf(searchProfile.SearchString) +1; // +1 -> convert from array-pos to text column num

            return position;
        }
    }
}
