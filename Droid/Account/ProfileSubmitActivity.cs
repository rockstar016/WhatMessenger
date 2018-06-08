
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
using API.Models.RequestModels;
using Plugin.Connectivity;
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Account
{
    [Activity(Label = "ProfileSubmitActivity")]
    public class ProfileSubmitActivity : BaseActivity
    {
        ProfileViewModel ThisViewModel;
        protected override int LayoutResource => Resource.Layout.activity_profile_edit;
        TextView txtIndicator;
        EditText txtContent;
        Button btCancel, btSave;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ThisViewModel = EngineService.EngineInstance.ProfileViewModel;
            base.OnCreate(savedInstanceState);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "";

            txtIndicator = FindViewById<TextView>(Resource.Id.txtIndicator);
            txtContent = FindViewById<EditText>(Resource.Id.txtContent);
            btCancel = FindViewById<Button>(Resource.Id.btCancel);
            btSave = FindViewById<Button>(Resource.Id.btSave);
            btCancel.Click += (sender, e) => Finish();
            btSave.Click += BtSave_Click;
        }

        void BtSave_Click(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                if (ThisViewModel.IsUpdatePhone)
                {
                    ThisViewModel.CommandUpdatePhone.Execute(new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = txtContent.Text });
                }
                else
                {
                    ThisViewModel.CommandUpdateName.Execute(new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = txtContent.Text });
                }
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
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
            ThisViewModel.PropertyChanged += ThisViewModel_PropertyChanged;
            txtIndicator.Text = ThisViewModel.IsUpdatePhone?@"Edit Phone number":@"Edit User Name";
            txtContent.Text = ThisViewModel.IsUpdatePhone ? ThisViewModel.ME.PHONE.Trim() : ThisViewModel.ME.NAME.Trim();
        }

        protected override void OnStop()
        {
            base.OnStop();
            ThisViewModel.PropertyChanged -= ThisViewModel_PropertyChanged;
        }

        void ThisViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(ProfileViewModel.IsBusy)))
            {
                if (ThisViewModel.IsBusy)
                {
                    ShowLoadingDialog("Saving Information");
                }
                else
                {
                    HideLoadingDialog();
                }
            }

            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.ME)))
            {
                if(ThisViewModel.ME != null)
                {
                    MyApplication.Me = ThisViewModel.ME;
                    this.Finish();
                }
                else
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"Failed to update user information");
                }
            }
        }
    }
}
