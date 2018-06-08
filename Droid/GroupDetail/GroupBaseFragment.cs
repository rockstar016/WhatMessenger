
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using WhatMessenger.Droid.Utils;

namespace WhatMessenger.Droid.GroupDetail
{
    public class GroupBaseFragment : Android.Support.V4.App.Fragment
    {
        protected AddGroupActivity ParentActivity;
        LoadingIndicator loadingIndicator;
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            ParentActivity = context as AddGroupActivity;
        }
        public override void OnDetach()
        {
            base.OnDetach();
            ParentActivity = null;
        }

        protected void ShowLoadingDialog(string title)
        {
            loadingIndicator = LoadingIndicator.GetInstance(title);
            loadingIndicator.Show(ChildFragmentManager, "dialog");
        }

        protected void HideLoadingDialog()
        {
            if (loadingIndicator != null)
            {
                loadingIndicator.Dismiss();
            }
            loadingIndicator = null;
        }

    }
}
