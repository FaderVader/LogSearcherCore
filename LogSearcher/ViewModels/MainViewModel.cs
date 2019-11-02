using LogSearcher.Domain;
using LogSearcher.Models;
using LogSearcher.Utils;
using System;
using System.Collections.Generic;
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
            InputSourceFolder = @"C:\temp\LogSearcherTestFiles\VCP_logs";
            SearchStatus = "";

            // get settings
            var settings = AppSettings.GetSettings();
            SelectUseNPP = settings.UseNPP;

            // wire-up buttons
            GoSearch = new RelayCommandNoParams(StartSearch);
            FolderBrowseButton = new RelayCommandNoParams(FolderBrowse);
            SubmitSourceFolderButton = new RelayCommandNoParams(SubmitSourceFolder);
            ResetSourceFolderDisplayButton = new RelayCommandNoParams(ResetSourceFolderDisplay);
            SubmitTargetFolderButton = new RelayCommandNoParams(SubmitTargetFolder);
            OpenFileButton = new RelayCommandNoParams(OpenFile);
            
            CopyAllButton = new RelayCommandNoParams(CopyAllFiles, EnableCopyAll);            
            CopySelectedButton = new RelayCommandWithParams(CopySelected, EnableCopySelected);

        }


        #region RelayCommands
        public RelayCommandNoParams GoSearch { get; }
        public RelayCommandNoParams FolderBrowseButton { get; }
        public RelayCommandNoParams SubmitSourceFolderButton { get; }
        public RelayCommandNoParams ResetSourceFolderDisplayButton { get; }
        public RelayCommandNoParams SubmitTargetFolderButton { get; }
        public RelayCommandNoParams CopyAllButton { get; }
        public RelayCommandNoParams OpenFileButton { get; }
        public RelayCommandWithParams CopySelectedButton { get; }

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
        }

        public async void CopyAllFiles()
        {
            await FileHandler.CopyHits(HitList, InputTargetFolder);
        }

        public async void CopySelected(object elements)
        {
            var objList = (IEnumerable<object>)elements; // convert object to list

            ObservableCollection<HitFile> hitList = new ObservableCollection<HitFile>();
            foreach (var obj in objList)
            {
                hitList.Add(obj as HitFile);
            }
            await FileHandler.CopyHits(hitList, InputTargetFolder);
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

        public bool EnableCopyAll()
        {
            var result = HitList.Count > 0 ? true : false;
            return result;
        }

        public bool EnableCopySelected()
        {
            var check = SelectedFile != null ? true : false;
            return check;
        }

        #endregion
    }
}
