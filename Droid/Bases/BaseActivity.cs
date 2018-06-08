
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Util;
using Rock.Utils;
using WhatMessenger.Droid.Utils;

namespace WhatMessenger.Droid.Bases
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class BaseActivity : AppCompatActivity
    {
        public MainApplication MyApplication;
        LoadingIndicator loadingIndicator;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MyApplication = Application as MainApplication;
            var valueLayout = LayoutResource;
            SetContentView(valueLayout);
        }

        protected virtual int LayoutResource
        {
            get;
        }

        protected void ShowLoadingDialog(string title)
        {
            loadingIndicator = LoadingIndicator.GetInstance(title);
            loadingIndicator.Show(SupportFragmentManager, "dialog");
        }

        protected void HideLoadingDialog()
        {
            if (loadingIndicator != null)
            {
                loadingIndicator.Dismiss();
            }
            loadingIndicator = null;
        }

        protected int GetMyUserId()
        {
            return MyApplication.Me.USERID;
        }

        protected string GetMyUserName()
        {
            return MyApplication.Me.NAME;
        }

		protected override void OnStop()
		{
            base.OnStop();
		}

		protected override void OnDestroy()
		{
            base.OnDestroy();
		}
	}
}
