
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
    public class ForgotEmail_Fragment : ForgotPasswordBaseFragment
    {
        public static ForgotEmail_Fragment GetInstance() => new ForgotEmail_Fragment { Arguments = new Bundle() };
        TextView txtTitle, txtDescription;
        TextInputLayout editEmail;
        Button btSubmit, btHaveCode;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView =  inflater.Inflate(Resource.Layout.fragment_forgotpassword_email, container, false);
            txtTitle = rootView.FindViewById<TextView>(Resource.Id.txtTitle);
            txtDescription = rootView.FindViewById<TextView>(Resource.Id.txtDescription);
            editEmail = rootView.FindViewById<TextInputLayout>(Resource.Id.editEmail);
            btSubmit = rootView.FindViewById<Button>(Resource.Id.btSubmit);
            btHaveCode = rootView.FindViewById<Button>(Resource.Id.btHaveCode);

            btSubmit.Click += BtSubmit_Click;
            btHaveCode.Click += BtHaveCode_Click;
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
                    ShowLoadingDialog("Submit E-mail");    
                }
                else
                {
                    HideLoadingDialog();
                }
            }
            else if(string.Equals(e.PropertyName, nameof(LoginViewModel.AskPasswordResult)))
            {
                if(ParentActivity.ViewModel.AskPasswordResult != null && ParentActivity.ViewModel.AskPasswordResult.RESULT)
                {
                    ParentActivity.SetFragment(ForgotPasswordActivity.NEW_PASSWORD_FRAGMENT);
                }
                else
                {
                    DialogUtils.ShowOKDialog(this.ParentActivity, @"Warning", @"Failed to submit E-mail.");
                }
            }
        }


		void BtSubmit_Click(object sender, EventArgs e)
        {
            if (StringCheckUtil.isEmpty(editEmail.EditText)) return;
            if (!StringCheckUtil.isEmailAddress(editEmail.EditText)) return;
            if(CrossConnectivity.Current.IsConnected)
            {
                var ImeiCode = PhoneIMEI.GetImei(ParentActivity);
                var requestModel = new AskPasswordChangeRequest() { EMAIL = editEmail.EditText.Text.Trim(), IMEI = ImeiCode };
                ParentActivity.ViewModel.AskChangePasswordCommand.Execute(requestModel);
            }
            else
            {
                DialogUtils.ShowOKDialog(this.ParentActivity, @"Warning", @"No Internet Connection");
            }
        }

        void BtHaveCode_Click(object sender, EventArgs e)
        {
            ParentActivity.SetFragment(ForgotPasswordActivity.NEW_PASSWORD_FRAGMENT);
        }
	}
}
