
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Widget;
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Utils;

namespace WhatMessenger.Droid.AuthActivity
{
    public class RegisterEmail_Fragment : RegisterBaseFragment
    {
        public static RegisterEmail_Fragment GetInstance() => new RegisterEmail_Fragment { Arguments = new Bundle() };
        TextInputLayout txtEmail, txtPassword;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.fragment_register_email, container, false);
            var btAgreeTerms = rootView.FindViewById<Button>(Resource.Id.btNext);
            btAgreeTerms.Click += BtNext_Click;
            Dictionary<string, string> StringResource = ParentActivity.MyApplication.CurrentLangSetting.GetStringResourceContents();
            rootView.FindViewById<TextView>(Resource.Id.txtTitle).Text = StringResource.GetValueOrDefault("HeaderTitleRegister");
            rootView.FindViewById<TextView>(Resource.Id.txtDescription).Text = StringResource.GetValueOrDefault("HeaderRegisterDescription");
            txtEmail = rootView.FindViewById<TextInputLayout>(Resource.Id.editEmail);
            txtEmail.EditText.Hint = StringResource.GetValueOrDefault("EmailPlaceHolder");
            txtPassword = rootView.FindViewById<TextInputLayout>(Resource.Id.editPassword);
            txtPassword.EditText.Hint = StringResource.GetValueOrDefault("PasswordPlaceHolder");
            btAgreeTerms.Text = StringResource.GetValueOrDefault("Next");

            return rootView;
        }

        public override void OnStart()
        {
            base.OnStart();
            txtEmail.EditText.Text = ParentActivity.RegisterModel.EMAIL == null ? @"" : ParentActivity.RegisterModel.EMAIL;
            txtPassword.EditText.Text = ParentActivity.RegisterModel.USERPASSWORD == null ? @"" : ParentActivity.RegisterModel.USERPASSWORD;
        }

        void BtNext_Click(object sender, EventArgs e)
        {
            if(StringCheckUtil.isEmpty(txtEmail.EditText))
            {
                return;
            }

            if (!StringCheckUtil.isEmailAddress(txtEmail.EditText))
            {
                return;
            }

            if(StringCheckUtil.isEmpty(txtPassword.EditText))
            {
                return;
            }

            if(StringCheckUtil.isLength(txtPassword.EditText, 8))
            {
                return;
            }
            ParentActivity.RegisterModel.EMAIL = txtEmail.EditText.Text;
            ParentActivity.RegisterModel.USERPASSWORD = txtPassword.EditText.Text;
            ParentActivity.RegisterModel.IMEI = PhoneIMEI.GetImei(ParentActivity);
            ParentActivity.SetFragment(RegisterActvity.PROFILE_FRAGMENT);
        }
    }
}
