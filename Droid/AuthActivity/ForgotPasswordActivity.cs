
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WhatMessenger.Droid.Bases;
using WhatMessenger.ViewModel.ViewModels;

namespace WhatMessenger.Droid.AuthActivity
{
    [Activity(Label = "")]
    public class ForgotPasswordActivity : BaseActivity
    {
        private int CURRENT_FRAGMENT = 0;
        public const int EMAIL_SUBMIT_FRAGMENT = 1;
        public const int NEW_PASSWORD_FRAGMENT = 2;
        Android.Support.V7.Widget.Toolbar toolbar;
        protected override int LayoutResource => Resource.Layout.activity_register;
        public LoginViewModel ViewModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ViewModel = new LoginViewModel();
            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SetFragment(EMAIL_SUBMIT_FRAGMENT);
        }
  
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                if (CURRENT_FRAGMENT == EMAIL_SUBMIT_FRAGMENT)
                {
                    BackToLogin();
                }
                else
                {
                    SetFragment(CURRENT_FRAGMENT - 1);
                }
            }
            return true;
        }

		protected override void OnStart()
		{
            base.OnStart();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		protected override void OnStop()
		{
            base.OnStop();
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
		}


        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        void BackToLogin()
        {
            var intentLoginIntent = new Intent(this, typeof(LoginActivity));
            StartActivity(intentLoginIntent);
            Finish();
        }

        public void SetFragment(int FragmentType)
        {
            Android.Support.V4.App.Fragment f = null;
            switch (FragmentType)
            {
                case EMAIL_SUBMIT_FRAGMENT:
                    f = ForgotEmail_Fragment.GetInstance();
                    break;
                case NEW_PASSWORD_FRAGMENT:
                    f = ForgotPassword_Fragment.GetInstance();
                    break;
            }
            CURRENT_FRAGMENT = FragmentType;
            AttachFragment(f);
        }

        void AttachFragment(Android.Support.V4.App.Fragment f)
        {
            if (f != null)
            {
                while (SupportFragmentManager.BackStackEntryCount > 0)
                {
                    SupportFragmentManager.PopBackStackImmediate();
                }
                SupportFragmentManager.BeginTransaction()
                                      .Replace(Resource.Id.registerContainer, f)
                                      .Commit();
            }
        }

	}
}
