using LogSearcher.Domain;
using LogSearcher.Models;
using LogSearcher.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogSearcher.ViewModels
{
    class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            // initialize main lists
            SourceDirectories = new ObservableCollection<SourceDirectory>();
            HitList = new ObservableCollection<HitFile>();
            DirectoryHistory = new BindingList<LogDirectory>();

            // reset ui
            InputExtension = "*txt";
            InputSearchString = "";
            InputTargetFolder = "";
            InputSourceFolder = ""; //@"C:\temp\LogSearcherTestFiles\VCP_logs";
            SearchStatus = "";

            // get settings
            settings = AppSettings.GetSettings();
            persistHistory = new PersistHistory(settings.HistoryPath);
            SelectUseNPP = settings.UseNPP;

            // wire-up buttons
            CopyAllButton = new RelayCommandNoParams(CopyAllFiles, EnableCopyAll);
            ResetHistoryButton = new RelayCommandNoParams(ResetHistory);
            OpenSourceFolderButton = new RelayCommandNoParams(BrowseForSourceFolder);
            OpenTargetFolderButton = new RelayCommandNoParams(OpenTargetFolder);
            ResetSourceFolderDisplayButton = new RelayCommandNoParams(ResetSourceFolderDisplay);

            GoSearch = new RelayCommandNoParams(StartSearch, () => SourceDirectories.Count > 0);
            OpenFileButton = new RelayCommandNoParams(OpenFile, () => SelectedFile != null);
            CancelSearchButton = new RelayCommandNoParams(CancelSearch, () => SearchIsRunning == true);
            SubmitSourceFolderButton = new RelayCommandNoParams(SubmitSourceFolder, () => InputSourceFolder.ValidateDirectory()); 
            SetInputFromHistoryButton = new RelayCommandWithParams(SetSourceFolder, () => SelectedHistoryFolder != null);
            SetTargetFromHistoryButton = new RelayCommandWithParams(SetTargetFolder, () => SelectedHistoryFolder != null);
            OpenExplorerForTargetButton = new RelayCommandWithParams(OpenExplorer, () => InputTargetFolder.ValidateDirectory());  
            
            OpenExplorerButton = new RelayCommandWithParams(OpenExplorer, () => SelectedFile != null);
            CopySelectedButton = new RelayCommandWithParams(CopySelected, EnableCopySelected);
            RemoveFromHistoryButton = new RelayCommandWithParams(RemoveFromHistory, () => SelectedHistoryFolder != null);
            RemoveFromSourceListButton = new RelayCommandWithParams(RemoveFromSourceList, () => SelectedInputSourceFolder != null);

            InitializeHistory();            
        }


        #region RelayCommands
        public RelayCommandNoParams GoSearch { get; }
        public RelayCommandNoParams OpenSourceFolderButton { get; }
        public RelayCommandNoParams SubmitSourceFolderButton { get; }
        public RelayCommandNoParams ResetSourceFolderDisplayButton { get; }
        public RelayCommandNoParams OpenTargetFolderButton { get; }
        public RelayCommandNoParams CopyAllButton { get; }
        public RelayCommandNoParams OpenFileButton { get; }
        public RelayCommandWithParams CopySelectedButton { get; }
        public RelayCommandWithParams SetInputFromHistoryButton { get; }
        public RelayCommandWithParams SetTargetFromHistoryButton { get; }
        public RelayCommandNoParams ResetHistoryButton { get; }
        public RelayCommandWithParams OpenExplorerButton { get; }
        public RelayCommandWithParams OpenExplorerForTargetButton { get; }
        public RelayCommandWithParams RemoveFromSourceListButton { get;}
        public RelayCommandWithParams RemoveFromHistoryButton { get; }
        public RelayCommandNoParams CancelSearchButton { get; }

        #endregion

        #region Main lists

        private Settings settings;
        private bool searchIsRunning;
        private PersistHistory persistHistory;

        private TargetDirectory targetDirectory;
        private ObservableCollection<HitFile> hitList;
        private BindingList<LogDirectory> directoryHistory;
        private ObservableCollection<SourceDirectory> sourceDirectories;

        CancellationTokenSource cancel;
        
        public bool SearchIsRunning
        {
            get { return searchIsRunning; }
            set { OnPropertyChanged(ref searchIsRunning, value); }
        }

        public ObservableCollection<SourceDirectory> SourceDirectories
        {
            get { return sourceDirectories; }
            set
            {
                OnPropertyChanged(ref sourceDirectories, value);
            }
        }
        public TargetDirectory TargetDirectory
        {
            get { return targetDirectory; }
            set
            {
                OnPropertyChanged(ref targetDirectory, value);
            }
        }
        public ObservableCollection<HitFile> HitList
        {
            get { return hitList; }
            set { OnPropertyChanged(ref hitList, value); }
        }
        public BindingList<LogDirectory> DirectoryHistory
        {
            get { return directoryHistory; }
            set { OnPropertyChanged(ref directoryHistory, value); }
        }

        #endregion

        #region View-property bindings 

        private bool selectUseNPP;
        public bool SelectUseNPP
        {
            get { return selectUseNPP; }
            set { OnPropertyChanged(ref selectUseNPP, value); }
        }

        private string searchStatus;
        public string SearchStatus
        {
            get { return searchStatus; }
            set { OnPropertyChanged(ref searchStatus, value); }
        }

        private string inputSearchString;
        public string InputSearchString
        {
            get { return inputSearchString; }
            set
            {
                OnPropertyChanged(ref inputSearchString, value);
            }
        }

        private string inputExtension;
        public string InputExtension
        {
            get { return inputExtension; }
            set
            {
                OnPropertyChanged(ref inputExtension, value);
            }
        }

        private string inputSourceFolder;
        public string InputSourceFolder
        {
            get { return inputSourceFolder; }
            set { OnPropertyChanged(ref inputSourceFolder, value); }
        }

        private SourceDirectory selectedInputSourceFolder;
        public SourceDirectory SelectedInputSourceFolder
        {
            get { return selectedInputSourceFolder; }
            set { OnPropertyChanged(ref selectedInputSourceFolder, value); InputSourceFolder = value?.DirectoryName; }
        }

        private string inputTargetFolder;
        public string InputTargetFolder
        {
            get { return inputTargetFolder; }
            set { OnPropertyChanged(ref inputTargetFolder, value); }
        }

        private HitFile selectedFile;
        public HitFile SelectedFile
        {
            get { return selectedFile; }
            set { OnPropertyChanged(ref selectedFile, value); FileContent = SelectedFile?.SearchPosition.Text; }
        }

        private string fileContent;
        public string FileContent
        {
            get { return fileContent; }
            set { OnPropertyChanged(ref fileContent, value); }
        }

        private SourceDirectory selectedHistoryFolder;
        public SourceDirectory SelectedHistoryFolder
        {
            get { return selectedHistoryFolder; }
            set { OnPropertyChanged(ref selectedHistoryFolder, value); }
        }


        #endregion

        #region Button-methods
        private void BrowseForSourceFolder()
        {
            var target = InputSourceFolder.ValidateDirectory() == true ? InputSourceFolder : "";
            var folder = FileHandler.BrowseForFolder(target);
            AcceptFolderToSources(folder);
            AcceptFolderToHistory(folder);
        }
        private void SubmitSourceFolder()
        {
            AcceptFolderToSources(InputSourceFolder);
            AcceptFolderToHistory(InputSourceFolder);

            InputSourceFolder = "";  // ensure input-field is cleared, to signal input accepted
        }
        private void SetSourceFolder(object parameter)
        {
            if (parameter == null) return;
            var dir = parameter as LogDirectory;
            AcceptFolderToSources(dir?.DirectoryName);
            AcceptFolderToHistory(dir?.DirectoryName);
        }
        private void SetTargetFolder(object parameter)
        {
            if (parameter == null) return;
            var source = parameter as LogDirectory;
            AcceptFolderToHistory(source?.DirectoryName);

            InputTargetFolder = source?.DirectoryName;
        }
        private void ResetSourceFolderDisplay()
        {
            // reset value of SourceFolderDisplay
            SourceDirectories = new ObservableCollection<SourceDirectory>();
            HitList = new ObservableCollection<HitFile>();
            SearchStatus = "";
        }
        private void OpenTargetFolder()
        {
            var target = InputTargetFolder.ValidateDirectory() == true ? InputTargetFolder : "";
            InputTargetFolder = FileHandler.BrowseForFolder(target);

            AcceptFolderToHistory(InputTargetFolder);
        }
        private async void StartSearch()
        {
            SearchIsRunning = true;

            try
            {
                SearchStatus = "Searching...";
                HitList.Clear();
                await SearchForFiles();

                if (HitList.Count < 1)
                {
                    SearchStatus = "No files found!";
                    return;
                }

                SearchStatus = $"Found Files: {HitList.Count}";
                await persistHistory.SaveHistory(DirectoryHistory);
            }
            catch (TaskCanceledException)
            {
                SearchStatus = "Cancelled Search!";
            }
            finally
            {
                SearchIsRunning = false;
            }
        }
        private async void CopyAllFiles()
        {
            await FileHandler.CopyHits(HitList, InputTargetFolder);
            UpdateHitList();
        }
        private async void CopySelected(object elements)
        {
            if (elements == null) return;
            await CopySelectedFiles(elements);
        }
        private async void OpenExplorer(object path)
        {
            if (path != null && (path as string).ValidateDirectory())
            {
                LogDirectory directory = new SourceDirectory(path as string);
                await FileHandler.OpenExplorer(directory);
            }
        }
        private void OpenFile()
        {
            if (SelectUseNPP)
            {
                FileHandler.OpenWithNPP(SelectedFile);
                return;
            }
            FileHandler.OpenWithDefault(SelectedFile);
        }
        private void ResetHistory()
        {
            DirectoryHistory = new BindingList<LogDirectory>();
        }
        private void RemoveFromSourceList(object parameter)        
        {
            if (parameter == null) return;

            var directory = parameter as SourceDirectory;
            SourceDirectories.Remove(directory);
        }
        private void RemoveFromHistory(object parameter)
        {
            if (parameter == null) return;
            var directory = parameter as LogDirectory;
            DirectoryHistory.Remove(directory);
        }
        private void CancelSearch()
        {
            cancel.Cancel();
        }
        #endregion

        #region Button-toggles

        private bool EnableCopyAll()
        {
            var foundFiles = HitList.Count > 0 ? true : false;
            var destinationOK = inputTargetFolder.ValidateDirectory();

            return foundFiles && destinationOK;
        }

        private bool EnableCopySelected()
        {
            var selected = SelectedFile != null ? true : false;
            var destinationOK = inputTargetFolder.ValidateDirectory();

            return selected && destinationOK;
        }

        #endregion

        #region Methods
        private async Task SearchForFiles()
        {
            // create new instance of Cancel-token for each search
            cancel = new CancellationTokenSource();

            SearchProfile profile = new SearchProfile(InputSearchString, InputExtension);
            FileGatherer gatherer = new FileGatherer(SourceDirectories, profile);
            await gatherer.TraverseSourceDirs(cancel.Token);
            var result = gatherer.GetFoundFiles();

            ObservableCollection<HitFile> localHits = new ObservableCollection<HitFile>();
            foreach (var file in result)
            {
                localHits.Add(file);
            }

            HitList = localHits;
        }

        private async Task CopySelectedFiles(object elements)
        {
            var selectedFiles = new ObservableCollection<HitFile>();

            // first, get selected files
            if (elements != null)
            {                        
                var objList = (IEnumerable<object>)elements; // cast the argument to a collection
                selectedFiles = new ObservableCollection<HitFile>(objList.Select(item => item).Cast<HitFile>()); // use LINQ to cast to type, and add to generic ObsCol
            }

            // then, get marked files
            var marked = HitList.Where(hit => hit.Marked == true);
            var markedFiles = new ObservableCollection<HitFile>(marked);

            // now, join the two lists + filter out duplicates
            var joined = selectedFiles.Concat(markedFiles).Distinct(new HitFileComparer());
            var hitList = new ObservableCollection<HitFile>(joined);

            await FileHandler.CopyHits(hitList, InputTargetFolder);

            // update list af found files to reflect copy-state
            UpdateHitList();

        }

        private void UpdateHitList()
        {
            ObservableCollection<HitFile> temp = new ObservableCollection<HitFile>(HitList.Select(hit => hit));

            HitList.Clear();
            HitList = temp;
        }

        private void AcceptFolderToSources(string folder)
        {
            if (folder.ValidateDirectory())
            {
                SourceDirectory sourceDir = new SourceDirectory(folder);
                var sourceExists = SourceDirectories.Where(item => item.DirectoryName == sourceDir.DirectoryName).FirstOrDefault();  
                
                if (sourceExists == null) SourceDirectories.Add(sourceDir);  //ensure duplicate entries are not allowed     
            }
        }

        private void AcceptFolderToHistory(string folder)
        {
            if (folder.ValidateDirectory())
            {
                TargetDirectory newDir = new TargetDirectory(folder);
                var historyExists = DirectoryHistory.Where(item => item.DirectoryName == newDir.DirectoryName).FirstOrDefault();

                if (historyExists == null) DirectoryHistory.Add(newDir);
            }
        }

        private async void InitializeHistory()
        {
            try
            {
                var fetchHistory = await persistHistory.GetHistory();
                var toList = fetchHistory.ToList();
                var history = new BindingList<LogDirectory>(toList);

                DirectoryHistory = history;
            }
            catch (Exception)
            {
                // if failed to read, we just ignore error and continue
            }
        }
        #endregion

      
    }
}
