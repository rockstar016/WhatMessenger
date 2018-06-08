using System;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace WhatMessenger.Droid.Utils
{
    public class LoadingIndicator : Android.Support.V4.App.DialogFragment
    {
        string title;
        public static LoadingIndicator GetInstance(string title)
        {
            Bundle bundle = new Bundle();
            bundle.PutString("TITLE", title);
            var LoadingIndicator = new LoadingIndicator() { Arguments = bundle };
            return LoadingIndicator;
        }

        public LoadingIndicator()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            title = Arguments.GetString("TITLE");
        }
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            
            var RootView = inflater.Inflate(Resource.Layout.loading_spinner_dialog, container, false);
            TextView txtName = RootView.FindViewById<TextView>(Resource.Id.txtindicator);
            txtName.Text = title;
            this.Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            this.Dialog.SetCancelable(false);
            this.Dialog.SetCanceledOnTouchOutside(false);
            this.SetStyle(DialogFragment.StyleNoTitle, 0);
            return RootView;
        }
		
	}
}
