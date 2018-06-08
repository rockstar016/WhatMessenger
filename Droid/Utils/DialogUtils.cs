using System;
using Android.Content;
using Android.Support.V7.App;

namespace Rock.Utils
{
    public class DialogUtils
    {
        public DialogUtils()
        {
        }
        public static void ShowOKDialog(Context context, string title, string content)
        {
            new AlertDialog.Builder(context)
                           .SetTitle(title)
                           .SetMessage(content)
                           .SetPositiveButton("Ok", (sender, e) =>
                           {
                               (sender as IDialogInterface).Dismiss();
                           })
                           .Create()
                           .Show();
        }
        public static void ShowOKDialog(Context context, string title, string content, Action CompletionHandler)
        {
            new AlertDialog.Builder(context)
                           .SetTitle(title)
                           .SetMessage(content)
                           .SetPositiveButton("Ok", (sender, e) =>
                           {
                                CompletionHandler.Invoke();
                           })
                           .Create()
                           .Show();
        }

        public static void ShowOkCancelDialog(Context context, string title, string content, Action OkHandler, Action CancelHandler)
        {
            new AlertDialog.Builder(context)
                           .SetTitle(title)
                           .SetMessage(content)
                           .SetPositiveButton("Ok", (sender, e) =>
                           {
                               OkHandler.Invoke();
                           })
                           .SetNegativeButton("Cancel", (sender1, e1) =>
                           {
                               CancelHandler.Invoke();
                           })
                           .Create()
                           .Show();
        }
    }
}
