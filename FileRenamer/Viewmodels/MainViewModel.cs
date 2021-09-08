using FileRenamer.Libs;
using FileRenamer.Pages.ViewModels.Dialogs;
using FileRenamer.Validations;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FileRenamer.Viewmodels
{
    public class MainViewModel : ObservableObject
    {
        private string targetDirectory = "";
        private string targetRegex = "";
        private string replacementRegex = "";
        private bool canUseFFMpeg = false;


        public MainViewModel()
        {
            UpdateFileList();
            ReplacedFileInfo.LoadIconMap();
        }

        private void UpdateFileList()
        {
            if (!Directory.Exists(TargetDirectory))
                return;

            Files.Clear();

            foreach (string dir in Directory.EnumerateDirectories(TargetDirectory))
            {
                Files.Add(new ReplacedFileInfo(ReplacedFileInfo.DirectoryType, Path.GetFileName(dir)));
            }

            foreach (string file in Directory.EnumerateFiles(TargetDirectory))
            {
                Files.Add(new ReplacedFileInfo(Path.GetExtension(file).ToLower(), Path.GetFileName(file)));
            }

            ApplyFilenameRegex();
        }

        private void ApplyFilenameRegex()
        {
            if (string.IsNullOrWhiteSpace(TargetRegex))
            {
                foreach (ReplacedFileInfo file in Files)
                {
                    file.ReplacedName = file.OriginalName;
                    file.IsSelected = false;
                }
                return;
            }

            CanUseFFMpeg = true;

            int i = 1;
            int inc = 1;
            string replaceRegex = ReplacementRegex;
            bool isEmpty = true;

            if (replaceRegex.StartsWith("inc"))
            {
                Match m = RegexValidationRule.MatchIncreaseToken(replaceRegex);
                if (m.Success && m.Groups.Count == 3)
                {
                    i = int.Parse(m.Groups[1].Value);
                    inc = int.Parse(m.Groups[2].Value);
                    replaceRegex = replaceRegex.Substring(m.Value.Length);
                }
            }

            foreach (ReplacedFileInfo file in Files)
            {
                // $i, $i(n) syntax processing
                string replRegex = replaceRegex;
                foreach (Match match in Regex.Matches(replRegex, "\\$i\\(([0-9]+)\\)"))
                {
                    int endIndex = match.Index + match.Value.Length;
                    string front = replRegex.Substring(0, match.Index);
                    string end = replRegex.Length <= endIndex ? "" : replRegex.Substring(endIndex);
                    string repl = CommonUtils.FillFront(i.ToString(), '0', int.Parse(match.Groups[1].Value));
                    replRegex = front + repl + end;
                }
                replRegex = replRegex.Replace("$i", i.ToString());

                // actual replace name
                file.ReplacedName = Regex.Replace(file.OriginalName, targetRegex, replRegex);
                file.IsSelected = file.IsReplaced;
                if (file.IsSelected)
                {
                    CanUseFFMpeg &= file.IsFFMpegSupports;
                    i += inc;
                    isEmpty = false;
                }
            }

            CanUseFFMpeg &= !isEmpty;
        }

        private void FindTargetDirectory()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "대상 폴더 지정";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok || string.IsNullOrWhiteSpace(dialog.FileName))
                return;

            TargetDirectory = dialog.FileName;
        }

        private void DoRename()
        {
            MessageBoxResult result = MessageBox.Show("바꾸면 되돌릴 수 없습니다. 정말로 바꿉니까?", "주의", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                foreach (ReplacedFileInfo file in Files)
                {
                    if (!file.IsReplaced)
                        continue;

                    string src = Path.Combine(TargetDirectory, file.OriginalName);
                    string dest = Path.Combine(TargetDirectory, file.ReplacedName);
                    File.Move(src, dest);
                }

                UpdateFileList();
                MessageBox.Show("이름을 변경했습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        public string TargetDirectory
        {
            get => targetDirectory;
            set
            {
                SetProperty(ref targetDirectory, value);
                UpdateFileList();
            }
        }

        public string TargetRegex
        {
            get => targetRegex;
            set
            {
                SetProperty(ref targetRegex, value);
                ApplyFilenameRegex();
            }
        }

        public string ReplacementRegex
        {
            get => replacementRegex;
            set
            {
                SetProperty(ref replacementRegex, value);
                ApplyFilenameRegex();
            }
        }

        public bool CanUseFFMpeg
        {
            get => canUseFFMpeg;
            set => SetProperty(ref canUseFFMpeg, value);
        }

        public ObservableCollection<ReplacedFileInfo> Files { get; } = new ObservableCollection<ReplacedFileInfo>();
        public ICommand FindTargetDirectoryCommand => new RelayCommand(FindTargetDirectory);
        public ICommand RenameCommand => new RelayCommand(DoRename);
        public ICommand FFMpegCommand => new RelayCommand(() => Console.WriteLine("FFMpeg"));
        public ICommand ShowExtendedReplRegexCommand => new RelayCommand(() => CommonUtils.ShowDialog(new HelpExtendedSyntaxVM()));
    }
}
