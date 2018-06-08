
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Plugin.Connectivity;
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Utils;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel.ViewModels;

namespace WhatMessenger.Droid.AuthActivity
{
    public class ForgotPassword_Fragment : ForgotPasswordBaseFragment
    {
        public static ForgotPassword_Fragment GetInstance() => new ForgotPassword_Fragment { Arguments = new Bundle() };
        TextView txtTitle;
        TextInputLayout editEmail, editNewPassword, editConfirmPassword;
        Button btSubmit;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.fragment_forgotpassword_newpassl, container, false);
            txtTitle = rootView.FindViewById<TextView>(Resource.Id.txtTitle);
            editEmail = rootView.FindViewById<TextInputLayout>(Resource.Id.editEmail);
            editNewPassword = rootView.FindViewById<TextInputLayout>(Resource.Id.editNewPassword);
            editConfirmPassword = rootView.FindViewById<TextInputLayout>(Resource.Id.editConfirmPassword);
            btSubmit = rootView.FindViewById<Button>(Resource.Id.btSubmit);
            btSubmit.Click += BtSubmit_Click;
            return rootView;
        }

		public override void OnStart()
		{
            base.OnStart();
            ParentActivity.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		public override void OnStop()
		{
            base.OnStop();
            ParentActivity.ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
		}

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(LoginViewModel.IsBusy)))
            {
                if(ParentActivity.ViewModel.IsBusy)
                {
                    ShowLoadingDialog(@"Submit New Password");
                }
                else
                {
                    HideLoadingDialog();
                }
            }
            else if(string.Equals(e.PropertyName, nameof(LoginViewModel.ChangePasswordResult)))
            {
                if(ParentActivity.ViewModel.ChangePasswordResult != null && ParentActivity.ViewModel.ChangePasswordResult.RESULT)
                {
                    //success
                    Toast.MakeText(this.ParentActivity, @"Password changed successfully", ToastLength.Long).Show();
                    this.ParentActivity.Finish();
                }
                else
                {
                    //failed
                    DialogUtils.ShowOKDialog(this.ParentActivity, @"Warning", @"Failed to change password");
                }
            }
        }

		void BtSubmit_Click(object sender, EventArgs e)
        {
            if (StringCheckUtil.isEmpty(editEmail.EditText)) return;
            if (StringCheckUtil.isLength(editNewPassword.EditText, 6)) return;
            if (StringCheckUtil.isEmpty(editNewPassword.EditText)) return;
            if (StringCheckUtil.isEmpty(editConfirmPassword.EditText)) return;
            if (!StringCheckUtil.CompareText(editNewPassword.EditText, editConfirmPassword.EditText)) return;
            if(CrossConnectivity.Current.IsConnected)
            {
                var phoneIMEI = PhoneIMEI.GetImei(this.ParentActivity);
                var requestModel = new ChangePasswordRequest(){ CODE = editEmail.EditText.Text.Trim(), IMEI = phoneIMEI, NEW_PASS = editNewPassword.EditText.Text};
                ParentActivity.ViewModel.ChangePasswordCommand.Execute(requestModel);
            }
            else
            {
                DialogUtils.ShowOKDialog(this.ParentActivity, @"Warning", @"No Internet Connection");
            }
        }

    }
}
