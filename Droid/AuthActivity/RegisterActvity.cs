
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using API.Models.RequestModels;
using Plugin.Connectivity;
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Utils;
using WhatMessenger.ViewModel.ViewModels;
using Plugin.Permissions;
using System;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using WhatMessenger.Model.Constants;
using Plugin.CurrentActivity;

namespace WhatMessenger.Droid.AuthActivity
{
    [Activity(Label = "", Theme = "@style/AppTheme.NoActionBar")]
    public class RegisterActvity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_register;
        Android.Support.V7.Widget.Toolbar toolbar;

        private int CURRENT_FRAGMENT = 0;
        public const int PROFILE_FRAGMENT = 3;
        public const int EMAIL_FRAGMENT = 2;
        public const int PRIVACY_FRAGMENT = 1;
        public const int LANG_FRAGMENT = 0;
        public RegisterRequest RegisterModel { get; set; }
        LoginViewModel ViewModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ViewModel = new LoginViewModel();

            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            RegisterModel = new RegisterRequest();
            CrossCurrentActivity.Current.Activity = this;
            CheckAppPermission();
        }

        async void CheckAppPermission()
        {
            var permissionResult = await CheckNecessaryPermissions();
            if(!permissionResult)
            {
                BackToLogin();    
            }
            else
            {
                SetFragment(LANG_FRAGMENT);
            }
        }

        async Task<bool> CheckNecessaryPermissions()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            var phoneStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Phone);
            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted || phoneStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage, Permission.Phone });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
                phoneStatus = results[Permission.Phone];
            }
            return await Task.FromResult(cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted && phoneStatus == PermissionStatus.Granted);    
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
            if (e.PropertyName == nameof(LoginViewModel.IsBusy))
            {
                if (ViewModel.IsBusy)
                {
                    this.RunOnUiThread(()=>{
                        ShowLoadingDialog("Register");    
                    });
                }
                else
                {
                    this.RunOnUiThread(()=>{
                        HideLoadingDialog();
                    });

                    if (ViewModel.RegisterResultModel == null)
                    {
                        DialogUtils.ShowOKDialog(this, @"Warning", @"Server Error");
                        return;
                    }
                    else
                    {
                        if(ViewModel.RegisterResultModel.RESULT)
                        {
                            DialogUtils.ShowOKDialog(this, @"Info", @"We sent you verification email. Check email to verify yourself.", ()=>{
                                BackToLogin();    
                            });
                        }
                        else
                        {
                            if (string.Equals(ViewModel.RegisterResultModel.MSG, GlobalConstants.RESPONSE_DUPLICATE_ERROR))
                            {
                                DialogUtils.ShowOKDialog(this, @"Warning", @"This device is already registered.");
                            }
                            else
                            {
                                DialogUtils.ShowOKDialog(this, @"Warning", @"Server Error");
                            }
                        }
                    }
                }
            }
        }


        void BackToLogin()
        {
            var intentLoginIntent = new Intent(this, typeof(LoginActivity));
            StartActivity(intentLoginIntent);
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                if(CURRENT_FRAGMENT == LANG_FRAGMENT)
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

        #region Register Fragment Handler
        public void SetFragment(int FragmentType)
        {
            Android.Support.V4.App.Fragment f = null;
            switch (FragmentType)
            {
                case LANG_FRAGMENT:
                    f = RegisterLang_Fragment.GetInstance();
                    break;
                case PRIVACY_FRAGMENT:
                    f = RegisterTerms_Fragment.GetInstance();
                    break;
                case EMAIL_FRAGMENT:
                    f = RegisterEmail_Fragment.GetInstance();
                    break;
                case PROFILE_FRAGMENT:
                    f = RegisterProfile_Fragment.GetInstance();
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
        #endregion

        public void DoRegister()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                ViewModel.RegisterCommand.Execute(RegisterModel);
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
        }

        public void GoMainScreen()
        {
            var IntentMain = new Intent(this, typeof(MainActivity));
            StartActivity(IntentMain);
            Finish();
        }
    }
}
