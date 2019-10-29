using LogSearcher.Domain;
using LogSearcher.Models;
using LogSearcher.Utils;
using System;
using System.Collections.ObjectModel;
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

            // reset ui
            InputExtension = "*txt";
            InputSearchString = "error";
            InputTargetFolder = @"C:\temp\test";
            SearchStatus = "";

            // get settings
            var settings = AppSettings.GetSettings();
            SelectUseNPP = settings.UseNPP;

            // wire-up buttons
            GoSearch = new CommandNoParams(StartSearch);
            FolderBrowseButton = new CommandNoParams(FolderBrowse);
            SubmitSourceFolderButton = new CommandNoParams(SubmitSourceFolder);
            ResetSourceFolderDisplayButton = new CommandNoParams(ResetSourceFolderDisplay);
            SubmitTargetFolderButton = new CommandNoParams(SubmitTargetFolder);
            OpenFileButton = new CommandNoParams(OpenFile);
            
            CopyAllButton = new CommandNoParams(CopyFiles, EnableCopy);            
            CopySelectedButton = new CommandWithParams(CopySelected);

        }


        #region RelayCommands
        public CommandNoParams GoSearch { get; }
        public CommandNoParams FolderBrowseButton { get; }
        public CommandNoParams SubmitSourceFolderButton { get; }
        public CommandNoParams ResetSourceFolderDisplayButton { get; }
        public CommandNoParams SubmitTargetFolderButton { get; }
        public CommandNoParams CopyAllButton { get; }
        public CommandNoParams OpenFileButton { get; }
        public CommandWithParams CopySelectedButton { get; }

        #endregion

        #region Main lists

        private ObservableCollection<SourceDirectory> sourceDirectories;
        private TargetDirectory targetDirectory;
        private ObservableCollection<HitFile> hitList;
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

        public void FolderBrowse()
        {
            var folder = FileHandler.BrowseForFolder();

            if (Validate.ValidateDirectory(folder))
            {
                SourceDirectory sourceDir = new SourceDirectory(folder);
                SourceDirectories.Add(sourceDir);
            }
        }

        public void SubmitSourceFolder()
        {
            if (Validate.ValidateDirectory(InputSourceFolder))
            {
                SourceDirectory sourceDir = new SourceDirectory(InputSourceFolder);
                sourceDirectories.Add(sourceDir);

                InputSourceFolder = "";  // ensure input-field is cleared, to signal input accepted
            }
        }

        public void ResetSourceFolderDisplay()
        {
            // reset value of SourceFolderDisplay
            SourceDirectories = new ObservableCollection<SourceDirectory>();
        }

        public void SubmitTargetFolder()
        {
            // grab value of TargetDirectory
            // Not currently implemented
            var test = InputTargetFolder;
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
            CopyAllButton.RaiseCanExecuteChanged();
        }

        public async void CopyFiles()
        {
            await FileHandler.CopyHits(HitList, InputTargetFolder);
        }

        public async void CopySelected(object param)
        {
            // convert object / list of object to List<HitFiles>
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
        public async Task SearchForFiles()
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

        //Func<bool> EnableCopy = () => HitList.Count > 0;
        public bool EnableCopy()
        {
            //var result = HitList.Count > 0 ? true : false;
            var result = false;
            return result;
        }

        #endregion
    }
}
