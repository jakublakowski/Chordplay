using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ChordPlay.Helpers
{
    public static class DialogHelper
    {
        public static async Task<ContentDialogResult> DisplayConfirmCancelDialog(string title, string content, string primaryBtnText = "OK", string secondaryButtonText = "Cancel")
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = primaryBtnText,
                SecondaryButtonText = secondaryButtonText
            };

            ContentDialogResult result = await dialog.ShowAsync();
            return result;
        }

        public static async void DisplayInformationDialog(string title,string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = "Ok"
            };

            ContentDialogResult result = await dialog.ShowAsync();
        }
    }
}
