
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
using WhatMessenger.Droid.Bases;

namespace WhatMessenger.Droid.AuthActivity
{
    public class RegisterTerms_Fragment : RegisterBaseFragment
    {
        public static RegisterTerms_Fragment GetInstance() => new RegisterTerms_Fragment { Arguments = new Bundle() };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.fragment_register_privacy, container, false);
            var btAgree = rootView.FindViewById<Button>(Resource.Id.btAgreeTerms);
            btAgree.Click += BtAgree_Click;
            Dictionary<string, string> StringResource = ParentActivity.MyApplication.CurrentLangSetting.GetStringResourceContents();
            rootView.FindViewById<TextView>(Resource.Id.txtTitle).Text = StringResource.GetValueOrDefault("welcome");
            rootView.FindViewById<TextView>(Resource.Id.txtHtmlLink).Text = StringResource.GetValueOrDefault("view_privacy");
            btAgree.Text = StringResource.GetValueOrDefault("agree_terms");
            return rootView;
        }

        void BtAgree_Click(object sender, EventArgs e) {
            //ParentActivity.RegisterModel.AGREETERM = true;
            ParentActivity.SetFragment(RegisterActvity.EMAIL_FRAGMENT);
        }
    }
}
