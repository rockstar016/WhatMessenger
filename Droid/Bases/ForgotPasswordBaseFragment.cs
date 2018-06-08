
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
using WhatMessenger.Droid.AuthActivity;
using WhatMessenger.Droid.Utils;

namespace WhatMessenger.Droid.Bases
{
    public class ForgotPasswordBaseFragment : Android.Support.V4.App.Fragment
    {
        LoadingIndicator loadingIndicator;
        protected ForgotPasswordActivity ParentActivity;
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            ParentActivity = context as ForgotPasswordActivity;
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
