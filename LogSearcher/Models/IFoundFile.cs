namespace LogSearcher.Models
{
    interface IFoundFile
    {
        string FileName { get; }

        string FilePath { get; }

        string FilePathAndName { get; }
        
    }
}
