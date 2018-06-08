
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using Plugin.Connectivity;
using Rock.Utils;
using TaskManagerBLM.Droid.Sources.Utils;
using WhatMessenger.Droid.Account;
using WhatMessenger.Droid.AuthActivity;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments
{
    public class AccountFragment : MainBaseFragment
    {
        public static AccountFragment GetInstance() => new AccountFragment { Arguments = new Bundle() };
        ProfileViewModel ViewModel;
        Button btPrivacy, btDeleteAccount, btProfile, btChangePassword, btLogout;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewModel = EngineService.EngineInstance.ProfileViewModel;
            var rootView = inflater.Inflate(Resource.Layout.fragment_account, container, false);
            btPrivacy = rootView.FindViewById<Button>(Resource.Id.btPrivacy);
            btProfile = rootView.FindViewById<Button>(Resource.Id.btProfile);
            btDeleteAccount = rootView.FindViewById<Button>(Resource.Id.btDeleteAccount);
            btChangePassword = rootView.FindViewById<Button>(Resource.Id.btChangePass);
            btLogout = rootView.FindViewById<Button>(Resource.Id.btLogout);
            var StringResource = ParentActivity.MyApplication.CurrentLangSetting.GetStringResourceContents();
            btPrivacy.Text = StringResource.GetValueOrDefault("privacy");
            btPrivacy.Click += BtPrivacy_Click;
            btProfile.Text = StringResource.GetValueOrDefault("profile");
            btProfile.Click += BtProfile_Click;
            btDeleteAccount.Text = StringResource.GetValueOrDefault("deleteAccount");
            btDeleteAccount.Click += BtDeleteAccount_Click;
            btChangePassword.Text = StringResource.GetValueOrDefault("changePassword");
            btChangePassword.Click += BtChangePassword_Click;
            btLogout.Text = StringResource.GetValueOrDefault("logout");
            btLogout.Click += BtLogout_Click;
            return rootView;
        }

        public override void OnStart()
        {
            base.OnStart();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void OnStop()
        {
            base.OnStop();
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ProfileViewModel.IsBusy):
                    if(ViewModel.IsBusy)
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            ShowLoadingDialog("Change Account Status");
                        });    
                    }
                    else
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            HideLoadingDialog();
                        });
                    }
                    break;
                case nameof(ProfileViewModel.CloseAccountResult):
                    if(ViewModel.CloseAccountResult.RESULT)
                    {
                        EngineService.EngineInstance.StopThis();
                        GotoLoginScreen();
                    }
                    else
                    {
                        DialogUtils.ShowOKDialog(ParentActivity, @"Warning", @"Failed to close account");
                    }
                    break;
                case nameof(ProfileViewModel.IsSignOut):
                    if(ViewModel.IsSignOut)
                    {
                        HideLoadingDialog();
                        EngineService.EngineInstance.StopThis();
                        GotoLoginScreen();
                    }
                    break;
            }
        }

        void BtLogout_Click(object sender, EventArgs e)
        {
            
            if(CrossConnectivity.Current.IsConnected)
            {
                ViewModel.CommandSignOutAccount.Execute(ParentActivity.MyApplication.Me.TOKEN);    
            }
            else
            {
                PreferenceUtils.saveString(this.ParentActivity, PreferenceUtils.TOKEN, @"");
                GotoLoginScreen();
                DialogUtils.ShowOKDialog(this.ParentActivity, @"Warning", @"No Internet Connection");
            }
        }


        void GotoLoginScreen()
        {
            PreferenceUtils.saveString(this.ParentActivity, PreferenceUtils.TOKEN, @"");
            var intent = new Intent(ParentActivity, typeof(LoginActivity));    
            ParentActivity.StartActivity(intent);
            ParentActivity.Finish();
        }

        void BtPrivacy_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(ParentActivity, typeof(PrivacyActivity));
            StartActivity(mIntent);
        }

        void BtProfile_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(ParentActivity, typeof(MyProfileEditActivity));
            StartActivity(mIntent);
        }

        void BtChangePassword_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(ParentActivity, typeof(PasswordChangeActivity));
            StartActivity(mIntent);
        }

        void BtDeleteAccount_Click(object sender, EventArgs e)
        {
            new AlertDialog.Builder(ParentActivity)
                           .SetTitle(@"Info")
                           .SetMessage(@"Are you sure to close account?")
                           .SetPositiveButton(@"Yes", (sender1, e1) => {
                                CloseAccountHandler();
                            })
                           .SetNegativeButton(@"No", (sender2, e2) => {
                                
                            })
                           .Show();
                           
        }

        void CloseAccountHandler()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                ViewModel.CommandCloseAccount.Execute(ParentActivity.MyApplication.Me.TOKEN);
            }
            else
            {
                DialogUtils.ShowOKDialog(ParentActivity, @"Warning", @"No Internet Connection");
            }
        }
    }
}
