
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using Plugin.Connectivity;
using Rock.Utils;
using TaskManagerBLM.Droid.Sources.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.Utils;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Account
{
    [Activity(Label = "PasswordChangeActivity")]
    public class PasswordChangeActivity : BaseActivity
    {
        ProfileViewModel ThisViewModel;
        protected override int LayoutResource => Resource.Layout.activity_password_change;

        Button btCancel, btSave;
        EditText txtCurrentPassword, txtNewPassword, txtConfirmNewPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ThisViewModel = EngineService.EngineInstance.ProfileViewModel;
            base.OnCreate(savedInstanceState);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Password change";

            btCancel = FindViewById<Button>(Resource.Id.btCancel);
            btSave = FindViewById<Button>(Resource.Id.btSave);
            txtCurrentPassword = FindViewById<EditText>(Resource.Id.txtCurrentPassword);
            txtNewPassword = FindViewById<EditText>(Resource.Id.txtNewPassword);
            txtConfirmNewPassword = FindViewById<EditText>(Resource.Id.txtConfirmNewPassword);

            btCancel.Click += BtCancel_Click;
            btSave.Click += BtSave_Click;
        }

        void BtSave_Click(object sender, EventArgs e)
        {
            if (StringCheckUtil.isEmpty(txtCurrentPassword)) return;
            if (StringCheckUtil.isEmpty(txtNewPassword)) return;
            if (StringCheckUtil.isEmpty(txtConfirmNewPassword)) return;
            if (StringCheckUtil.isLength(txtNewPassword, 8)) return;
            if(StringCheckUtil.CompareText(txtNewPassword, txtConfirmNewPassword))
            {
                //update password
                if (CrossConnectivity.Current.IsConnected)
                {
                    var model = new ResetPasswordRequest()
                    {
                        EMAIL = MyApplication.Me.EMAIL,
                        IMEI = PhoneIMEI.GetImei(this),
                        NEW_PASS = txtNewPassword.Text,
                        OLD_PASS = txtCurrentPassword.Text
                    };
                    ThisViewModel.CommandUpdatePassword.Execute(model);
                }
                else
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                }
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"New password doesn't match");
            }
        }

        void BtCancel_Click(object sender, EventArgs e)
        {
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (ThisViewModel == null)
            { 
                EngineService.EngineInstance.ProfileViewModel = new ProfileViewModel();
                ThisViewModel = EngineService.EngineInstance.ProfileViewModel;
            }
            ThisViewModel.ME = null;

            ThisViewModel.PropertyChanged += ThisViewModel_PropertyChanged;

        }

        protected override void OnStop()
        {
            base.OnStop();
            ThisViewModel.PropertyChanged -= ThisViewModel_PropertyChanged;
        }

        private void ThisViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(ProfileViewModel.IsBusy)))
            {
                if (ThisViewModel.IsBusy)
                {
                    ShowLoadingDialog("Change Password");
                }
                else
                {
                    HideLoadingDialog();
                }
            }

            if (string.Equals(e.PropertyName, nameof(ProfileViewModel.ME)))
            {
                if (ThisViewModel.ME != null)
                {
                    MyApplication.Me = ThisViewModel.ME;
                    PreferenceUtils.saveString(this, PreferenceUtils.TOKEN, ThisViewModel.ME.TOKEN);
                    Toast.MakeText(this, @"User password is changed successfully", ToastLength.Long).Show();
                    Finish();
                }
                else
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"Failed to update user information");
                }
            }
        }
    }
}
