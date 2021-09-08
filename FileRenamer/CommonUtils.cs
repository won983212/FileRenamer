using FileRenamer.Libs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer
{
    public static class CommonUtils
    {
        public delegate void DialogCompleteEventHandler<T>(T vm, DialogClosingEventArgs eventArgs)
            where T : ObservableObject;

        public static string FillFront(string text, char fill, int fullLength)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = text.Length; i < fullLength; i++)
                sb.Append(fill);
            sb.Append(text);
            return sb.ToString();
        }

        public static Task<object> ShowDialog<T>(T content, DialogCompleteEventHandler<T> closingHandler = null)
            where T : ObservableObject
        {
            return ShowDialog(content, "RootDialogHost", closingHandler);
        }

        public static Task<object> ShowDialog<T>(T content, string dialog,
            DialogCompleteEventHandler<T> closingHandler = null) where T : ObservableObject
        {
            return DialogHost.Show(content, dialog,
                (o, e) => closingHandler?.Invoke((T)((DialogHost)o).DialogContent, e));
        }

        public static void CloseDialog()
        {
            DialogHost.Close("RootDialogHost");
        }
    }
}
