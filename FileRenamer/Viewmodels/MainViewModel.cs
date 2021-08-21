using FileRenamer.Libs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FileRenamer.Viewmodels
{
    public class MainViewModel : ObservableObject
    {
        private string targetDirectory = "";
        private string targetRegex = "";
        private string replacementRegex = "";


        public MainViewModel()
        {
            UpdateFileList();
        }

        private void UpdateFileList()
        {
            Files.Add(new ReplacedFileInfo() { Type = FileType.Directory, OriginalName = "mydirectory", ReplacedName = "mydirectory" });
            Files.Add(new ReplacedFileInfo() { Type = FileType.Directory, OriginalName = "this_what", ReplacedName = "you_what" });
            Files.Add(new ReplacedFileInfo() { Type = FileType.File, OriginalName = "test1.txt", ReplacedName = "test1.txt" });
            Files.Add(new ReplacedFileInfo() { Type = FileType.File, OriginalName = "test2.txt", ReplacedName = "test2.txt" });
            Files.Add(new ReplacedFileInfo() { Type = FileType.File, OriginalName = "test3.txt", ReplacedName = "test312.bat" });
        }


        public string TargetDirectory
        {
            get => targetDirectory;
            set => SetProperty(ref targetDirectory, value);
        }

        public string TargetRegex
        {
            get => targetRegex;
            set
            {
                SetProperty(ref targetRegex, value);
                UpdateFileList();
            }
        }

        public string ReplacementRegex
        {
            get => replacementRegex;
            set
            {
                SetProperty(ref replacementRegex, value);
                UpdateFileList();
            }
        }

        public ObservableCollection<ReplacedFileInfo> Files { get; } = new ObservableCollection<ReplacedFileInfo>();
        public ICommand FindTargetDirectoryCommand => new RelayCommand(() => Console.WriteLine("Find"));
        public ICommand RenameCommand => new RelayCommand(() => Console.WriteLine("Rename"));
        public ICommand FFMpegCommand => new RelayCommand(() => Console.WriteLine("FFMpeg"));
    }

    public enum FileType
    {
        Directory, File
    }

    public class ReplacedFileInfo
    {
        public FileType Type { get; set; }
        public string ReplacedName { get; set; }
        public string OriginalName { get; set; }

        public string TypeIcon => GetTypeIcon();
        public bool IsReplaced => ReplacedName != OriginalName;


        private string GetTypeIcon()
        {
            switch (Type)
            {
                case FileType.Directory:
                    return "FolderOutline";
                default:
                    return "FileOutline";
            }
        }
    }
}
