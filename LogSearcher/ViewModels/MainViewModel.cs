using LogSearcher.Domain;
using LogSearcher.Models;
using LogSearcher.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            InputSearchString = "error";
            InputTargetFolder = @"C:\temp\test";
            InputSourceFolder = @"C:\temp\LogSearcherTestFiles\VCP_logs";
            SearchStatus = "";

            // get settings
            settings = AppSettings.GetSettings();
            persistHistory = new PersistHistory(settings.HistoryPath);
            SelectUseNPP = settings.UseNPP;

            // wire-up buttons
            GoSearch = new RelayCommandNoParams(StartSearch);
            OpenSourceFolderButton = new RelayCommandNoParams(BrowseForSourceFolder);
            SubmitSourceFolderButton = new RelayCommandNoParams(SubmitSourceFolder);
            ResetSourceFolderDisplayButton = new RelayCommandNoParams(ResetSourceFolderDisplay);
            OpenTargetFolderButton = new RelayCommandNoParams(OpenTargetFolder);
            OpenFileButton = new RelayCommandNoParams(OpenFile);
            CopyAllButton = new RelayCommandNoParams(CopyAllFiles, EnableCopyAll);
            CopySelectedButton = new RelayCommandWithParams(CopySelected, EnableCopySelected);
            SetInputFromHistoryButton = new RelayCommandWithParams(SetSourceFolder);
            OpenExplorerButton = new RelayCommandWithParams(OpenExplorer);

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
        public RelayCommandWithParams OpenExplorerButton { get; }

        #endregion

        #region Main lists

        private Settings settings;
        private PersistHistory persistHistory;

        private ObservableCollection<SourceDirectory> sourceDirectories;
        private TargetDirectory targetDirectory;
        private ObservableCollection<HitFile> hitList;
        private BindingList<LogDirectory> directoryHistory;
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

        #endregion

        #region Button-methods
        public void BrowseForSourceFolder()
        {
            var target = InputSourceFolder.ValidateDirectory() == true ? InputSourceFolder : "";
            var folder = FileHandler.BrowseForFolder(target);
            AcceptSourceFolder(folder);
        }
        public void SubmitSourceFolder()
        {
            AcceptSourceFolder(InputSourceFolder);
        }
        public void SetSourceFolder(object parameter)
        {
            if (parameter == null) return;
            var dir = parameter as SourceDirectory;
            InputSourceFolder = dir.DirectoryName;
        }
        public void ResetSourceFolderDisplay()
        {
            // reset value of SourceFolderDisplay
            SourceDirectories = new ObservableCollection<SourceDirectory>();
            HitList = new ObservableCollection<HitFile>();
        }
        public void OpenTargetFolder()
        {
            var target = InputTargetFolder.ValidateDirectory() == true ? InputTargetFolder : "";
            InputTargetFolder = FileHandler.BrowseForFolder(target);
        }
        public async void StartSearch()
        {
            SearchStatus = "Searching...";
            HitList.Clear();
            await SearchForFiles();

            if (HitList.Count < 1)
            {
                SearchStatus = "No files found!";
                return;
            }
            SearchStatus = "Found Files:";
            await persistHistory.SaveHistory(DirectoryHistory);
        }
        public async void CopyAllFiles()
        {
            await FileHandler.CopyHits(HitList, InputTargetFolder);
            UpdateHitList();
        }
        public async void CopySelected(object elements)
        {
            if (elements == null) return;
            await CopySelectedFiles(elements);
        }
        public async void OpenExplorer(object path)
        {
            if (path != null && (path as string).ValidateDirectory())
            {
                LogDirectory directory = new SourceDirectory(path as string);
                await FileHandler.OpenExplorer(directory);
            }
        }
        public void OpenFile()
        {
            var useNPP = SelectUseNPP;
            if (useNPP)
            {
                FileHandler.OpenWithNPP(SelectedFile);
                return;
            }
            FileHandler.OpenWithDefault(SelectedFile);
        }
        #endregion

        #region Methods
        private async Task SearchForFiles()
        {
            SearchProfile profile = new SearchProfile(InputSearchString, InputExtension);
            FileGatherer gatherer = new FileGatherer(SourceDirectories, profile);
            await gatherer.TraverseSourceDirs();
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
                //TODO: there must be a LINQ expression like js objList.map(x => selectedFiles.Add(x as HitFile)) ??
                var objList = (IEnumerable<object>)elements;
                foreach (var obj in objList)
                {
                    selectedFiles.Add(obj as HitFile);
                }
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
            var temp = new ObservableCollection<HitFile>();

            foreach (var hit in HitList)
            {
                temp.Add(hit);
            }

            HitList.Clear();
            HitList = temp;
        }

        private void AcceptSourceFolder(string folder)
        {
            if (folder.ValidateDirectory())
            {
                SourceDirectory sourceDir = new SourceDirectory(folder);
                var sourceExists = SourceDirectories.Where(item => item.DirectoryName == sourceDir.DirectoryName).FirstOrDefault();
                var historyExists = DirectoryHistory.Where(item => item.DirectoryName == sourceDir.DirectoryName).FirstOrDefault();

                if (sourceExists == null) SourceDirectories.Add(sourceDir);  //ensure duplicate entries are not allowed
                if (historyExists == null) DirectoryHistory.Add(sourceDir);

                InputSourceFolder = "";  // ensure input-field is cleared, to signal input accepted

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
    }
}
