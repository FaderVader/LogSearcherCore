using LogSearcher.Domain;
using LogSearcher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using static LogSearcher.Domain.Utils;

namespace LogSearcher.ViewModels
{
    class MainViewModel : ObservableObject
    {


        public MainViewModel()
        {
            SourceDirectories = new ObservableCollection<SourceDirectory>();
            HitList = new ObservableCollection<HitFile>();

            InputExtension = "";
            InputSearchString = "";
            SearchStatus = ""; // Found Files:

            SelectUseNPP = true; //Properties.Settings.Default.UseNPP; // UI reflects config-setting

            // wire-up buttons
            GoSearch = new RelayCommand(StartSearch);
            FolderBrowseButton = new RelayCommand(FolderBrowse);
            SubmitSourceFolderButton = new RelayCommand(SubmitSourceFolder);
            ResetSourceFolderDisplayButton = new RelayCommand(ResetSourceFolderDisplay);
            SubmitTargetFolderButton = new RelayCommand(SubmitTargetFolder);
            OpenFileButton = new RelayCommand(OpenFile);
        }

        #region RelayCommands
        public RelayCommand GoSearch { get; }
        public RelayCommand FolderBrowseButton { get; }
        public RelayCommand SubmitSourceFolderButton { get; }
        public RelayCommand ResetSourceFolderDisplayButton { get; }
        public RelayCommand SubmitTargetFolderButton { get; }
        public RelayCommand OpenFileButton { get; }
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

            if (Utils.ValidateDirectory(folder))
            {
                SourceDirectory sourceDir = new SourceDirectory(folder);
                SourceDirectories.Add(sourceDir);
            }
        }
        
        public void SubmitSourceFolder()
        {
            if (Utils.ValidateDirectory(InputSourceFolder))
            {
                SourceDirectory sourceDir = new SourceDirectory(InputSourceFolder);
                sourceDirectories.Add(sourceDir);

                InputSourceFolder = "";
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
            SearchStatus = "Found Files:";
        }

        public void OpenFile()
        {
            var useNPP = SelectUseNPP;    //Properties.Settings.Default.UseNPP;
            if (useNPP)
            {
                FileHandler.SendToNotePadPP(SelectedFile);
                return;
            }
            FileHandler.OpenWithFile(SelectedFile);
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
        #endregion
    }
}
