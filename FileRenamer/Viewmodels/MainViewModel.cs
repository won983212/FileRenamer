using FileRenamer.Libs;
using System;
using System.Windows.Input;

namespace FileRenamer.Viewmodels
{
    public class MainViewModel : ObservableObject
    {
        public ICommand FindTargetDirectoryCommand => new RelayCommand(() => Console.WriteLine("Find"));

        public ICommand RenameCommand => new RelayCommand(() => Console.WriteLine("Rename"));

        public ICommand FFMpegCommand => new RelayCommand(() => Console.WriteLine("FFMpeg"));
    }
}
