using FileRenamer.Libs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer
{
    public class ReplacedFileInfo : ObservableObject
    {
        public static readonly string DirectoryType = "Directory";

        private static readonly Dictionary<string, string> IconMap = new Dictionary<string, string>();
        private string replacedName;
        private bool isSelected = false;


        public ReplacedFileInfo(string type, string name)
        {
            Type = type;
            ReplacedName = name;
            OriginalName = name;
        }


        private string GetTypeIcon()
        {
            if (!IconMap.TryGetValue(Type, out string icon))
                return "FileOutline";
            return icon;
        }

        public static void LoadIconMap()
        {
            if (IconMap.Count > 0)
                return;

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("FileRenamer.FileIconMap.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string[] result = reader.ReadToEnd().Split('\n');
                string icon = null;
                foreach (string entry in result)
                {
                    if (string.IsNullOrWhiteSpace(entry) || entry.StartsWith("#"))
                        continue;

                    if (entry.StartsWith("@"))
                        icon = entry.Substring(1).Trim();
                    else if (icon != null)
                        IconMap.Add(entry.Trim(), icon);
                }
            }
        }

        public string Type { get; }
        public string OriginalName { get; }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public string ReplacedName
        {
            get => replacedName;
            set 
            { 
                SetProperty(ref replacedName, value);
                OnPropertyChanged(nameof(IsReplaced));
            }
        }

        public string TypeIcon => GetTypeIcon();
        public bool IsReplaced => ReplacedName != OriginalName;
        public bool IsFFMpegSupports => TypeIcon == "FileVideoOutline" || TypeIcon == "FileMusicOutline";
        public bool IsDirectory => Type == DirectoryType;
    }
}
