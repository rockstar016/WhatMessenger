
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using TaskManagerBLM.Droid.Sources.Utils;
using WhatMessenger.Droid.Bases;

namespace WhatMessenger.Droid.AuthActivity
{
    public class RegisterLang_Fragment : RegisterBaseFragment
    {
        public static RegisterLang_Fragment GetInstance() => new RegisterLang_Fragment { Arguments = new Bundle() };

        AppCompatRadioButton radioEng, radioChinese;
        Button btNext;
        TextView txtTitle;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
          
            var rootView = inflater.Inflate(Resource.Layout.fragment_register_langsetting, container, false);
            radioEng = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioEng);
            radioChinese = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioChinese);
            btNext = rootView.FindViewById<Button>(Resource.Id.btNext);
            txtTitle = rootView.FindViewById<TextView>(Resource.Id.txtTitle);
            radioEng.CheckedChange += RadioEng_CheckedChange;
            radioChinese.CheckedChange += RadioChinese_CheckedChange;
            btNext.Click += BtNext_Click;
            return rootView;
        }

        public override void OnStart()
        {
            base.OnStart();

            InitView();
        }

        void InitView()
        {
            var langDic = ParentActivity.MyApplication.CurrentLangSetting.GetStringResourceContents();
            txtTitle.Text = langDic.GetValueOrDefault("welcome");
            btNext.Text = langDic.GetValueOrDefault("Next");
        }

        void RadioEng_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if(e.IsChecked == true)
            {
                ParentActivity.RegisterModel.USERLANG = @"0";
                PreferenceUtils.saveString(ParentActivity, PreferenceUtils.LANG, @"EN");
                ParentActivity.MyApplication.InitLangResource(@"EN");
                InitView();    
            }
        }

        void RadioChinese_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if(e.IsChecked == true)
            {
                ParentActivity.RegisterModel.USERLANG = @"1";
                PreferenceUtils.saveString(ParentActivity, PreferenceUtils.LANG, @"CH");
                ParentActivity.MyApplication.InitLangResource(@"CH");
                InitView();    
            }
        }

        void BtNext_Click(object sender, EventArgs e)
        {
            ParentActivity.SetFragment(RegisterActvity.PRIVACY_FRAGMENT);
        }
    }
}
